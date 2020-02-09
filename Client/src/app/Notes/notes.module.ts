import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AllNotesComponent } from './all-notes/all-notes.component';
import { SharedNotesComponent } from './shared-notes/shared-notes.component';
import { LockedNotesComponent } from './locked-notes/locked-notes.component';
import { NoteComponent } from './note/note.component';
import { NotesContainerComponent } from './notes-container/notes-container.component';
import { AppRoutingModule } from '..//app-routing.module';
import { FullNoteComponent } from './full-note/full-note.component';
import { MaterialModule } from '../material.module';
import { ContentMenuComponent } from '../NoteMenus/content-menu/content-menu.component';
import { FontContentMenuComponent } from '../NoteMenus/font-content-menu/font-content-menu.component';

@NgModule({
  declarations: [
    AllNotesComponent,
    SharedNotesComponent,
    LockedNotesComponent,
    NoteComponent,
    NotesContainerComponent,
    FullNoteComponent,
    ContentMenuComponent,
    FontContentMenuComponent,
  ],
  imports: [
    CommonModule,
    AppRoutingModule,
    MaterialModule,
  ],
  exports: []
})
export class NotesModule { }
