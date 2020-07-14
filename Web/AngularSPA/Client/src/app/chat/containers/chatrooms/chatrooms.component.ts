// import { Component, OnInit } from '@angular/core';
// import { Observable } from 'rxjs';
// import { Chat } from '../../models/Chat';
// import { Store, select } from '@ngrx/store';
// import { getChatrooms, getSelectedChatroomId } from '../../reducers';
// import { SelectChatroom, CloseChat } from '../../actions/chat.actions';

// @Component({
//   selector: 'app-chatrooms',
//   templateUrl: './chatrooms.component.html',
//   styleUrls: ['./chatrooms.component.css']
// })
// export class ChatroomsComponent implements OnInit {

//   chatrooms$: Observable<Chat[]>;
//   selectedChatroomId$: Observable<string>;

//   constructor(private store: Store<any>) { }

//   ngOnInit() {
//     this.chatrooms$ = this.store.pipe(select(getChatrooms));
//     this.selectedChatroomId$ = this.store.pipe(select(getSelectedChatroomId));
//   }

//   onChatroomSelected(id: string) {
//     this.store.dispatch(new SelectChatroom({id: id}));
//   }

//   onChatClosed(id: string) {
//     this.store.dispatch(new CloseChat({id: id}));
//   }
// }
