import { Action } from "@ngrx/store";
import { MessageEntity } from "../entities/MessageEntity";

export enum MessageActionTypes {
    SendMessage = '[Chat] SendMessage',
}

export class SendMessage implements Action {
    readonly type = MessageActionTypes.SendMessage;
  
    constructor(public payload: { chatId: string, content: string }) {}
}

export type MessageActionsUnion =
    SendMessage;