// import { Component, OnInit } from '@angular/core';
// import { Observable } from 'rxjs';
// import { Store, select } from '@ngrx/store';
// import { MatTabChangeEvent } from '@angular/material/tabs';
// import { SelectChat, CloseChat } from '../../actions/chat.actions';
// import { map } from 'rxjs/operators';
// import { Chat } from '../../models/Chat';
// import { getPrivateChats, getSelectedPrivateChatId } from '../../reducers';
// import { getUserById } from 'src/app/user/reducers';

// @Component({
//   selector: 'app-chats',
//   templateUrl: './private-chats.component.html',
//   styleUrls: ['./private-chats.component.css'],
//   host: {
//     class: 'd-flex flex-grow-1'
//   }
// })
// export class PrivateChatsComponent implements OnInit {

//   privateChats$: Observable<Chat[]>;
//   selectedPrivateChatId$: Observable<string>;

//   constructor(private store: Store<any>) { }

//   ngOnInit() {
//     this.privateChats$ = this.store.pipe(select(getPrivateChats));
//     this.selectedPrivateChatId$ = this.store.pipe(select(getSelectedPrivateChatId));
//   }

//   trackByChat(chatroom: Chat) {
//     return chatroom.id;
//   }

//   onChatSelected($event: MatTabChangeEvent) {
//     this.store.dispatch(new SelectChat({id: $event.tab.textLabel}));
//   }

//   onChatClosed(id: string) {
//     this.store.dispatch(new CloseChat({id: id}));
//   }

//   getChatName(chat: Chat) {
//     return this.store.pipe(
//       select(getUserById(chat.memberIds[0])),
//       map(user => `Chat with ${user.userName}`)
//     );
//   }

// }
