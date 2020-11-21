import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { PersonalizationService } from 'src/app/shared/services/personalization.service';
import { Store } from '@ngxs/store';
import { LoadArchiveNotes, UnSelectAllNote, LoadAllExceptNotes } from '../state/notes-actions';
import { take, takeUntil } from 'rxjs/operators';
import { NoteType } from 'src/app/shared/enums/NoteTypes';
import { UserStore } from 'src/app/core/stateUser/user-state';
import { SpinnerChangeStatus, UpdateRoute } from 'src/app/core/stateApp/app-action';
import { EntityType } from 'src/app/shared/enums/EntityTypes';
import { NoteStore } from '../state/notes-state';
import { FontSize } from 'src/app/shared/enums/FontSize';
import { MurriService } from 'src/app/shared/services/murri.service';
import { NotesService } from '../notes.service';

@Component({
  selector: 'app-archive',
  templateUrl: './archive.component.html',
  styleUrls: ['./archive.component.scss'],
  providers: [NotesService]
})
export class ArchiveComponent implements OnInit, OnDestroy {

  fontSize = FontSize;
  destroy = new Subject<void>();
  loaded = false;

  constructor(public pService: PersonalizationService,
              private store: Store,
              public murriService: MurriService,
              public noteService: NotesService) { }

  async ngOnInit() {
    await this.store.dispatch(new UpdateRoute(EntityType.NoteArchive)).toPromise();
    await this.store.dispatch(new SpinnerChangeStatus(true)).toPromise();
    this.store.select(UserStore.getTokenUpdated)
    .pipe(takeUntil(this.destroy))
    .subscribe(async (x: boolean) => {
      if (x) {
        await this.loadContent();
      }
    }
    );
  }

  async loadContent() {
    await this.store.dispatch(new LoadArchiveNotes()).toPromise();
    this.store.dispatch(new LoadAllExceptNotes(NoteType.Archive));


    const notes = this.store.selectSnapshot(NoteStore.archiveNotes);
    this.noteService.firstInit(notes);

    await this.pService.waitPreloading();
    this.store.dispatch(new SpinnerChangeStatus(false));
    this.loaded = true;
    await this.murriService.initMurriNoteAsync(NoteType.Archive, !this.noteService.isFiltedMode());
    await this.murriService.setOpacityTrueAsync();
  }


  ngOnDestroy(): void {
    this.murriService.flagForOpacity = false;
    this.murriService.muuriDestroy();
    this.destroy.next();
    this.destroy.complete();
    this.store.dispatch(new UnSelectAllNote());
  }
}
