import { Component, OnInit, OnDestroy } from '@angular/core';
import {
  PersonalizationService,
  showMenuLeftRight
} from 'src/app/shared/services/personalization.service';
import { Theme } from 'src/app/shared/enums/Theme';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserStore } from 'src/app/core/stateUser/user-state';
import { Select, Store } from '@ngxs/store';
import { MakePublicNotes, MakePrivateNotes } from '../../notes/state/notes-actions';
import { MakePublicFolders, MakePrivateFolders } from '../../folders/state/folders-actions';
import { AppStore } from 'src/app/core/stateApp/app-state';
import { NoteStore } from '../../notes/state/notes-state';
import { FolderStore } from '../../folders/state/folders-state';
import {
  UpdateMenuActive,
} from 'src/app/core/stateApp/app-action';
import { MenuButtonsService } from '../menu-buttons.service';
import { EntityType } from 'src/app/shared/enums/EntityTypes';
import { NoteType } from 'src/app/shared/enums/NoteTypes';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  animations: [showMenuLeftRight]
})
export class HeaderComponent implements OnInit, OnDestroy {

  public countSelected: number;

  destroy = new Subject<void>();

  newButtonActive = false;
  // Upper Menu

  @Select(AppStore.getSettingsButtonActive)
  public settingsButtonActive$: Observable<boolean>;

  @Select(AppStore.getSelectAllButtonActive)
  public selectAllButtonActive$: Observable<boolean>;

  @Select(AppStore.getMenuActive)
  public menuActive$: Observable<boolean>;

  @Select(AppStore.getdefaultBackground)
  public defaultBackground$: Observable<boolean>;

  @Select(AppStore.isNoteInner)
  public isNoteInner$: Observable<boolean>;

  @Select(AppStore.getName)
  public route$: Observable<string>;

  @Select(UserStore.getUserTheme)
  public theme$: Observable<Theme>;

  theme = Theme;

  constructor(public pService: PersonalizationService,
              private store: Store,
              public menuButtonService: MenuButtonsService) { }

  ngOnDestroy(): void {
    this.destroy.next();
    this.destroy.complete();
  }

  ngOnInit(): void {

    this.store.select(AppStore.getNewButtonActive)
      .pipe(takeUntil(this.destroy))
      .subscribe(z => {
        console.log(z);
        this.newButtonActive = z;
      });

    this.store.select(NoteStore.activeMenu)
      .pipe(takeUntil(this.destroy))
      .subscribe(x => this.configShowMenu(x));

    this.store.select(FolderStore.activeMenu)
      .pipe(takeUntil(this.destroy))
      .subscribe(x => this.configShowMenu(x));

    this.store.select(AppStore.getRouting)
      .pipe(takeUntil(this.destroy))
      .subscribe(x => this.routeChange(x));

    this.store.select(NoteStore.selectedCount)
      .pipe(takeUntil(this.destroy))
      .subscribe(x => {
        if (x > 0) {
          this.countSelected = x;
        }
      });

    this.store.select(FolderStore.selectedCount)
      .pipe(takeUntil(this.destroy))
      .subscribe(x => {
        if (x > 0) {
          this.countSelected = x;
        }
      });
  }

  showUsers() {
    this.pService.users = !this.pService.users;
  }

  hideMenu() {
    this.pService.hideInnerMenu = !this.pService.hideInnerMenu;
  }


  disableTooltpUser(): boolean {
    if (this.pService.checkWidth()) {
      return true;
    }
  }

  closeMenu(): void {
    if (this.pService.checkWidth()) {
      this.pService.users = false;
    }

    if (!this.pService.check()) {
      this.pService.hideInnerMenu = false;
    }
  }

  configShowMenu(flag: boolean) {
    if (this.store.selectSnapshot(AppStore.isNoteInner)) {
      return;
    }
    if (flag) {
      this.store.dispatch(new UpdateMenuActive(true));
    } else {
      this.store.dispatch(new UpdateMenuActive(false));
    }
  }

  async routeChange(type: EntityType) {

    switch (type) {
      case EntityType.FolderPrivate: {
        this.menuButtonService.setItems(this.menuButtonService.foldersItemsPrivate);
        break;
      }
      case EntityType.FolderShared: {
        this.menuButtonService.setItems(this.menuButtonService.foldersItemsShared);
        break;
      }
      case EntityType.FolderArchive: {
        this.menuButtonService.setItems(this.menuButtonService.foldersItemsArchive);
        break;
      }
      case EntityType.FolderDeleted: {
        this.menuButtonService.setItems(this.menuButtonService.foldersItemsDeleted);
        break;
      }
      case EntityType.NotePrivate: {
        this.menuButtonService.setItems(this.menuButtonService.notesItemsPrivate);
        break;
      }
      case EntityType.NoteShared: {
        this.menuButtonService.setItems(this.menuButtonService.notesItemsShared);
        break;
      }
      case EntityType.NoteArchive: {
        this.menuButtonService.setItems(this.menuButtonService.notesItemsArchive);
        break;
      }
      case EntityType.NoteDeleted: {
        this.menuButtonService.setItems(this.menuButtonService.notesItemsDeleted);
        break;
      }
      case EntityType.NoteInner: {
        switch (this.store.selectSnapshot(AppStore.getInnerNoteType)) {
          case NoteType.Private: {
            this.menuButtonService.setItems(this.menuButtonService.notesItemsPrivate);
            break;
          }
          case NoteType.Shared: {
            this.menuButtonService.setItems(this.menuButtonService.notesItemsShared);
            break;
          }
          case NoteType.Deleted: {
            this.menuButtonService.setItems(this.menuButtonService.notesItemsDeleted);
            break;
          }
          case NoteType.Archive: {
            this.menuButtonService.setItems(this.menuButtonService.notesItemsArchive);
            break;
          }
        }
        break;
      }
      default: {
        console.log('default');
      }
    }
  }


  makePublic() {
    const noteType = this.store.selectSnapshot(AppStore.getRouting);
    this.store.dispatch(new MakePublicNotes(noteType));
  }

  makePrivate() {
    const noteType = this.store.selectSnapshot(AppStore.getRouting);
    this.store.dispatch(new MakePrivateNotes(noteType));
  }

  makePublicFolder() {
    const folderType = this.store.selectSnapshot(AppStore.getRouting);
    this.store.dispatch(new MakePublicFolders(folderType));
  }

  makePrivateFolder() {
    const folderType = this.store.selectSnapshot(AppStore.getRouting);
    this.store.dispatch(new MakePrivateFolders(folderType));
  }

}
