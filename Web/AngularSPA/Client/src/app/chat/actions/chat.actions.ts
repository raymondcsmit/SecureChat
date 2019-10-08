import { Action } from "@ngrx/store";

export enum ChatActionTypes {
    SelectChat = '[Chat] SelectChat',
    SelectChatroom = '[Chat] SelectChatroom',
    CloseChat = '[Chat] CloseChat',
    StartChat = '[Chat] StartChat',
    CreateChatroom = '[Chat] CreateChatroom',
    DeleteChatroom = '[Chat] DeleteChatroom',
    JoinChatroom = '[Chat] JoinChatroom',
    InviteFriends = '[Chat] InviteFriends'
}

export class SelectChat implements Action {
    readonly type = ChatActionTypes.SelectChat;
  
    constructor(public payload: { id: string }) {}
}

export class SelectChatroom implements Action {
    readonly type = ChatActionTypes.SelectChatroom;
  
    constructor(public payload: { id: string }) {}
}

export class CloseChat implements Action {
    readonly type = ChatActionTypes.CloseChat;
  
    constructor(public payload: { id: string }) {}
}

export class CreateChatroom implements Action {
    readonly type = ChatActionTypes.CreateChatroom;
  
    constructor(public payload: { name: string }) {}
}

export class CreatePrivateChat implements Action {
    readonly type = ChatActionTypes.StartChat;
  
    constructor(public payload: { peerId: string }) {}
}

export class DeleteChatroom implements Action {
    readonly type = ChatActionTypes.DeleteChatroom;
  
    constructor(public payload: { id: string }) {}
}

export class JoinChatroom implements Action {
    readonly type = ChatActionTypes.JoinChatroom;
  
    constructor(public payload: { id: string }) {}
}

export class InviteFriends implements Action {
    readonly type = ChatActionTypes.InviteFriends;
  
    constructor(public payload: { chatroomId: string, friendIds: string[] }) {}
}

export type ChatActionsUnion =
    SelectChat
    | SelectChatroom
    | CloseChat
    | CreatePrivateChat
    | CreateChatroom
    | JoinChatroom
    | DeleteChatroom
    | InviteFriends;