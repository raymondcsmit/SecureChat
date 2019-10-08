import { Action } from "@ngrx/store";
import { Message } from "../models/Message";

export enum MessageActionTypes {
    SendMessage = '[Chat] SendMessage',
    MessageReceived = '[Chat] MessageReceived',
}

export class SendMessage implements Action {
    readonly type = MessageActionTypes.SendMessage;
  
    constructor(public payload: { chatId: string, content: string }) {}
}

export class MessageReceived implements Action {
    readonly type = MessageActionTypes.MessageReceived;
  
    constructor(public payload: { message: Message }) {}
}

export type MessageActionsUnion =
    SendMessage
    | MessageReceived;