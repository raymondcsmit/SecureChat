import { ChatActionsUnion, ChatActionTypes } from "../actions/chat.actions";
import { EntityActionsUnion, isEntityAction } from "../../core/actions/entity.actions";
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import * as _ from "lodash";
import { MessageActionsUnion, MessageActionTypes } from "../actions/message.actions";
import { entityReducer } from "src/app/core/reducers";
import { ChatEntity } from "../entities/ChatEntity";

export class SelectedChatDescriptor {
    id: string;
    time: Date;

    static default: SelectedChatDescriptor = {
        id: "sd892jksjds98",
        time: new Date()
    }
}

export interface State extends EntityState<ChatEntity> {
    selectedChatroom: SelectedChatDescriptor;
    selectedPrivateChat: SelectedChatDescriptor;
}

export const chatAdapter: EntityAdapter<ChatEntity> = createEntityAdapter<ChatEntity>();

const initialState: State = chatAdapter.getInitialState({
    selectedChatroom: SelectedChatDescriptor.default,
    selectedPrivateChat: SelectedChatDescriptor.default,
    newMessageCounts: {}
});

export function reducer(state: State = initialState, action: ChatActionsUnion | EntityActionsUnion<ChatEntity> | MessageActionsUnion): State {
    if (isEntityAction(action) && action.entityType === ChatEntity.name) {
        return entityReducer(action, chatAdapter, state);
    }
    
    switch (action.type) {

        default: {
          return state;
        }
      }
}

export const chatEntitySelectors = chatAdapter.getSelectors();

export const selectChatrooms = (state: State) => chatEntitySelectors.selectAll(state).filter(chat => chat.isChatroom());
export const selectPrivateChats = (state: State) => chatEntitySelectors.selectAll(state).filter(chat => !chat.isChatroom());
export const selectChatById = (id: string) => (state: State) => chatEntitySelectors.selectEntities(state)[id];
