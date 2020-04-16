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
}

const userAdapter: EntityAdapter<UserEntity> = createEntityAdapter<UserEntity>();

const initialState: State = userAdapter.getInitialState({
    selfId: null
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

        default: {
          return state;
        }
    }
}

export const entitySelectors = userAdapter.getSelectors();

export const selectUserById = (id: string) => (state: State) => entitySelectors.selectEntities(state)[id];
export const selectUsersById = (ids: string[]) => (state: State) => entitySelectors.selectAll(state).filter(user => ids.includes(user.id));
export const selectSelf = (state: State) => selectUserById(state.selfId)(state);