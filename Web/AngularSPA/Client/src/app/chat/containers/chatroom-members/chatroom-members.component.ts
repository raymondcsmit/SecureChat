import { Component, OnInit, Input } from '@angular/core';
import { Observable, of, from } from 'rxjs';
import { User } from 'src/app/user/models/User';
import { Store, select } from '@ngrx/store';
import { map, concatMap } from 'rxjs/operators';
import { getChatById } from '../../reducers';
import { getUserById } from 'src/app/user/reducers';

@Component({
  selector: 'app-chatroom-members',
  templateUrl: './chatroom-members.component.html',
  styleUrls: ['./chatroom-members.component.css']
})
export class ChatroomMembersComponent implements OnInit {

  @Input()
  chatId: string;

  memberIds$: Observable<string[]>;

  constructor(private store: Store<any>) { }

  ngOnInit() {
    this.memberIds$ = this.store.pipe(
      select(getChatById(this.chatId)),
      map(chat => chat.members)
    );
  }

  getUser(id: string) {
    return this.store.pipe(
      select(getUserById(id))
    );
  }

  trackByMemberIds(index: number, memberId: string) {
    return memberId;
  }

}
