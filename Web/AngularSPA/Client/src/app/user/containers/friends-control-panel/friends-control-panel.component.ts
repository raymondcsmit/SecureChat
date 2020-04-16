import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { User } from 'src/app/user/models/User';
import { Store, select } from '@ngrx/store';
import { CreatePrivateChat } from '../../../chat/actions/chat.actions';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent, ConfirmationDialogResult } from 'src/app/core/components/confirmation-dialog/confirmation-dialog.component';
import { Router } from '@angular/router';
import { getFriends, getFriendshipRequests, getUsersById } from '../../reducers';
import { RemoveFriend } from '../../actions/friendship.actions';
import { withLatestFrom, mergeMap, first, map, switchMap } from 'rxjs/operators';
import * as _ from 'lodash';
import { FriendshipRequestEntity } from '../../entities/FriendshipRequestEntity';
import { UserEntity } from '../../entities/UserEntity';
import { UpdateFriendshipRequest } from '../../actions/friendship-request.actions';

@Component({
  selector: 'app-friends-control-panel',
  templateUrl: './friends-control-panel.component.html',
  styleUrls: ['./friends-control-panel.component.css']
})
export class FriendsControlPanelComponent implements OnInit {

  friends: Observable<User[]>;
  friendshipRequests: Observable<{ f: FriendshipRequestEntity; u: UserEntity; }[]>;

  constructor(private store: Store<any>, private dialog: MatDialog, private router: Router) { }

  ngOnInit() {
    this.friends = this.store.select(getFriends);
    this.friendshipRequests = this.store.pipe(
      select(getFriendshipRequests),
      switchMap(friendshipRequests => 
        this.store.pipe(
          select(getUsersById(friendshipRequests.map(f => f.requester))),
          map(users => _.zipWith(friendshipRequests, users, (f, u) => ({ f, u })))
        )
      )
    )
  }
  
  onCreatePrivateChat(peerId: string) {
    this.store.dispatch(new CreatePrivateChat({ peerId: peerId }));
    let x = {
      e: 1,
      s: 2
    }
  }

  openConfirmationDialog(message: string): Observable<any> {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '250px',
      data: {
        message: message,
        selection: null
      }
    });

    return dialogRef.afterClosed();
  }

  onRemoveFriend(id: string, name: string) {
    this.openConfirmationDialog(`Remove ${name}?`).subscribe((result: ConfirmationDialogResult) => {
      if (result === "continue") {
        this.store.dispatch(new RemoveFriend({ id: id }));
      }
    });
  }

  onAcceptFriendshipRequest(id: string, name: string) {
    this.openConfirmationDialog(`Accept ${name}'s friendship request?`).subscribe((result: ConfirmationDialogResult) => {
      if (result === "continue") {
        this.store.dispatch(new UpdateFriendshipRequest({id: id, status: "accepted"}));
      }
    });
  }

  onRejectFriendshipRequest(id: string, name: string) {
    this.openConfirmationDialog(`Reject ${name}'s friendship request?`).subscribe((result: ConfirmationDialogResult) => {
      if (result === "continue") {
        this.store.dispatch(new UpdateFriendshipRequest({id: id, status: "rejected"}));
      }
    });
  }
}
