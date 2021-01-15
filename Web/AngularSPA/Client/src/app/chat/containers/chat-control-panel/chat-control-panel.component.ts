import { Component, OnInit } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { Observable, combineLatest } from 'rxjs';
import { take, exhaustMap, map, switchMap, filter } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent, ConfirmationDialogResult } from 'src/app/core/components/confirmation-dialog/confirmation-dialog.component';
import { InviteFriendComponent, InviteFriendDialogResult } from '../../../user/containers/invite-friend/invite-friend.component';
import { getSelf, getFriends } from 'src/app/user/reducers';
import { UserEntity } from 'src/app/user/entities/UserEntity';
import { getMessages, getPrivateChats, getChatrooms } from '../../reducers';
import { arrayDistinctUntilChanged } from 'src/app/core/helpers/arrayDistinctUntilChanged';
import { ChatEntity } from '../../entities/ChatEntity';
import { Actions, ofType } from '@ngrx/effects';
import { Success, CoreActionTypes } from 'src/app/core/actions/core.actions';
import { CreateChat, LoadChats } from '../../actions/chat.actions';
import { MatSnackBar } from '@angular/material/snack-bar';

interface ChatDescriptor {
  chat: ChatEntity,  
  memberCount: number;
  messageCount: number;
}

@Component({
  selector: 'app-chat-control-panel',
  templateUrl: './chat-control-panel.component.html',
  styleUrls: ['./chat-control-panel.component.css']
})
export class ChatControlPanelComponent implements OnInit {

  chatrooms$: Observable<ChatDescriptor[]>;
  privateChats$: Observable<ChatDescriptor[]>
  myChatrooms$: Observable<ChatDescriptor[]>;

  constructor(
    private store: Store<any>, 
    private dialog: MatDialog, 
    private actions: Actions,
    private snackBar: MatSnackBar) { }

  ngOnInit() {
    this.chatrooms$ = combineLatest(
      this.store.pipe(
        select(getChatrooms),
        arrayDistinctUntilChanged()),
      this.store.select(getMessages), (chats, messages) => chats.map(c => ({
        chat: c,
        memberCount: c.members.length,
        messageCount: messages.filter(msg => msg.chatId === c.id).length
      })));

    this.privateChats$ = combineLatest(
      this.store.pipe(
        select(getPrivateChats,
        arrayDistinctUntilChanged())),
      this.store.select(getMessages), (chats, messages) => chats.map(c => ({
        chat: c,
        memberCount: c.members.length,
        messageCount: messages.filter(msg => msg.chatId === c.id).length
      })));

      this.myChatrooms$ = this.store.pipe(
        select(getSelf),
        switchMap(loggedInUser => this.chatrooms$.pipe(
          map(chats => chats.filter(cd => cd.chat.owner == loggedInUser.id))
        )));

      this.actions.pipe(
          ofType<Success>(CoreActionTypes.Success),
          filter(action => action.payload.action instanceof CreateChat),
          map(action => action.payload.action as CreateChat)
        ).subscribe(action =>
          this.snackBar.open(`Chat ${action.payload.name} with capacit ${action.payload.capacity} Created`, 'Dismiss', {duration: 5000})
        );

      this.store.dispatch(new LoadChats());
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

  openInviteFriendDialog(chatroomName: string, friends: UserEntity[]): Observable<any> {
    const dialogRef = this.dialog.open(InviteFriendComponent, {
      width: '500px',
      data: {
        friends: friends,
        chatroomName: chatroomName
      }
    });

    return dialogRef.afterClosed();
  }

  onDeleteChatroom(name: string) {
    this.openConfirmationDialog(name).subscribe((result: ConfirmationDialogResult) => {
      if (result === "continue") {
        //this.store.dispatch(new DeleteChatroom({ id: id }))
      }
    });
  }

  onInviteFriend(chatroomName: string) {
    this.store.pipe(
      select(getFriends),
      take(1),
      exhaustMap(friends => this.openInviteFriendDialog(chatroomName, friends)))
      .subscribe((result: InviteFriendDialogResult) => {
        if (result && result.selectedFriendIds && result.selectedFriendIds.length > 0) {
          //this.store.dispatch(new InviteFriends({chatroomId: chatroomId, friendIds: result.selectedFriendIds}));
        }
      });
  }

}
