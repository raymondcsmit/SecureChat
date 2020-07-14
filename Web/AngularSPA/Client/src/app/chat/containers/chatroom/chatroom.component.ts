// import { Component, OnInit, Input } from '@angular/core';
// import { Store, select } from '@ngrx/store';
// import { Observable } from 'rxjs';
// import { Chatroom } from '../../models/Chatroom';
// import { map, exhaustMap } from 'rxjs/operators';
// import { Message } from '../../models/Message';
// import { User } from '../../../user/models/User';
// import { getChatById, getMessagesByChatId } from '../../reducers';
// import { getUsersById } from 'src/app/user/reducers';

// @Component({
//   selector: 'app-chatroom',
//   templateUrl: './chatroom.component.html',
//   styleUrls: ['./chatroom.component.css']
// })
// export class ChatroomComponent implements OnInit {

//   @Input()
//   chatId: string;

//   chatroom$: Observable<Chatroom>;
//   messages$: Observable<Message[]>;
//   members$: Observable<User[]>

//   constructor(private store: Store<any>) { }

//   ngOnInit() {
//     this.chatroom$ = this.store.pipe(
//       select(getChatById(this.chatId)),
//       map(chat => {
//         if (chat !instanceof Chatroom) {
//           throw `Invalid usage of chatroom component: ${this.chatId} is not a chatroom`
//         }
//         return chat as Chatroom;
//       })
//     );

//     this.messages$ = this.store.pipe(
//       select(getMessagesByChatId(this.chatId))
//     )

//     this.members$ = this.chatroom$.pipe(
//       exhaustMap(chatroom => this.store.select(getUsersById(chatroom.memberIds)))
//     )
//   }

  
// }
