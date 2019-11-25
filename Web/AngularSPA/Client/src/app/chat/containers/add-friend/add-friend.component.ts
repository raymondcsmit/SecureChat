import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { User } from 'src/app/chat/models/User';
import { UserQuery } from '../../models/UserQuery';
import { switchMap, map, debounceTime } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { AddFriend } from '../../actions/user.actions';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent, ConfirmationDialogResult } from 'src/app/core/components/confirmation-dialog/confirmation-dialog.component';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { ActionEvent } from '../../models/ActionEvent';

@Component({
  selector: 'app-add-friend',
  templateUrl: './add-friend.component.html',
  styleUrls: ['./add-friend.component.css'],
  host: {
    class: 'd-flex flex-grow-1'
  }
})
export class AddFriendComponent implements OnInit {

  searchSubject = new Subject<UserQuery>();
  searchResult$: Observable<User[]>;
  actions = ['add']

  constructor(
    private store: Store<any>, 
    private dialog: MatDialog, 
    private router: Router,
    private userService: UserService) { }

  ngOnInit() {
    this.searchResult$ = this.searchSubject.pipe(
      debounceTime(1000),
      switchMap(query => this.userService.getUsers(query).pipe(
          map(result => result.items.users))
      )
    )
  }

  onSearch(query: UserQuery) {
    this.searchSubject.next(query)
  }

  onAction(actionEvent: ActionEvent) {
    if (!this.actions.includes(actionEvent.action)) {
      return;
    }
    
    let user = actionEvent.data.user;
    this.openConfirmationDialog(user.userName).subscribe((result: ConfirmationDialogResult) => {
      if (result === "continue") {
        this.store.dispatch(new AddFriend({user: user}));
        this.router.navigate(["./"]);
      }
    });
  }

  openConfirmationDialog(userName: string): Observable<any> {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '250px',
      data: {
        message: `Add ${userName} to friend list?`,
        selection: null
      }
    });

    return dialogRef.afterClosed();
  }
}
