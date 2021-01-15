import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, of, combineLatest, interval, Subscription } from 'rxjs';
import { User } from 'src/app/user/models/User';
import { Store, select } from '@ngrx/store';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent, ConfirmationDialogResult } from 'src/app/core/components/confirmation-dialog/confirmation-dialog.component';
import { Router } from '@angular/router';
import { getFriends, getFriendshipRequests, getUsersById, getSelf, getUserStatus } from '../../reducers';
import { RemoveFriend, LoadFriendships } from '../../actions/friendship.actions';
import { withLatestFrom, mergeMap, first, map, switchMap, filter, tap } from 'rxjs/operators';
import * as _ from 'lodash';
import { FriendshipRequestEntity } from '../../entities/FriendshipRequestEntity';
import { UserEntity } from '../../entities/UserEntity';
import { UpdateFriendshipRequest, LoadFriendshipRequests } from '../../actions/friendship-request.actions';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { FriendshipRequest, friendshipRequestSchema } from '../../models/FriendshipRequest';
import { normalize } from 'normalizr';
import { AddEntity, UpsertEntity, UpsertEntities, DeleteEntity } from 'src/app/core/actions/entity.actions';
import { Friendship, friendshipSchema } from '../../models/Friendship';
import { FriendshipEntity } from '../../entities/FriendshipEntity';
import { UpdateUserStatus, LoadOnlineStatus } from '../../actions/user.actions';
import { appConfig } from 'src/app.config';
import { MatSnackBar } from '@angular/material/snack-bar';

interface UserWithStatus extends User {
  status: 'online'|'offline'|'idle';
}

@Component({
  selector: 'app-friends-control-panel',
  templateUrl: './friends-control-panel.component.html',
  styleUrls: ['./friends-control-panel.component.css']
})
export class FriendsControlPanelComponent implements OnInit, OnDestroy {

  friends$: Observable<UserWithStatus[]>;
  friendshipRequests$: Observable<{ f: FriendshipRequestEntity; u: UserEntity; }[]>;
  subscription: Subscription;

  constructor(
    private store: Store<any>, 
    private dialog: MatDialog, 
    private signalr: SignalrService) { }

  ngOnInit() {
    this.friends$ = combineLatest(
      this.store.select(getFriends), this.store.select(getUserStatus)
    ).pipe(
      map(([users, userStatus]) => users.map(user => { return {...user, status: userStatus[user.id]} }))
    );
    
    this.friendshipRequests$ = this.store.pipe(
      select(getFriendshipRequests),
      withLatestFrom(this.store.select(getSelf)),
      filter(([, self]) => self != null),
      map(([friendshipRequests, self]) => friendshipRequests.filter(f => f.requester != self.id && !f.outcome)),
      switchMap(friendshipRequests => 
        this.store.pipe(
          select(getUsersById(friendshipRequests.map(f => f.requester))),
          map(users => _.zipWith(friendshipRequests, users, (f, u) => ({ f, u })))
        )
      )
    );

    this.store.dispatch(new LoadFriendshipRequests());
    this.store.dispatch(new LoadFriendships());
    
    this.store.dispatch(new LoadOnlineStatus());
    this.subscription = interval(appConfig.refreshMilliseconds).pipe(
      tap(() => this.store.dispatch(new LoadOnlineStatus()))
    ).subscribe();

    this.addSignalrHandlers();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
  
  // onCreatePrivateChat(peerId: string) {
  //   this.store.dispatch(new CreatePrivateChat({ peerId: peerId }));
  //   let x = {
  //     e: 1,
  //     s: 2
  //   }
  // }

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

  private addSignalrHandlers(): void {
    this.signalr.addHandler('FriendshipRequestReceived', (msg: FriendshipRequest) => {
      console.log(`Friendship request received from ${msg.requester.id}`);
      const normalized = normalize(msg, friendshipRequestSchema);
      const friendshipRequest = Object.values(normalized.entities.friendshipRequests)[0] as FriendshipRequestEntity;
      const requestee = normalized.entities.users[msg.requester.id] as UserEntity;
      this.store.dispatch(new AddEntity(FriendshipRequestEntity.name, {entity: friendshipRequest}));
      this.store.dispatch(new UpsertEntity(UserEntity.name, {entity: requestee}));
    });

    this.signalr.addHandler('FriendshipCreated', (msg: Friendship) => {
      console.log(`Friendship created between ${msg.user1.id} and ${msg.user2.id}`);
      const normalized = normalize(msg, friendshipSchema);
      const friendship = Object.values(normalized.entities.friendships)[0] as FriendshipEntity;
      const users = Object.values(normalized.entities.users) as UserEntity[];
      this.store.dispatch(new AddEntity(FriendshipEntity.name, {entity: friendship}));
      this.store.dispatch(new UpsertEntities(UserEntity.name, {entities: users}));
    });

    this.signalr.addHandler('UserConnected', (msg: any) => {
      let obj = JSON.parse(msg);
      console.log(`User ${obj.userId} is online`);
      this.store.dispatch(new UpdateUserStatus({id: obj.userId, status: "online"}));
    });

    this.signalr.addHandler('UserDisconnected', (msg: any) => {
      let obj = JSON.parse(msg);
      console.log(`User ${obj.userId} is offline`);
      this.store.dispatch(new UpdateUserStatus({id: obj.userId, status: "offline"}));
    });

    this.signalr.addHandler('FriendshipDeleted', (msg: any) => {
      const friendship = JSON.parse(msg).friendship;
      console.log(`Friendship between ${friendship.User1} and ${friendship.User2} has been deleted`);
      this.store.dispatch(new DeleteEntity(FriendshipEntity.name, {id: friendship.Id}));
      this.showSnackbarMessage(`Friendship between ${friendship.User1} and ${friendship.User2} has been deleted`);
    });

    this.signalr.registerHandlers();
  }

  private showSnackbarMessage(message: string) {
    this.snackBar.open(message, 'Dismiss', {duration: 5000});
  }
}
