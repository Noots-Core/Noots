import { Injectable, NgZone } from '@angular/core';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import * as firebase from 'firebase';
import { AngularFireAuth } from '@angular/fire/auth';
import { AuthAPIService } from './auth-api.service';

export interface Status {
  loggin: boolean;
}

@Injectable()
export class AuthService {

  constructor(
    private afAuth: AngularFireAuth,
    private ngZone: NgZone,
    private router: Router,
    private api: AuthAPIService) {
    this.afAuth.authState.subscribe((firebaseUser) => {
      this.configureAuthState(firebaseUser);
    });
  }

  GoogleAuth() {
    return this.AuthLogin(new firebase.auth.GoogleAuthProvider());
  }

  private configureAuthState(firebaseUser: firebase.User): void {
    if (firebaseUser) {
      firebaseUser.getIdToken(true).then((theToken) => {
        console.log('we have a token');
        this.setStatus(true);
        this.api.verifyToken(theToken).toPromise().catch(error => this.setStatus(false));
      }, (failReason) => {
        this.setStatus(false);
        this.logout();
      });
    } else {
      this.setStatus(false);
      this.logout();
    }
  }

  private setStatus(flag: boolean) {
    const item: Status = {
      loggin: flag
    };
    localStorage.setItem('login', JSON.stringify(item));
  }
  private AuthLogin(provider: firebase.auth.GoogleAuthProvider) {
    return this.afAuth.signInWithPopup(provider)
      .then(result => {})
      .catch(error => {
        window.alert(error);
      });
  }

  getToken() {
    return localStorage.getItem('jwt');
  }

  getStatus(): Status {
    const item = localStorage.getItem('login');
    return JSON.parse(item);
  }

  logout() {
    const item: Status = {
      loggin: false
    };
    localStorage.setItem('login', JSON.stringify(item));
    localStorage.removeItem('jwt');
    return this.afAuth.signOut();
  }
}
