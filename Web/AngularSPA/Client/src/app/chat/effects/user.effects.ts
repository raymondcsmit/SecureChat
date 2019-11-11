import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { tap, exhaustMap, map, withLatestFrom, switchMap } from 'rxjs/operators';
import { } from 'rxjs';
import { Store } from "@ngrx/store";
import { HttpClient } from "@angular/common/http";
import { apiConfig } from "../apiConfig";
import { LoadSelf, UserActionTypes } from "../actions/user.actions";
import { State } from "../reducers/user.reducer";
import { UserService } from "../services/user.service";
import { AddEntity } from "../actions/entity.actions";
import { User } from "../models/User";

@Injectable()
export class UserEffects {
    
    @Effect()
    LoadSelf$ = this.actions$.pipe(
        ofType<LoadSelf>(UserActionTypes.LoadSelf),
        switchMap(action => this.userService.getUser(action.payload.id)),
        map((user: User) => new AddEntity(user.constructor.name, {entity: user}))
    );

    constructor(
        private actions$: Actions,
        private httpClient: HttpClient,
        private store: Store<State>,
        private userService: UserService
    ) {
    }
}