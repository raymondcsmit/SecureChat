import { Action } from "@ngrx/store";
import { User } from "../models/User";
import { UserEntity } from "../entities/UserEntity";

export enum FriendshipActionTypes {
    RemoveFriend = '[Friendship] RemoveFriend',
    LoadFriendships = '[Friendship] LoadFriendships'
}

export class RemoveFriend implements Action {
    readonly type = FriendshipActionTypes.RemoveFriend;
  
    constructor(public payload: { id: string }) {}
}

export class LoadFriendships implements Action {
    readonly type = FriendshipActionTypes.LoadFriendships
}

export type FriendshipActionsUnion =
    RemoveFriend |
    LoadFriendships;