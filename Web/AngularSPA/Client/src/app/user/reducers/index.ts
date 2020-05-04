import * as fromUsers from './user.reducer';
import * as fromFriendships from './friendship.reducer';
import * as fromFriendshipRequests from './friendship-request.reducer';
import * as fromRoot from '../../core/reducers';
import { ActionReducerMap, createFeatureSelector, createSelector } from '@ngrx/store';

export interface UserModuleState {
    users: fromUsers.State,
    friendships: fromFriendships.State,
    friendshipRequests: fromFriendshipRequests.State
}

export interface State extends fromRoot.State {
    userModule: UserModuleState
}

export const reducers: ActionReducerMap<UserModuleState> = {
    users: fromUsers.reducer,
    friendships: fromFriendships.reducer,
    friendshipRequests: fromFriendshipRequests.reducer
};

export const getUserModuleState = createFeatureSelector<UserModuleState>('userModule');

// users

export const getUserState = createSelector(
    getUserModuleState,
    (state: UserModuleState) => state.users
);

export const getUserIds = createSelector(getUserState, fromUsers.entitySelectors.selectIds);
export const getAllUsers = createSelector(getUserState, fromUsers.entitySelectors.selectAll);
export const getUserTotal = createSelector(getUserState, fromUsers.entitySelectors.selectTotal);
export const getUserById = (id: string) => createSelector(getUserState, fromUsers.selectUserById(id));
export const getUsersById = (ids: string[]) => createSelector(getUserState, fromUsers.selectUsersById(ids));
export const getSelf = createSelector(getUserState, fromUsers.selectSelf);
export const getUserStatusById = (id: string) => createSelector(getUserState, fromUsers.selectUserStatusById(id));
export const getUserStatus = createSelector(getUserState, fromUsers.selectUserStatus);

// friendships

export const getFriendshipState = createSelector(
    getUserModuleState,
    (state: UserModuleState) => state.friendships
);

export const getFriendIds = createSelector(getFriendshipState, getSelf, (friendshipState, self) => fromFriendships.entitySelectors.selectAll(friendshipState)
                                                                                            .map(friendship => friendship.user1 === self.id ? friendship.user2 : friendship.user1));
export const getFriends = createSelector(getUserState, getFriendIds, (userState, friendIds) => fromUsers.selectUsersById(friendIds)(userState));

//friendship requests

export const getFriendshipRequestState = createSelector(
    getUserModuleState,
    (state: UserModuleState) => state.friendshipRequests
);

export const getFriendshipRequests = createSelector(getFriendshipRequestState, fromFriendshipRequests.entitySelectors.selectAll);