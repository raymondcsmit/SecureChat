import { EntityActionsUnion, isEntityAction } from "../../core/actions/entity.actions";
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { entityReducer } from "src/app/core/reducers";
import { FriendshipRequestEntity } from "../entities/FriendshipRequestEntity";
import { FriendshipRequestActionsUnion } from "../actions/friendship-request.actions";

export type State = EntityState<FriendshipRequestEntity>;

const friendshipRequestAdapter: EntityAdapter<FriendshipRequestEntity> = createEntityAdapter<FriendshipRequestEntity>();

const initialState: State = friendshipRequestAdapter.getInitialState();

export function reducer(state = initialState, action: FriendshipRequestActionsUnion | EntityActionsUnion<FriendshipRequestEntity>): State {
    
    if (isEntityAction(action) && action.entityType === FriendshipRequestEntity.name) {
        return entityReducer(action, friendshipRequestAdapter, state);
    }

    return state;
}

export const entitySelectors = friendshipRequestAdapter.getSelectors();