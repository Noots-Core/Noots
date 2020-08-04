import { Component, OnInit, OnDestroy } from '@angular/core';
import { Store } from '@ngxs/store';
import { SmallNote } from '../models/smallNote';
import { PersonalizationService } from 'src/app/shared/services/personalization.service';
import { LoadPrivateNotes, UnSelectAllNote, SetDeleteNotesClear } from '../state/notes-actions';
import { Order, OrderEntity } from 'src/app/shared/services/order.service';
import { take, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { UpdateColorNote } from '../state/updateColor';

@Component({
  selector: 'app-privates',
  templateUrl: './privates.component.html',
  styleUrls: ['./privates.component.scss']
})
export class PrivatesComponent implements OnInit, OnDestroy {

  destroy = new Subject<void>();

  public notes: SmallNote[];

  constructor(public pService: PersonalizationService,
              private store: Store
  ) { }


  ngOnDestroy(): void {
    this.destroy.next();
    this.destroy.complete();
    this.store.dispatch(new UnSelectAllNote());
  }

  async ngOnInit() {
    await this.store.dispatch(new LoadPrivateNotes()).toPromise();

    this.store.select(x => x.Notes.privateNotes).pipe(take(1))
      .subscribe(x => { this.notes = [...x].map(note => { note = { ...note }; return note; }); setTimeout(() => this.initMurri()); });

    this.store.select(x => x.Notes.updateColor)
      .pipe(takeUntil(this.destroy))
      .subscribe(x => this.changeColorHandler(x));

    this.store.select(x => x.Notes.setdeleteNotesEvent)
      .pipe(takeUntil(this.destroy))
      .subscribe(x => this.setDeleteNotes(x));
  }

  initMurri() {

    this.pService.gridSettings();

    this.pService.grid.on('dragEnd', async (item, event) => {
      console.log(item._element.id);
      const order: Order = {
        orderEntity: OrderEntity.Note,
        position: item.getGrid().getItems().indexOf(item) + 1,
        entityId: item._element.id
      };
    });
  }

  changeColorHandler(updateColor: UpdateColorNote[]) {
    for (const update of updateColor) {
      this.notes.find(x => x.id === update.id).color = update.color;
    }
  }

  setDeleteNotes(notes: SmallNote[]) {
    if (notes.length !== 0) {
      this.notes = this.notes.filter(x => this.customFilterPredicate(notes.map(z => z.id), x));
      setTimeout(() => this.pService.grid.refreshItems().layout(), 0);
      this.store.dispatch(new SetDeleteNotesClear());
    }
  }

  customFilterPredicate(ids: string[], note: SmallNote) {
    for (const id of ids) {
      if (id === note.id) {
        return false;
      }
    }
    return true;
  }
}
