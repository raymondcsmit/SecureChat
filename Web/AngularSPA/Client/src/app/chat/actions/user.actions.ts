import { Action } from "@ngrx/store";
import { User } from "../models/User";

export enum UserActionTypes {
    FriendAdded = '[User] FriendAdded',
    FriendRemoved = '[User] FriendRemoved',
    AddFriend = '[User] AddFriend',
    RemoveFriend = '[User] RemoveFriend',
    ExecuteUserQuery = '[User] ExecuteUserQuery',
    UserQueryExecuted = '[User] UserQueryExecuted',
    LoadSelf = "[User] LoadSelf",
    AddSelf = "[User] AddSelf",
    ConfirmEmail = "[User] ConfirmEmail"
}

export class LoadSelf implements Action {
    readonly type = UserActionTypes.LoadSelf;
}

export class AddSelf implements Action {
    readonly type = UserActionTypes.AddSelf;
  
    constructor(public payload: { user: User }) {}
}

export class AddFriend implements Action {
    readonly type = UserActionTypes.AddFriend;
  
    constructor(public payload: { id: string }) {}
}

export class RemoveFriend implements Action {
    readonly type = UserActionTypes.RemoveFriend;
  
    constructor(public payload: { id: string }) {}
}

export class ConfirmEmail implements Action {
    readonly type = UserActionTypes.ConfirmEmail;
}

export type UserActionsUnion =
    AddFriend
    | RemoveFriend
    | LoadSelf
    | AddSelf
    | ConfirmEmail;