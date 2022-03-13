import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import * as moment from 'moment';
import { environment } from 'src/environments/environment';
import { Store } from '@ngxs/store';
import * as mm from 'music-metadata-browser';
import { generateFormData } from 'src/app/core/defaults/form-data-generator';
import { StreamAudioState } from './models/stream-audio-state.model';
import { AudioEvents } from './models/enums/audio-events.enum';
import { NoteStore } from './state/notes-state';
import { AudioModel } from './models/editor-models/audios-collection';
import { ApiNoteFilesService } from './full-note/services/api-note-files.service';
import { FileNoteTypes } from './full-note/models/file-note-types.enum';

@Injectable({
  providedIn: 'root',
})
export class AudioService {
  audioEvents = AudioEvents;

  currentFile: AudioModel;

  playlist: AudioModel[] = [];

  volumeHelper: number;

  private state: StreamAudioState = {
    id: '',
    playing: false,
    readableCurrentTime: undefined,
    currentTime: undefined,
    canplay: false,
    error: false,
    loop: false,
    currentVolume: 0.25,
  };

  private stop$ = new Subject();

  private audioObj = new Audio();

  private stateChange: BehaviorSubject<StreamAudioState> = new BehaviorSubject(this.state);

  constructor(private store: Store, private apiNoteFilesService: ApiNoteFilesService) {}

  getState(): Observable<StreamAudioState> {
    return this.stateChange;
  }

  resetCurrent() {
    this.currentFile = null;
    this.playlist = null;
    this.stop();
    this.resetState();
    this.stateChange.next(this.state);
  }

  playStream(url, id) {
    return this.streamObservable(url, id).pipe(takeUntil(this.stop$));
  }

  play() {
    this.audioObj.play();
  }

  pause() {
    this.audioObj.pause();
  }

  stop() {
    this.stop$.next();
  }

  loop() {
    this.audioObj.loop = !this.audioObj.loop;
    this.state.loop = this.audioObj.loop;
  }

  mute() {
    const { currentVolume } = this.state;
    if (this.audioObj.muted) {
      this.audioObj.muted = false;
      this.seekToVolume(this.volumeHelper);
    } else {
      this.audioObj.muted = true;
      this.volumeHelper = currentVolume;
      this.seekToVolume(0);
    }
  }

  seekTo(seconds) {
    this.audioObj.currentTime = seconds;
  }

  seekToVolume(volume) {
    this.audioObj.volume = volume;
  }

  formatTime = (time: number, format: string = 'mm:ss'): string => {
    const momentTime = time * 1000;
    return moment.utc(momentTime).format(format);
  };

  getAudioUrl(url: string) {
    return escape(url);
  }

  async tryToUpdateMetaDataIfNeed(audio: AudioModel) {
    if (audio.isNeedUpdateMetaData()) {
      try {
        const [file, duration] = await this.getMetadata(audio.audioPath);
        const noteId = this.store.selectSnapshot(NoteStore.oneFull)?.id;
        let fileId = null;
        if (file && noteId) {
          const formData = generateFormData(file);
          const response = await this.apiNoteFilesService
            .uploadFilesToNoteNoProgressReport(formData, noteId, FileNoteTypes.Photo)
            .toPromise();
          if (response?.data && response?.data[0]) {
            fileId = response.data[0].id;
          }
        }
        const resp = await this.apiNoteFilesService
          .updateFileMetaData(audio.fileId, Math.floor(duration), fileId)
          .toPromise();
        if (resp.success && resp?.data?.metaData) {
          // eslint-disable-next-line no-param-reassign
          audio.secondsDuration = resp.data.metaData?.secondsDuration;
          // eslint-disable-next-line no-param-reassign
          audio.pathToImage = resp.data.metaData?.imagePath;
        }
      } catch (e) {
        console.log(e);
      }
    }
  }

  private async getMetadata(audioPath): Promise<[Blob, number]> {
    let imageBlob: Blob;

    const metadata = await mm.fetchFromUrl(this.getAudioUrl(audioPath), {
      skipPostHeaders: true,
      includeChapters: false,
      duration: false,
    });

    if (metadata && metadata.common && metadata.common && metadata.common.picture) {
      const arrayBufferView = new Uint8Array(metadata.common.picture[0].data.buffer);
      imageBlob = new Blob([arrayBufferView], { type: 'image/png' });
    }

    return [imageBlob, metadata.format.duration];
  }

  private streamObservable(url, id) {
    return new Observable((observer) => {
      this.state.id = id;
      this.audioObj.src = this.getAudioUrl(url);
      this.audioObj.volume = this.state.currentVolume;
      this.audioObj.load();

      const handler = (event: Event) => {
        this.updateStateEvents(event);
        observer.next(event);
      };

      this.addEvents(this.audioObj, Object.keys(this.audioEvents), handler);
      return () => {
        this.audioObj.pause();
        this.audioObj.currentTime = 0;
        this.removeEvents(this.audioObj, Object.keys(this.audioEvents), handler);
        this.resetState();
      };
    });
  }

  private addEvents = (obj, events, handler) => {
    events.forEach((event) => {
      obj.addEventListener(event, handler);
    });
  };

  private removeEvents = (obj, events, handler) => {
    events.forEach((event) => {
      obj.removeEventListener(event, handler);
    });
  };

  private updateStateEvents(event: Event): void {
    switch (event.type) {
      case this.audioEvents.play:
      case this.audioEvents.loadstart:
      case this.audioEvents.loadedmetadata:
      case this.audioEvents.canplay:
        this.state.canplay = true;
        this.play();
        break;
      case this.audioEvents.playing:
        this.state.playing = true;
        break;
      case this.audioEvents.pause:
        this.state.playing = false;
        break;
      case this.audioEvents.timeupdate:
        this.state.currentTime = this.audioObj.currentTime;
        this.state.readableCurrentTime = this.formatTime(this.state.currentTime);
        break;
      case this.audioEvents.volumechange:
        this.state.currentVolume = this.audioObj.volume;
        if (this.state.currentVolume) {
          this.audioObj.muted = false;
        }
        break;
      case this.audioEvents.ended:
        if (this.audioObj.loop) {
          this.state.currentTime = 0;
          this.state.playing = true;
          this.state.loop = true;
          this.play();
        }
        break;
      case this.audioEvents.error:
        this.resetState();
        this.state.error = true;
        break;
      default:
        throw new Error('Error in audio player');
    }
    this.stateChange.next(this.state);
  }

  private resetState() {
    this.state = {
      id: '',
      playing: false,
      readableCurrentTime: undefined,
      currentTime: undefined,
      canplay: false,
      error: false,
      loop: false,
      currentVolume: 0.25,
    };
  }
}
