import { ChatActionsUnion, ChatActionTypes } from "../actions/chat.actions";
import { EntityActionsUnion, EntityActionTypes, isEntityAction } from "../../core/actions/entity.actions";
import { Chat } from "../models/Chat";
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import * as _ from "lodash";
import { Chatroom } from "../models/Chatroom";
import { MessageActionsUnion, MessageActionTypes } from "../actions/message.actions";

export interface State extends EntityState<Chat> {
    selectedChatroomId: string;
    selectedPrivateChatId: string;
}

export const adapter: EntityAdapter<Chat> = createEntityAdapter<Chat>();

const initialState = adapter.getInitialState({
    selectedChatroomId: null,
    selectedPrivateChatId: null
});

export function reducer(state = initialState, action: ChatActionsUnion | EntityActionsUnion<Chat> | MessageActionsUnion): State {
    if (isEntityAction(action) && action.entityType !== Chat.name) {
        return state;
    }
    
    switch (action.type) {
        case EntityActionTypes.LoadEntities: {
            return adapter.addAll(action.payload.entities, state);
        }

        case EntityActionTypes.UpdateEntity: {
            return adapter.updateOne({
                id: action.payload.entity.id,
                changes: action.payload.entity
            }, state);
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

        case ChatActionTypes.SelectChat: {
            let newState = adapter.updateOne({
                id: action.payload.id,
                changes: {
                    newMessageCount: 0
                }
            }, state);
            newState.selectedChatroomId = action.payload.id;

            return newState;
        }

        case ChatActionTypes.SelectChatroom: {
            let newState = adapter.updateOne({
                id: action.payload.id,
                changes: {
                    newMessageCount: 0
                }
            }, state);
            newState.selectedPrivateChatId = action.payload.id;

            return newState;
        }

        case MessageActionTypes.MessageReceived: {
            if (state.selectedChatroomId === action.payload.message.chatId || state.selectedPrivateChatId === action.payload.message.chatId) {
                return state;
            }
            let newState = adapter.updateOne({
                id: action.payload.message.chatId,
                changes: {
                    newMessageCount: state.entities[action.payload.message.chatId].newMessageCount + 1
                }
            }, state);
        }

        default: {
          return state;
        }
      }
}

const {
    selectAll,
    selectTotal,
    selectEntities
  } = adapter.getSelectors();
export const selectChats = selectAll;
export const selectChatCount = selectTotal;
export const selectChatrooms = (state: State) => selectChats(state).filter(chat => chat instanceof Chatroom)
                                                                   .map(chat => chat as Chatroom);
export const selectPrivateChats = (state: State) => selectChats(state).filter(chat => !(chat instanceof Chatroom));
export const selectChatroomTotal = (state: State) => selectChatrooms(state).length;
export const selectPrivateChatTotal = (state: State) => selectPrivateChats(state).length;
export const selectChatById = (id: string) => (state: State) => selectEntities(state)[id];
export const selectNewMessageCountById = (id: string) => (state: State) => selectChatById(id)(state).newMessageCount;
export const selectChatroomsByOwnerId = (id: string) => (state: State) => selectChatrooms(state).filter(chat => chat instanceof Chatroom && chat.ownerId === id)
                                                                                                .map(chat => chat as Chatroom);
export const selectSelectedChatroomId = (state: State) => state.selectedChatroomId;
export const selectSelectedPrivateChatId = (state: State) => state.selectedPrivateChatId;