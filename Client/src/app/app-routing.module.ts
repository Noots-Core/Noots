import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { ContentActiveteGuard } from './core/guards/content-activete.guard';
import { ContentComponent } from './content/content/content.component';


const routes: Routes = [
  {
    path: '',
    component: ContentComponent,
    canActivate: [ContentActiveteGuard],
    children: [
      {
          path: 'notes',
          loadChildren: () => import('./content/notes/notes.module').then(m => m.NotesModule),
      },
      {
          path: 'folders',
          loadChildren: () => import('./content/folders/folders.module').then(m => m.FoldersModule),
      },
      {
          path: 'people',
          loadChildren: () => import('./content/people/people.module').then(m => m.PeopleModule),
      },
      {
          path: 'labels',
          loadChildren: () => import('./content/labels/labels.module').then(m => m.LabelsModule),
      },
      {
          path: 'profile',
          loadChildren: () => import('./content/profile/profile.module').then(m => m.ProfileModule),
      },
      {
        path: '',
        redirectTo: '/notes',
        pathMatch: 'full'
      },
  ],
  },
  {
    path: 'about',
    loadChildren: () => import('./about/about.module').then(m => m.AboutModule),
  },
  {
    path: 'note/:id',
    loadChildren: () => import('./read/read.module').then(m => m.ReadModule),
  },
  {
    path: '**',
    loadChildren: () => import('./error-four/error-four.module').then(m => m.ErrorFourModule)
  },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
