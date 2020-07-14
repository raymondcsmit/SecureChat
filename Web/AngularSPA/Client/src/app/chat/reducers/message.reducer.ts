import { EntityActionsUnion, EntityActionTypes, isEntityAction } from "../../core/actions/entity.actions";
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { MessageActionsUnion, MessageActionTypes } from "../actions/message.actions";
import { MessageEntity } from "../entities/MessageEntity";
import { entityReducer } from "src/app/core/reducers";

export interface State extends EntityState<MessageEntity> {
    
}

export const messageAdapter: EntityAdapter<MessageEntity> = createEntityAdapter<MessageEntity>();

const initialState: State = messageAdapter.getInitialState({
    
});

export function reducer(state = initialState, action: MessageActionsUnion | EntityActionsUnion<MessageEntity>): State {
    if (isEntityAction(action) && action.entityType === MessageEntity.name) {
        return entityReducer(action, messageAdapter, state);
    }
    
    switch (action.type) {

        default: {
          return state;
        }
      }
}

export const messageEntitySelectors = messageAdapter.getSelectors();

export const selectMessageById = (id: string) => (state: State) => messageEntitySelectors.selectEntities(state)[id];
export const selectMessagesByChatId = (chatId: string) => (state: State) => messageEntitySelectors.selectAll(state).filter(message => message.chatId === chatId)
