import { Injectable } from '@angular/core';
import { MatSnackBarRef, TextOnlySnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { take } from 'rxjs/operators';
import { AppStore } from 'src/app/core/stateApp/app-state';
import { LoadUsedDiskSpace } from 'src/app/core/stateUser/user-action';
import { UserStore } from 'src/app/core/stateUser/user-state';
import { NoteTypeENUM } from 'src/app/shared/enums/note-types.enum';
import { RefTypeENUM } from 'src/app/shared/enums/ref-type.enum';
import { SnackBarWrapperService } from 'src/app/shared/services/snackbar/snack-bar-wrapper.service';
import { FolderStore } from '../../folders/state/folders-state';
import {
  CopyNotes,
  ChangeTypeNote,
  UpdateFullNote,
  DeleteNotesPermanently,
} from '../../notes/state/notes-actions';
import { NoteStore } from '../../notes/state/notes-state';
import { DialogsManageService } from './dialogs-manage.service';

@Injectable({
  providedIn: 'root',
})
export class MenuButtonsNotesService {
  constructor(
    private store: Store,
    private sbws: SnackBarWrapperService,
    private apiTranslate: TranslateService,
    private dialogsService: DialogsManageService,
  ) {}

  openDeletionNoteModal(): void {
    const instance = this.dialogsService.openDeletionPopup(
      'modal.deletionModal.sureDeleteNotes',
      'modal.deletionModal.additionalMessage',
    );
    instance
      .afterClosed()
      .pipe(take(1))
      .subscribe((x) => {
        if (x) {
          this.deleteNotes();
        }
      });
  }

  copyNotes() {
    const isInnerNote = this.store.selectSnapshot(AppStore.isNoteInner);
    const isInnerFolder = this.store.selectSnapshot(AppStore.isFolderInner);
    const pr = this.store.selectSnapshot(UserStore.getPersonalizationSettings);
    if (isInnerNote) {
      const note = this.store.selectSnapshot(NoteStore.oneFull);
      const ids = [note.id];
      this.store.dispatch(new CopyNotes(ids, pr));
    } else {
      const folderId = isInnerFolder ? this.store.selectSnapshot(FolderStore.full).id : null;
      const ids = this.store.selectSnapshot(NoteStore.selectedIds);
      this.store.dispatch(new CopyNotes(ids, pr, folderId));
    }
  }

  setDeleteNotes = () => {
    const ids = this.getSelectedNoteIds();
    const message =
      this.sbws.getNotesNaming(ids.length > 1) +
      this.sbws.getMoveToMessage(ids.length > 1) +
      this.apiTranslate.instant('snackBar.toBin');
    const successCallback = () =>
      this.successNoteCallback(ids, this.getSelectedNoteType(), NoteTypeENUM.Deleted, message);
    const command = new ChangeTypeNote(
      NoteTypeENUM.Deleted,
      ids,
      false,
      this.permissionsErrorMessage(),
      successCallback,
    );
    this.store.dispatch(command);
  };

  setPrivateNotes = () => {
    const ids = this.getSelectedNoteIds();
    const message =
      this.sbws.getNotesNaming(ids.length > 1) +
      this.sbws.getMoveToMessage(ids.length > 1) +
      this.apiTranslate.instant('snackBar.toPrivate');
    const successCallback = () =>
      this.successNoteCallback(ids, this.getSelectedNoteType(), NoteTypeENUM.Private, message);
    const command = new ChangeTypeNote(
      NoteTypeENUM.Private,
      ids,
      false,
      this.permissionsErrorMessage(),
      successCallback,
    );
    this.store.dispatch(command);
  };

  archiveNotes = () => {
    const ids = this.getSelectedNoteIds();
    const message =
      this.sbws.getNotesNaming(ids.length > 1) +
      this.sbws.getMoveToMessage(ids.length > 1) +
      this.apiTranslate.instant('snackBar.archive');
    const successCallback = () =>
      this.successNoteCallback(ids, this.getSelectedNoteType(), NoteTypeENUM.Archive, message);
    const command = new ChangeTypeNote(
      NoteTypeENUM.Archive,
      ids,
      false,
      this.permissionsErrorMessage(),
      successCallback,
    );
    this.store.dispatch(command);
  };

  private permissionsErrorMessage = (): string =>
    this.apiTranslate.instant('snackBar.onlyAuthorCanMoveIt');

  private getSelectedNoteIds(): string[] {
    if (this.store.selectSnapshot(AppStore.isNoteInner)) {
      const note = this.store.selectSnapshot(NoteStore.oneFull);
      return [note.id];
    }
    return this.store.selectSnapshot(NoteStore.selectedIds);
  }

  private getSelectedNoteType(): NoteTypeENUM {
    if (this.store.selectSnapshot(AppStore.isNoteInner)) {
      const note = this.store.selectSnapshot(NoteStore.oneFull);
      return note.noteTypeId;
    }
    return this.store.selectSnapshot(AppStore.getTypeNote);
  }

  private successNoteCallback = (
    ids: string[],
    typeFrom: NoteTypeENUM,
    typeTo: NoteTypeENUM,
    message: string,
  ) => {
    this.sbws.build(() => {
      this.store.dispatch(this.getRevertActionNotes(typeFrom, ids));
      this.changeFullNoteType(typeFrom);
    }, message);
    this.changeFullNoteType(typeTo);
  };

  // eslint-disable-next-line class-methods-use-this
  private getRevertActionNotes(type: NoteTypeENUM, ids): ChangeTypeNote {
    const types = NoteTypeENUM;
    switch (type) {
      case types.Private: {
        return new ChangeTypeNote(NoteTypeENUM.Private, ids, true);
      }
      case types.Shared: {
        return new ChangeTypeNote(NoteTypeENUM.Private, ids, true, null, null, RefTypeENUM.Viewer);
      }
      case types.Archive: {
        return new ChangeTypeNote(NoteTypeENUM.Archive, ids, true);
      }
      case types.Deleted: {
        return new ChangeTypeNote(NoteTypeENUM.Deleted, ids, true);
      }
      default: {
        throw new Error('incorrect type');
      }
    }
  }

  private changeFullNoteType(typeTo: NoteTypeENUM) {
    if (this.store.selectSnapshot(AppStore.isNoteInner)) {
      this.store.dispatch(new UpdateFullNote({ noteTypeId: typeTo }));
    }
  }

  private async deleteNotes(): Promise<MatSnackBarRef<TextOnlySnackBar>> {
    const isInnerNote = this.store.selectSnapshot(AppStore.isNoteInner);

    if (isInnerNote) {
      const note = this.store.selectSnapshot(NoteStore.oneFull);
      const idsInner = [note.id];
      await this.store.dispatch(new DeleteNotesPermanently(idsInner)).toPromise();
      return this.sbws.buildNotification(
        this.apiTranslate.instant('snackBar.notePermDeleted'),
        null,
      );
    }
    const idsOuter = this.store.selectSnapshot(NoteStore.selectedIds);
    await this.store.dispatch(new DeleteNotesPermanently(idsOuter)).toPromise();

    this.store.dispatch(LoadUsedDiskSpace);

    const message =
      idsOuter.length > 1
        ? this.apiTranslate.instant('snackBar.notesPermDeleted')
        : this.apiTranslate.instant('snackBar.notePermDeleted');
    return this.sbws.buildNotification(message, null);
  }
}
