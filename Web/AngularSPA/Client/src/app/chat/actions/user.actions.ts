import { Action } from "@ngrx/store";
import { Predicate } from "@angular/core";

export enum UserActionTypes {
    FriendAdded = '[User] FriendAdded',
    FriendRemoved = '[User] FriendRemoved',
    AddFriend = '[User] AddFriend',
    RemoveFriend = '[User] RemoveFriend',
    ExecuteUserQuery = '[User] ExecuteUserQuery',
    UserQueryExecuted = '[User] UserQueryExecuted',
    LoadSelf = "[User] LoadSelf"
}

export class LoadSelf implements Action {
    readonly type = UserActionTypes.LoadSelf;
}

export class AddFriend implements Action {
    readonly type = UserActionTypes.AddFriend;
  
    constructor(public payload: { id: string }) {}
}

export class RemoveFriend implements Action {
    readonly type = UserActionTypes.RemoveFriend;
  
    constructor(public payload: { id: string }) {}
}

export type UserActionsUnion =
    AddFriend
    | RemoveFriend
    | LoadSelf;