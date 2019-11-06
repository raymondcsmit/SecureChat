import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Store, select } from '@ngrx/store';
import * as fromAuth from '../reducers';
import * as AuthActions from '../actions/auth.actions';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ChatGuardService implements CanActivate {

  constructor(private store: Store<fromAuth.State>, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.store.pipe(
      select(fromAuth.getSignedIn),
      tap(signedIn => {
        if (!signedIn) this.router.navigate(['auth', 'login'])
      })
    );
  }
}
