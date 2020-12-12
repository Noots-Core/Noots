import { Component, OnInit, Inject, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DialogData } from '../dialog_data';
import { EnumUtil } from '../../services/enum.util';
import { NoteColorPallete } from '../../enums/NoteColors';
import { Theme } from '../../enums/Theme';
import { Store, Select } from '@ngxs/store';
import { ChangeColorFullNote, ChangeColorNote, UnSelectAllNote } from 'src/app/content/notes/state/notes-actions';
import { UserStore } from 'src/app/core/stateUser/user-state';
import { Observable } from 'rxjs/internal/Observable';
import { AppStore } from 'src/app/core/stateApp/app-state';
import { ChangeColorFolder } from 'src/app/content/folders/state/folders-actions';
import { NoteStore } from 'src/app/content/notes/state/notes-state';
import { FolderStore } from 'src/app/content/folders/state/folders-state';

@Component({
  selector: 'app-change-color',
  templateUrl: './change-color.component.html',
  styleUrls: ['./change-color.component.scss']
})
export class ChangeColorComponent implements OnInit, OnDestroy {

  @Select(AppStore.isNote)
  public isNote$: Observable<boolean>;

  @Select(AppStore.isFolder)
  public isFolder$: Observable<boolean>;



  @Select(UserStore.getUserTheme)
  public theme$: Observable<Theme>;

  pallete = EnumUtil.getEnumValues(NoteColorPallete);
  current;
  theme = Theme;

  date: Date;
  constructor(public dialogRef: MatDialogRef<ChangeColorComponent>,
              @Inject(MAT_DIALOG_DATA) public data: DialogData,
              private store: Store, ) { }

  ngOnInit(): void {
    this.date = new Date();
    this.current = this.pallete[0];
  }

  changeCurrent(color: string) {
    this.current = color;
  }

  async changeColor() { // TODO
    let routePath = this.store.selectSnapshot(AppStore.isNote);
    if (routePath) {
      const isInner = this.store.selectSnapshot(AppStore.isNoteInner);
      if (isInner) {
        await this.store.dispatch(new ChangeColorFullNote(this.current)).toPromise();
      } else {
        const ids = this.store.selectSnapshot(NoteStore.selectedIds);
        const type = this.store.selectSnapshot(AppStore.getTypeNote);
        await this.store.dispatch(new ChangeColorNote(this.current, type, ids)).toPromise();
      }
    }
    routePath = this.store.selectSnapshot(AppStore.isFolder);
    if (routePath) {
      const type = this.store.selectSnapshot(AppStore.getTypeFolder);
      const ids = this.store.selectSnapshot(FolderStore.selectedIds);
      await this.store.dispatch(new ChangeColorFolder(this.current, type, ids)).toPromise();
    }
    this.dialogRef.close();
  }

  shadeColor(color, percent) {
    return '#' + color.replace(/^#/, '')
    .replace(/../g, colorN => ('0' + Math.min(255, Math.max(0, parseInt(colorN, 16) + percent)).toString(16)).substr(-2));
  }

  ngOnDestroy(): void {
    this.store.dispatch(new UnSelectAllNote());
  }
}
