import { EntityActionsUnion, isEntityAction } from "../../core/actions/entity.actions";
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { entityReducer } from "src/app/core/reducers";
import { FriendshipEntity } from "../entities/FriendshipEntity";
import { FriendshipActionsUnion } from "../actions/friendship.actions";

export type State = EntityState<FriendshipEntity>;

const friendshipAdapter: EntityAdapter<FriendshipEntity> = createEntityAdapter<FriendshipEntity>();

const initialState: State = friendshipAdapter.getInitialState();

export function reducer(state = initialState, action: FriendshipActionsUnion | EntityActionsUnion<FriendshipEntity>): State {
    
    if (isEntityAction(action) && action.entityType === FriendshipEntity.name) {
        return entityReducer(action, friendshipAdapter, state);
    }

    return state;
}

export const entitySelectors = friendshipAdapter.getSelectors();