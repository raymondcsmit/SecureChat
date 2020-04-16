import { Action } from "@ngrx/store";
import { UserEntity } from "../entities/UserEntity";
import { FriendshipRequestStatus } from "../models/FriendshipRequest";

export enum FriendshipRequestActionTypes {
    AddFriend = '[FriendshipRequest] AddFriend',
    LoadFriendshipRequests = '[FriendshipRequest] Load',
    UpdateFriendshipRequest = '[FriendshipRequest] Update',
}

export class AddFriend implements Action {
    readonly type = FriendshipRequestActionTypes.AddFriend;
  
    constructor(public payload: { user: UserEntity }) {}
}

export class LoadFriendshipRequests implements Action {
    readonly type = FriendshipRequestActionTypes.LoadFriendshipRequests;
}

export class UpdateFriendshipRequest implements Action {
    readonly type = FriendshipRequestActionTypes.UpdateFriendshipRequest;

    constructor(public payload: {id: string, status: FriendshipRequestStatus}) {}
}

export type FriendshipRequestActionsUnion =
    AddFriend
    | LoadFriendshipRequests
    | UpdateFriendshipRequest;