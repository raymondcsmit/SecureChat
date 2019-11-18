import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { tap, exhaustMap, map, withLatestFrom, switchMap, filter, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { Store, select } from "@ngrx/store";
import { HttpClient } from "@angular/common/http";
import { apiConfig } from "../apiConfig";
import { LoadSelf, UserActionTypes, AddSelf, ConfirmEmail } from "../actions/user.actions";
import { State } from "../reducers/user.reducer";
import { UserService } from "../services/user.service";
import { AddEntity } from "../actions/entity.actions";
import { User } from "../models/User";
import { getSelf } from "../reducers";
import { Router } from "@angular/router";
import { Success, NoOp } from "src/app/core/actions/actions";

@Injectable()
export class UserEffects {
    
    @Effect()
    LoadSelf$ = this.actions$.pipe(
        ofType<LoadSelf>(UserActionTypes.LoadSelf),
        switchMap(action => this.userService.getSelf().pipe(
            map((user: User) => new AddSelf({user: user})),
            catchError(errors => {
                this.router.navigate(['/error'], {queryParams: {errors: JSON.stringify(errors)}});
                return of(new NoOp());
            }))
        )
    );

    @Effect()
    ConfirmEmail$ = this.actions$.pipe(
        ofType<ConfirmEmail>(UserActionTypes.ConfirmEmail),
        withLatestFrom(this.store.select(getSelf)),
        filter(([action, user]) => user != null),
        switchMap(([action, user]) => this.userService.confirmEmail(user.id).pipe(
            map(_ => new Success({action: action})),
            catchError(errors => {
                this.router.navigate(['/error'], {queryParams: {errors: JSON.stringify(errors)}});
                return of(new NoOp());
            })
        ))
    )

    constructor(
        private actions$: Actions,
        private store: Store<State>,
        private userService: UserService,
        private router: Router
    ) {
    }
}