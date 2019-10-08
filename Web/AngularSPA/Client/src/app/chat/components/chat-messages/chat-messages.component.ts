import { Component, OnInit, Input } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { Message } from '../../models/Message';
import { Observable } from 'rxjs';
import { getUserById, getMessagesByChatId } from '../../reducers';
import { map } from 'rxjs/operators';
import { SendMessage } from '../../actions/message.actions';
import { selectUser } from 'src/app/auth/reducers/auth.reducer';
import { User } from 'src/app/chat/models/User';
import * as fromAuth from '../../../auth/reducers';

@Component({
  selector: 'app-chat-messages',
  templateUrl: './chat-messages.component.html',
  styleUrls: ['./chat-messages.component.css']
})
export class ChatMessagesComponent implements OnInit {

  @Input()
  chatId: string;

  messages$: Observable<Message[]>;
  authUser$: Observable<User>;

  constructor(private store: Store<any>) { }

  ngOnInit() {
    this.messages$ = this.store.pipe(
      select(getMessagesByChatId(this.chatId))
    );
    
    this.authUser$ = this.store.pipe(
      select(fromAuth.getUser)
    );
  }

  getAuthor(msg: Message) {
    return this.store.pipe(
      select(getUserById(msg.authorId))
    );
  }

  trackByMessages(msg: Message) {
    return msg.id;
  }

  onMessageSubmitted(content: string) {
    this.store.dispatch(new SendMessage({
      chatId: this.chatId,
      content: content
    }));
  }

}
