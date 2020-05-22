import { Action } from "@ngrx/store";
import { UserEntity } from "../entities/UserEntity";

export enum UserActionTypes {
    LoadSelf = "[User] LoadSelf",
    AddSelf = "[User] AddSelf",
    ConfirmEmail = "[User] ConfirmEmail",
    UpdateUser = "[User] UpdateUser",
    UpdateUserStatus = "[User] UpdateUserStatus",
    LoadOnlineStatus = '[User] LoadOnlineStatus'
}

export class LoadSelf implements Action {
    readonly type = UserActionTypes.LoadSelf;
}

export class AddSelf implements Action {
    readonly type = UserActionTypes.AddSelf;
  
    constructor(public payload: { user: UserEntity }) {}
}

export class ConfirmEmail implements Action {
    readonly type = UserActionTypes.ConfirmEmail;
}

export class UpdateUser implements Action {
    readonly type = UserActionTypes.UpdateUser;

    constructor(public payload: {id: string, user: Partial<UserEntity>}) {}
}

export class UpdateUserStatus implements Action {
    readonly type = UserActionTypes.UpdateUserStatus;

    constructor(public payload: {id: string, status: "online"|"offline"|"idle"}) {}
}

export class LoadOnlineStatus implements Action {
    readonly type = UserActionTypes.LoadOnlineStatus;
}

export type UserActionsUnion =
    LoadSelf
    | AddSelf
    | ConfirmEmail
    | UpdateUser
    | UpdateUserStatus
    | LoadOnlineStatus;