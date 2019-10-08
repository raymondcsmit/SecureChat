import { Component, OnInit, Inject } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { getFriends } from '../../reducers';
import { Observable } from 'rxjs';
import { FormControl, Validators } from '@angular/forms';
import { User } from '../../models/User';

export interface InviteFriendDialogData {
  friends: User[],
  chatroomName: string
}

export interface InviteFriendDialogResult {
  selectedFriendIds: string[];
}

@Component({
  selector: 'app-invite-friend',
  templateUrl: './invite-friend.component.html',
  styleUrls: ['./invite-friend.component.css']
})
export class InviteFriendComponent implements OnInit {

  friends: User[];
  chatroomName: string;
  selection = new FormControl([], Validators.required);

  constructor(public dialogRef: MatDialogRef<InviteFriendComponent>, @Inject(MAT_DIALOG_DATA) private data: InviteFriendDialogData) { }

  ngOnInit() {
    this.friends = this.data.friends;
    this.chatroomName =  this.data.chatroomName;
  }

  onInvite() {
    this.dialogRef.close({
      selectedFriendIds: this.selection.value
    });
  }

  onClose() {
    this.dialogRef.close();
  }

}
