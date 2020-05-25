import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import { map, withLatestFrom, switchMap, filter, catchError, throttleTime, mergeMap, take } from 'rxjs/operators';
import { of, combineLatest } from 'rxjs';
import { Store, select, Action } from "@ngrx/store";
import { LoadSelf, UserActionTypes, AddSelf, ConfirmEmail, UpdateUser, LoadOnlineStatus, UpdateUserStatus } from "../actions/user.actions";
import { State } from "../reducers/user.reducer";
import { AccountService } from "../services/account.service";
import { AddEntity, UpsertEntity, AddEntities, UpsertEntities } from "../../core/actions/entity.actions";
import { Router } from "@angular/router";
import { Success, NoOp, Failure } from "src/app/core/actions/core.actions";
import * as jsonpatch from 'fast-json-patch';
import { getSelf, getUserById } from "src/app/user/reducers";
import { UsersService } from "../services/users.service";
import { FriendshipRequestEntity } from "../entities/FriendshipRequestEntity";
import { UserEntity } from "../entities/UserEntity";
import { FriendshipRequestActionTypes, AddFriend, LoadFriendshipRequests, UpdateFriendshipRequest } from "../actions/friendship-request.actions";
import { LoadFriendships, FriendshipActionTypes } from "../actions/friendship.actions";
import { FriendshipEntity } from "../entities/FriendshipEntity";
import { SessionService } from "../services/session.service";
import { appConfig } from "src/app.config";

@Injectable()
export class UserEffects {
    
    @Effect()
    LoadSelf$ = this.actions$.pipe(
        ofType<LoadSelf>(UserActionTypes.LoadSelf),
        switchMap(() => this.accountService.getSelf().pipe(map((user: UserEntity) => new AddSelf({ user: user })), catchError(errors => {
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
        ofType<AddFriend>(FriendshipRequestActionTypes.AddFriend),
        throttleTime(5000),
        withLatestFrom(this.store.select(getSelf)),
        switchMap(([action, self]) => this.userService.addFriend(self.id, action.payload.user.id).pipe(
            mergeMap(res => [ 
                new AddEntity(FriendshipRequestEntity.name, {entity: res.friendshipRequest}),
                new UpsertEntity(UserEntity.name, {entity: res.requestee}),
                new Success({action: action})
            ]),
            catchError(errors => of(new Failure({action: action, errors: errors})))
        ))
    );

    @Effect()
    LoadFriendshipRequests$ = combineLatest(
        this.actions$.pipe(ofType<LoadFriendshipRequests>(FriendshipRequestActionTypes.LoadFriendshipRequests)),
        this.store.select(getSelf)).pipe(
            filter(([, self]) => self != null),
            mergeMap(([action, self]) => this.userService.getFriendshipRequests(self.id).pipe(
                mergeMap(res => [ 
                    new AddEntities(FriendshipRequestEntity.name, {entities: res.friendshipRequests}),
                    new UpsertEntities(UserEntity.name, {entities: res.requesters}),
                    new Success({action: action})
                ]),
                catchError(errors => of(new Failure({action: action, errors: errors})))
            ))
        );

    @Effect()
    UpdateFriendshipRequest$ = this.actions$.pipe(
        ofType<UpdateFriendshipRequest>(FriendshipRequestActionTypes.UpdateFriendshipRequest),
        throttleTime(5000),
        switchMap(action => this.userService.updateFriendshipRequest(action.payload.id, action.payload.status).pipe(
            map(_ => new Success({action: action})),
            catchError(errors => of(new Failure({action: action, errors: errors})))
        ))
    );

    @Effect()
    LoadFriendships$ = combineLatest(
        this.actions$.pipe(ofType<LoadFriendships>(FriendshipActionTypes.LoadFriendships)),
        this.store.select(getSelf)).pipe(
            filter(([, self]) => self != null),
            mergeMap(([action, self]) => this.userService.getFriendships(self.id).pipe(
                mergeMap(res => [ 
                    new AddEntities(FriendshipEntity.name, {entities: res.friendships}),
                    new UpsertEntities(UserEntity.name, {entities: res.friends}),
                    new Success({action: action})
                ]),
                catchError(errors => of(new Failure({action: action, errors: errors})))
            ))
        );
    
    @Effect()
    LoadOnlineStatus$ = combineLatest(
        this.actions$.pipe(ofType<LoadOnlineStatus>(UserActionTypes.LoadOnlineStatus)),
        this.store.select(getSelf)).pipe(
            filter(([, self]) => self != null),
            mergeMap(([action,]) => this.sessionService.getFriendSessions().pipe(
                mergeMap(res => Object.entries(res)
                                    .filter(([id, session]) => session.startTime > session.endTime)
                                    .map(([id, session]) => {
                                        const status = session.idleTime > appConfig.idleSeconds ? 'idle' : 'online';
                                        return new UpdateUserStatus({id: id, status: 'online'}) as Action;
                                    })
                                    .concat([new Success({action: action})])),
                catchError(errors => of(new Failure({action: action, errors: errors})))
            )));

    constructor(
        private actions$: Actions,
        private store: Store<State>,
        private accountService: AccountService,
        private userService: UsersService,
        private sessionService: SessionService,
        private router: Router
    ) {
    }
}