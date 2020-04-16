import { Action } from "@ngrx/store";
import { UserEntity } from "../entities/UserEntity";

export enum UserActionTypes {
    LoadSelf = "[User] LoadSelf",
    AddSelf = "[User] AddSelf",
    ConfirmEmail = "[User] ConfirmEmail",
    UpdateUser = "[User] UpdateUser"
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

export type UserActionsUnion =
    LoadSelf
    | AddSelf
    | ConfirmEmail
    | UpdateUser;