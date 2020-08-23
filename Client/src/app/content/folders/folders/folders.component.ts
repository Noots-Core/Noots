import { Component, OnInit, OnDestroy } from '@angular/core';
import { Theme } from 'src/app/shared/enums/Theme';
import { PersonalizationService, sideBarCloseOpen } from 'src/app/shared/services/personalization.service';
import { Subject, Observable } from 'rxjs';
import { takeUntil, take } from 'rxjs/operators';
import { Folder } from '../models/folder';
import { FolderStore } from '../state/folders-state';
import { Select, Store } from '@ngxs/store';
import { LoadAllFolders, AddFolder } from '../state/folders-actions';
import { Router } from '@angular/router';
import { UserStore } from 'src/app/core/stateUser/user-state';
import { UpdateRoutePath } from 'src/app/core/stateApp/app-action';
import { RoutePathes } from 'src/app/shared/enums/RoutePathes';


@Component({
  selector: 'app-folders',
  templateUrl: './folders.component.html',
  styleUrls: ['./folders.component.scss'],
  animations: [ sideBarCloseOpen ]
})
export class FoldersComponent implements OnInit, OnDestroy {


  @Select(UserStore.getUserTheme)
  public theme$: Observable<Theme>;

  @Select(FolderStore.privateCount)
  public countPrivate: Observable<number>;

  @Select(FolderStore.sharedCount)
  public countShared: Observable<number>;

  @Select(FolderStore.deletedCount)
  public countDeleted: Observable<number>;

  @Select(FolderStore.archiveCount)
  public countArchive: Observable<number>;


  destroy = new Subject<void>();

  theme = Theme;

  constructor(public pService: PersonalizationService,
              private store: Store,
              private router: Router) { }

  ngOnDestroy(): void {
    this.destroy.next();
    this.destroy.complete();
  }

  async ngOnInit() {

    this.store.dispatch(new UpdateRoutePath(RoutePathes.Folder));

    this.store.select(UserStore.getTokenUpdated)
    .pipe(takeUntil(this.destroy))
    .subscribe(async (x: boolean) => {
      if (x) {
        await this.store.dispatch(new LoadAllFolders()).toPromise();
      }
    });

    this.pService.onResize();
    this.pService.subject
    .pipe(takeUntil(this.destroy))
    .subscribe(x => this.newFolder());
  }

  async newFolder() {
    await this.store.dispatch(new AddFolder()).toPromise();
    this.store.select(FolderStore.privateFolders).pipe(take(1)).subscribe(x => this.router.navigate([`folders/${x[0].id}`]));
  }
}
