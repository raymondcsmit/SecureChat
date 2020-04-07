import * as fromUsers from '../../user/reducers/user.reducer';
import * as fromRoot from '../../core/reducers';
import { ActionReducerMap, createFeatureSelector, createSelector } from '@ngrx/store';

export interface UserModuleState {
    users: fromUsers.State,
}

export interface State extends fromRoot.State {
    userModule: UserModuleState
}

export const reducers: ActionReducerMap<UserModuleState> = {
    users: fromUsers.reducer,
};

export const getUserModuleState = createFeatureSelector<UserModuleState>('userModule');

export const getUsersState = createSelector(
    getUserModuleState,
    (state: UserModuleState) => state.users
);

export const getUserIds = createSelector(getUsersState, fromUsers.selectUserIds);
export const getAllUsers = createSelector(getUsersState, fromUsers.selectUsers);
export const getUserTotal = createSelector(getUsersState, fromUsers.selectUserCount);
export const getUserById = (id: string) => createSelector(getUsersState, fromUsers.selectUserById(id));
export const getUsersById = (ids: string[]) => createSelector(getUsersState, fromUsers.selectUsersById(ids));
export const getFriendIds = createSelector(getUsersState, fromUsers.selectFriendIds);
export const getFriends = createSelector(getUsersState, fromUsers.selectFriends);
export const getSelf = createSelector(getUsersState, fromUsers.selectSelf);
