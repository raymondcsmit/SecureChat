import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { LoadEntities } from '../../actions/entity.actions';
import { getChatrooms, getPrivateChats, getUserById } from '../../reducers';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { SelectChat, SelectChatroom, CloseChat } from '../../actions/chat.actions';
import { map } from 'rxjs/operators';
import { Message } from '../../models/Message';
import { Chat } from '../../models/Chat';

@Component({
  selector: 'app-chats',
  templateUrl: './chats.component.html',
  styleUrls: ['./chats.component.css'],
  host: {
    class: 'd-flex flex-grow-1'
  }
})
export class ChatsComponent implements OnInit {

  ngOnInit() {
  }
  
}
