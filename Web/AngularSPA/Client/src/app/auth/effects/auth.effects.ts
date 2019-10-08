import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { Router } from "@angular/router";
import * as fromActions from "../actions/auth.actions";
import * as fromAuth from '../reducers';
import { exhaustMap, map, tap, withLatestFrom } from 'rxjs/operators';
import { from } from 'rxjs';
import { UserManager } from "oidc-client";
import { Store } from "@ngrx/store";
import { AddEntity } from "src/app/chat/actions/entity.actions";
import { User } from "../../chat/models/User";

@Injectable()
export class AuthEffects {

    @Effect({dispatch: false})
    signIn$ = this.actions$.pipe(
        ofType<fromActions.SignIn>(fromActions.AuthActionTypes.SignIn),
        tap(() => this.userManager.signinRedirect())
    );

    @Effect({dispatch: false})
    signInSuccess$ = this.actions$.pipe(
        ofType<fromActions.SignInSuccess>(fromActions.AuthActionTypes.SignInSuccess),
        tap(() => this.router.navigate(['chat'])),
        tap(() => this.userManager.events.addUserSignedOut(() => 
            this.store.dispatch(new fromActions.SignOutSuccess({ fromCallback: false })
        )))
    );

    @Effect()
    completeSignIn$ = this.actions$.pipe(
        ofType<fromActions.CompleteSignIn>(fromActions.AuthActionTypes.CompleteSignIn),
        exhaustMap(() => from(this.userManager.signinRedirectCallback())),
        map(user => new fromActions.SignInSuccess({user: user}))
    );

    @Effect({dispatch: false})
    signOut$ = this.actions$.pipe(
        ofType<fromActions.SignOut>(fromActions.AuthActionTypes.SignOut),
        tap(() => this.userManager.signoutRedirect())
    );

    @Effect()
    completeSignOut$ = this.actions$.pipe(
        ofType<fromActions.CompleteSignOut>(fromActions.AuthActionTypes.CompleteSignOut),
        exhaustMap(() => from(this.userManager.signoutRedirectCallback())),
        map(() => new fromActions.SignOutSuccess({ fromCallback: true }))
    );

    @Effect({dispatch: false})
    signOutSuccess$ = this.actions$.pipe(
        ofType<fromActions.SignOutSuccess>(fromActions.AuthActionTypes.SignOutSuccess),
        tap(async action => {
            if (!action.payload.fromCallback) {
                await this.userManager.removeUser();
            }
            this.router.navigate(['auth', 'login'])
        })
    )

    @Effect()
    authenticationFailure = this.actions$.pipe(
        ofType<fromActions.SignInFailure>(fromActions.AuthActionTypes.SignInFailure),
        tap(action => {
            this.router.navigate(['auth', 'login']);
        })
    );

    constructor(
        private actions$: Actions,
        private userManager: UserManager,
        private router: Router,
        private store: Store<fromAuth.State>,
    ) {
        this.initUserIfLoggedIn();
    }

    initUserIfLoggedIn() {
        this.userManager.getUser()
            .then(user => {
                if (user && !user.expired) {
                    this.store.dispatch(new fromActions.SignInSuccess({user: user}));
                }
            })
    }
}