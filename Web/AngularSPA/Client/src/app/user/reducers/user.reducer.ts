import { EntityActionsUnion, EntityActionTypes, isEntityAction } from "../../core/actions/entity.actions";
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { UserActionsUnion, UserActionTypes } from "../actions/user.actions";
import { User } from "../models/User";
import { entityReducer } from "src/app/core/reducers";
import { FriendshipRequestEntity } from "../entities/FriendshipRequestEntity";
import { FriendshipEntity } from "../entities/FriendshipEntity";
import { UserEntity } from "../entities/UserEntity";

export interface State extends EntityState<UserEntity> {
    selfId: string;
    userStatus: {
        [id: string]: "online"|"offline"|"idle";
    }
}

const userAdapter: EntityAdapter<UserEntity> = createEntityAdapter<UserEntity>();

const initialState: State = userAdapter.getInitialState({
    selfId: null,
    userStatus: {}
});

export function reducer(state = initialState, action: UserActionsUnion | EntityActionsUnion<UserEntity>): State {

    if (isEntityAction(action) && action.entityType === UserEntity.name) {
        return entityReducer(action, userAdapter, state);
    }
    
    switch (action.type) {
        case UserActionTypes.AddSelf: {
            let newUserState = userAdapter.upsertOne(action.payload.user, state);
            return {
                ...newUserState,
                selfId: action.payload.user.id
            }
        }

        case UserActionTypes.UpdateUserStatus: {
            let newState = {
                ...state,
                userStatus: {
                    ...state.userStatus,
                }};
            newState.userStatus[action.payload.id] = action.payload.status;
            return newState;
        }

        default: {
          return state;
        }
    }
}

export const entitySelectors = userAdapter.getSelectors();

export const selectUserById = (id: string) => (state: State) => entitySelectors.selectEntities(state)[id];
export const selectUsersById = (ids: string[]) => (state: State) => entitySelectors.selectAll(state).filter(user => ids.includes(user.id));
export const selectSelf = (state: State) => selectUserById(state.selfId)(state);
export const selectUserStatusById = (id: string) => (state: State) => state.userStatus[id];
export const selectUserStatus = (state: State) => state.userStatus;