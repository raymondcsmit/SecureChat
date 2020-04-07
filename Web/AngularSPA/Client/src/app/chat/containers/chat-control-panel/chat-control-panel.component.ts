import { Component, OnInit } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { Observable, combineLatest } from 'rxjs';
import { getChatroomsByOwnerId, getAllMessages } from '../../reducers';
import { exhaustMap, take, switchMap } from 'rxjs/operators';
import { User } from 'src/app/user/models/User';
import { DeleteChatroom, InviteFriends } from '../../actions/chat.actions';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent, ConfirmationDialogResult } from 'src/app/core/components/confirmation-dialog/confirmation-dialog.component';
import { InviteFriendComponent, InviteFriendDialogResult } from '../../../user/containers/invite-friend/invite-friend.component';
import { getSelf, getFriends } from 'src/app/user/reducers';

interface ChatroomInfo {
  id: string;
  name: string;
  numMembers: number;
  numMessages: number;
}

@Component({
  selector: 'app-chat-control-panel',
  templateUrl: './chat-control-panel.component.html',
  styleUrls: ['./chat-control-panel.component.css']
})
export class ChatControlPanelComponent implements OnInit {

  myChatrooms$: Observable<ChatroomInfo[]>;

  constructor(private store: Store<any>, private dialog: MatDialog) { }

  ngOnInit() {
    this.myChatrooms$ = this.store.pipe(
      select(getSelf),
      switchMap(loggedInUser => combineLatest(
        this.store.select(getChatroomsByOwnerId(loggedInUser.id), 
        this.store.select(getAllMessages)), (chatrooms, messages) => chatrooms.map(chatroom => ({
          id: chatroom.id,
          name: chatroom.name,
          numMembers: chatroom.memberIds.length,
          numMessages: messages.filter(msg => msg.chatId === chatroom.id).length
        })))))
  }

  openConfirmationDialog(chatroomName: string): Observable<any> {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '250px',
      data: {
        message: `Delete chatroom ${chatroomName}?`,
        selection: null
      }
    });

    return dialogRef.afterClosed();
  }

  openInviteFriendDialog(chatroomName: string, friends: User[]): Observable<any> {
    const dialogRef = this.dialog.open(InviteFriendComponent, {
      width: '500px',
      data: {
        friends: friends,
        chatroomName: chatroomName
      }
    });

    return dialogRef.afterClosed();
  }

  onDeleteChatroom(id: string, name: string) {
    this.openConfirmationDialog(name).subscribe((result: ConfirmationDialogResult) => {
      if (result === "continue") {
        this.store.dispatch(new DeleteChatroom({ id: id }))
      }
    });
  }

  onInviteFriend(chatroomId: string, chatroomName: string) {
    this.store.pipe(
      select(getFriends),
      take(1),
      exhaustMap(friends => this.openInviteFriendDialog(chatroomName, friends)))
      .subscribe((result: InviteFriendDialogResult) => {
        if (result && result.selectedFriendIds && result.selectedFriendIds.length > 0) {
          this.store.dispatch(new InviteFriends({chatroomId: chatroomId, friendIds: result.selectedFriendIds}));
        }
      });
  }

}
