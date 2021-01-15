import { Injectable } from "@angular/core";
import { combineLatest, of } from 'rxjs';
import { Actions, Effect, ofType } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Router } from "@angular/router";
import { State } from "../reducers/chat.reducer";
import { ChatsService } from "../services/chats.service";
import { LoadChats, ChatActionTypes, CreateChat as CreateChat } from "../actions/chat.actions";
import { getSelf } from "src/app/user/reducers";
import { filter, mergeMap, catchError, switchMap, throttleTime, withLatestFrom } from "rxjs/operators";
import { AddEntities, UpsertEntities, UpsertEntity } from "src/app/core/actions/entity.actions";
import { ChatEntity } from "../entities/ChatEntity";
import { UserEntity } from "src/app/user/entities/UserEntity";
import { Success, Failure } from "src/app/core/actions/core.actions";

@Injectable()
export class ChatEffects {

    @Effect()
    LoadChats$ = this.actions$.pipe(
        ofType<LoadChats>(ChatActionTypes.LoadChats),
        switchMap((action => this.chatsService.getChats().pipe(
            mergeMap(res => [ 
                new AddEntities(ChatEntity.name, {entities: res.chats}),
                new UpsertEntities(UserEntity.name, {entities: res.users}),
                new Success({action: action})
            ]),
            catchError(errors => of(new Failure({action: action, errors: errors})))
        ))
    ));

    @Effect()
    CreateChat$ = this.actions$.pipe(
        ofType<CreateChat>(ChatActionTypes.CreateChat),
        throttleTime(5000),
        withLatestFrom(this.store.select(getSelf)),
        switchMap(([action, self]) => this.chatsService.createChat(self.id, action.payload.name, action.payload.capacity).pipe(
            mergeMap(res => [ 
                new AddEntities(ChatEntity.name, {entities: res.chats}),
                new UpsertEntities(UserEntity.name, {entities: res.users}),
                new Success({action: action})
            ]),
            catchError(errors => of(new Failure({action: action, errors: errors})))
        ))
    );

    constructor(
        private actions$: Actions,
        private store: Store<State>,
        private chatsService: ChatsService,
        private router: Router
    ) {
    }
}