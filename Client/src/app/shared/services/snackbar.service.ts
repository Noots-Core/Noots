import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarHorizontalPosition } from '@angular/material/snack-bar';
import { Store } from '@ngxs/store';
import { UserStore } from 'src/app/core/stateUser/user-state';
import { ThemeENUM } from 'src/app/shared/enums/ThemeEnum';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  constructor(private snackbar: MatSnackBar, private store: Store) {}

  openSnackBar(
    message: string,
    action?: string,
    horizontalPosition: MatSnackBarHorizontalPosition = 'end',
  ) {
    const theme = this.store.selectSnapshot(UserStore.getUserTheme);

    return this.snackbar.open(message, action, {
      duration: 5000,
      panelClass: theme === ThemeENUM.Light ? 'snackbar-light' : 'snackbar-dark',
      horizontalPosition,
      verticalPosition: 'bottom',
    });
  }
}
