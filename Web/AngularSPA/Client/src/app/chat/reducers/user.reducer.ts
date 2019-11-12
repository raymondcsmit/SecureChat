import { EntityActionsUnion, EntityActionTypes, isEntityAction } from "../actions/entity.actions";
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { UserActionsUnion, UserActionTypes } from "../actions/user.actions";
import { User } from "../models/User";

export interface State extends EntityState<User> {
    friendIds: string[];
    selfId: string;
}

export const adapter: EntityAdapter<User> = createEntityAdapter<User>();

const initialState: State = adapter.getInitialState({
    friendIds: [],
    selfId: null
});

export function reducer(state = initialState, action: UserActionsUnion | EntityActionsUnion<User>): State {
    if (isEntityAction(action) && action.entityType !== User.name) {
        return state;
    }
    
    switch (action.type) {
        case EntityActionTypes.AddEntity: {
            return adapter.addOne(action.payload.entity, state);
        }

        case EntityActionTypes.LoadEntities: {
            return adapter.addAll(action.payload.entities, state);
        }

        case EntityActionTypes.UpsertEntity: {
            return adapter.upsertOne(action.payload.entity, state);
        }

        case EntityActionTypes.UpsertEntities: {
            return adapter.upsertMany(action.payload.entities, state);
        }

        case EntityActionTypes.DeleteEntity: {
            return adapter.removeOne(action.payload.id, state);
        }

        case EntityActionTypes.DeleteEntities: {
            return adapter.removeMany(action.payload.ids, state);
        }

        case EntityActionTypes.DeleteEntitiesByPredicate: {
            return adapter.removeMany(action.payload.predicate, state);
        }

        default: {
          return state;
        }
      }
}

const {
    selectIds,
    selectAll,
    selectTotal,
    selectEntities
  } = adapter.getSelectors();
export const selectUserIds = selectIds;
export const selectUsers = selectAll;
export const selectUserCount = selectTotal;
export const selectFriendIds = (state: State) => state.friendIds;
export const selectFriends = (state: State) => selectUsers(state).filter(user => selectFriendIds(state).includes(user.id));
export const selectUserById = (id: string) => (state: State) => selectEntities(state)[id];
export const selectUsersById = (ids: string[]) => (state: State) => selectAll(state).filter(user => ids.includes(user.id));
export const selectSelf = (state: State) => selectUserById(state.selfId)(state);