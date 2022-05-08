import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Select, Store } from '@ngxs/store';
import { Observable, Subject, Subscription } from 'rxjs';
import { UpdateRoute } from 'src/app/core/stateApp/app-action';
import { UserStore } from 'src/app/core/stateUser/user-state';
import { EntityType } from 'src/app/shared/enums/entity-types.enum';
import { PersonalizationService } from 'src/app/shared/services/personalization.service';
import { ContentModelBase } from '../../models/editor-models/content-model-base';
import { LoadSnapshotNote } from '../../state/notes-actions';
import { NoteStore } from '../../state/notes-state';
import { NoteSnapshotState } from '../models/history/note-snapshot-state.model';
import { NoteSnapshot } from '../models/history/note-snapshot.model';
import { ApiNoteHistoryService } from '../services/api-note-history.service';

@Component({
  selector: 'app-note-snapshot',
  templateUrl: './note-snapshot.component.html',
  styleUrls: ['./note-snapshot.component.scss'],
})
export class NoteSnapshotComponent implements OnInit, OnDestroy {
  @Select(UserStore.getUserBackground)
  public userBackground$: Observable<string>;

  @Select(NoteStore.snapshotState)
  public snapshotState$: Observable<NoteSnapshotState>;

  @Select(NoteStore.snapshotNote)
  public snapshotNote$: Observable<NoteSnapshot>;

  @Select(NoteStore.snapshotNoteTitle)
  public snapshotNoteTitle$: Observable<string>;

  destroy = new Subject<void>();

  contents: ContentModelBase[];

  isLoading = true;

  private routeSubscription: Subscription;

  private noteId: string;

  private snapshotId: string;

  constructor(
    public pService: PersonalizationService,
    private store: Store,
    private route: ActivatedRoute,
    private apiHistory: ApiNoteHistoryService,
  ) {
    this.routeSubscription = route.params.subscribe(async (params) => {
      this.noteId = params.id;
      this.snapshotId = params.snapshotId;
      await this.loadContent();
    });
  }

  async loadContent() {
    await this.store.dispatch(new LoadSnapshotNote(this.snapshotId, this.noteId)).toPromise();
    this.isLoading = false;
    const isCanRead = this.store.selectSnapshot(NoteStore.snapshotState).canView;
    if (isCanRead) {
      this.contents = await this.apiHistory
        .getSnapshotContent(this.noteId, this.snapshotId)
        .toPromise();
    }
  }

  ngOnDestroy(): void {
    this.routeSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.store.dispatch(new UpdateRoute(EntityType.History));
  }
}
