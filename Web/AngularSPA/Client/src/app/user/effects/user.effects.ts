import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { map, withLatestFrom, switchMap, filter, catchError, throttleTime, mergeMap, take } from 'rxjs/operators';
import { of } from 'rxjs';
import { Store, select } from "@ngrx/store";
import { LoadSelf, UserActionTypes, AddSelf, ConfirmEmail, UpdateUser, AddFriend } from "../actions/user.actions";
import { State } from "../reducers/user.reducer";
import { AccountService } from "../services/account.service";
import { AddEntity, UpsertEntities, UpsertEntity } from "../../core/actions/entity.actions";
import { User } from "../models/User";
import { Router } from "@angular/router";
import { Success, NoOp, Failure } from "src/app/core/actions/core.actions";
import * as jsonpatch from 'fast-json-patch';
import { getSelf, getUserById } from "src/app/user/reducers";
import { UsersService } from "../services/users.service";
import { FriendshipRequest } from "../models/FriendshipRequest";

@Injectable()
export class UserEffects {
    
    @Effect()
    LoadSelf$ = this.actions$.pipe(
        ofType<LoadSelf>(UserActionTypes.LoadSelf),
        switchMap(() => this.accountService.getSelf().pipe(map((user: User) => new AddSelf({ user: user })), catchError(errors => {
            this.router.navigate(['/error'], { queryParams: { errors: JSON.stringify(errors) } });
            return of(new NoOp());
        }))
        )
    );

    @Effect()
    ConfirmEmail$ = this.actions$.pipe(
        ofType<ConfirmEmail>(UserActionTypes.ConfirmEmail),
        throttleTime(5000),
        withLatestFrom(this.store.select(getSelf)),
        filter(([, user]) => user != null),
        switchMap(([action, user]) => this.accountService.confirmEmail(user.id).pipe(
            map(_ => new Success({action: action})),
            catchError(errors => of(new Failure({action: action, errors: errors})))
        ))
    );

    @Effect()
    UpdateUser$ = this.actions$.pipe(
        ofType<UpdateUser>(UserActionTypes.UpdateUser),
        throttleTime(5000),
        mergeMap(action => this.store.pipe(
            select(getUserById(action.payload.id)),
            take(1),
            map(user => {
                let u = {...user};
                var observer = jsonpatch.observe(u);
                Object.assign(u, action.payload.user);
                var patch = jsonpatch.generate(observer);
                return [action, patch];
            })
        )),
        switchMap(([action, patch]: [UpdateUser, any]) => this.userService.updateUser(action.payload.id, patch).pipe(
            map(_ => new Success({action: action})),
            catchError(errors => of(new Failure({action: action, errors: errors})))
        ))
    );

    @Effect()
    AddFriend$ = this.actions$.pipe(
        ofType<AddFriend>(UserActionTypes.AddFriend),
        throttleTime(5000),
        withLatestFrom(this.store.select(getSelf)),
        switchMap(([action, self]) => this.userService.addFriend(self.id, action.payload.user.id).pipe(
            mergeMap(res => [ 
                new AddEntity(FriendshipRequest.name, {entity: res.friendshipRequest}),
                new UpsertEntity(User.name, {entity: res.requestee}),
                new Success({action: action})
            ]),
            catchError(errors => of(new Failure({action: action, errors: errors})))
        ))
    );

    constructor(
        private actions$: Actions,
        private store: Store<State>,
        private accountService: AccountService,
        private userService: UsersService,
        private router: Router
    ) {
    }
}