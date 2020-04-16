import { Action } from "@ngrx/store";
import { User } from "../models/User";
import { UserEntity } from "../entities/UserEntity";

export enum FriendshipActionTypes {
    RemoveFriend = '[Friendship] RemoveFriend'
}

export class RemoveFriend implements Action {
    readonly type = FriendshipActionTypes.RemoveFriend;
  
    constructor(public payload: { id: string }) {}
}

export type FriendshipActionsUnion =
    RemoveFriend;