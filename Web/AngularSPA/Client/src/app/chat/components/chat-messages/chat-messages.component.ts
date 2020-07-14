// import { Component, OnInit, Input } from '@angular/core';
// import { Store, select } from '@ngrx/store';
// import { Message } from '../../models/Message';
// import { Observable } from 'rxjs';
// import { SendMessage } from '../../actions/message.actions';
// import { User } from '../../../user/models/User';
// import { getMessagesByChatId } from '../../reducers';
// import { getSelf, getUserById } from 'src/app/user/reducers';

// @Component({
//   selector: 'app-chat-messages',
//   templateUrl: './chat-messages.component.html',
//   styleUrls: ['./chat-messages.component.css']
// })
// export class ChatMessagesComponent implements OnInit {

//   @Input()
//   chatId: string;

//   messages$: Observable<Message[]>;
//   authUser$: Observable<User>;

//   constructor(private store: Store<any>) { }

//   ngOnInit() {
//     this.messages$ = this.store.pipe(
//       select(getMessagesByChatId(this.chatId))
//     );
    
//     this.authUser$ = this.store.pipe(
//       select(getSelf)
//     );
//   }

//   getAuthor(msg: Message) {
//     return this.store.pipe(
//       select(getUserById(msg.authorId))
//     );
//   }

//   trackByMessages(msg: Message) {
//     return msg.id;
//   }

//   onMessageSubmitted(content: string) {
//     this.store.dispatch(new SendMessage({
//       chatId: this.chatId,
//       content: content
//     }));
//   }

// }
