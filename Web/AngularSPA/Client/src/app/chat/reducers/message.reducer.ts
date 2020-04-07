import { ChatActionsUnion, ChatActionTypes } from "../actions/chat.actions";
import { EntityActionsUnion, EntityActionTypes, isEntityAction } from "../../core/actions/entity.actions";
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Message } from "../models/Message";
import { MessageActionsUnion, MessageActionTypes } from "../actions/message.actions";

export interface State extends EntityState<Message> {
    
}

export const adapter: EntityAdapter<Message> = createEntityAdapter<Message>();

const initialState: State = adapter.getInitialState({
    
});

export function reducer(state = initialState, action: MessageActionsUnion | EntityActionsUnion<Message>): State {
    if (isEntityAction(action) && action.entityType !== Message.name) {
        return state;
    }
    
    switch (action.type) {
        case EntityActionTypes.LoadEntities: {
            return adapter.addAll(action.payload.entities, state);
        }

        case EntityActionTypes.AddEntity: {
            return adapter.addOne(action.payload.entity, state)
        }

        case MessageActionTypes.MessageReceived: {
            return adapter.upsertOne(action.payload.message, state);
        }

        default: {
          return state;
        }
      }
}

const {
    selectIds,
    selectAll
  } = adapter.getSelectors();
export const selectMessageIds = selectIds;
export const selectMessages = selectAll;
export const selectMessageById = (id: string) => (state: State) => selectMessages(state).filter(message => message.id === id)[0];
export const selectMessagesByChatId = (chatId: string) => (state: State) => selectMessages(state).filter(message => message.chatId === chatId)
