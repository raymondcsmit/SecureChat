import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { User } from 'src/app/chat/models/User';
import { Store, select } from '@ngrx/store';
import { CreatePrivateChat } from '../../actions/chat.actions';
import { RemoveFriend } from '../../actions/user.actions';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent, ConfirmationDialogResult } from 'src/app/core/components/confirmation-dialog/confirmation-dialog.component';
import { Router } from '@angular/router';
import { getFriends } from '../../reducers';

@Component({
  selector: 'app-friends-control-panel',
  templateUrl: './friends-control-panel.component.html',
  styleUrls: ['./friends-control-panel.component.css']
})
export class FriendsControlPanelComponent implements OnInit {

  friends: Observable<User[]>;

  constructor(private store: Store<any>, private dialog: MatDialog, private router: Router) { }

  ngOnInit() {
    this.friends = this.store.pipe(
      select(getFriends)
    );
  }

  onCreatePrivateChat(peerId: string) {
    this.store.dispatch(new CreatePrivateChat({ peerId: peerId }));
  }

  openConfirmationDialog(userName: string): Observable<any> {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '250px',
      data: {
        message: `Remove ${userName}?`,
        selection: null
      }
    });

    return dialogRef.afterClosed();
  }

  onRemoveFriend(id: string, name: string) {
    this.openConfirmationDialog(name).subscribe((result: ConfirmationDialogResult) => {
      if (result === "continue") {
        this.store.dispatch(new RemoveFriend({ id: id }))
      }
    });
  }
}
