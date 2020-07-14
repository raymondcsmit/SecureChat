import { Action } from "@ngrx/store";
import { ChatEntity } from "../entities/ChatEntity";

export enum ChatActionTypes {
    LoadChats = '[Chat] LoadChats',
    SelectChat = '[Chat] SelectChat',
    CreateChat = '[Chat] CreateChat'
}

export class LoadChats implements Action {
    readonly type = ChatActionTypes.LoadChats;
}

export class SelectChat implements Action {
    readonly type = ChatActionTypes.SelectChat;

    constructor(public payload: {id: string}) {}
}

export class CreateChat implements Action {
    readonly type = ChatActionTypes.CreateChat;

    constructor(public payload: {name: string, capacity: number}) {}
}

export type ChatActionsUnion =
    LoadChats
    | SelectChat
    | CreateChat;