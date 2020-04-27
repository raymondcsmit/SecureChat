import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, Subject, merge, of, Subscription } from 'rxjs';
import { User } from 'src/app/user/models/User';
import { switchMap, map, debounceTime, filter, tap } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent, ConfirmationDialogResult } from 'src/app/core/components/confirmation-dialog/confirmation-dialog.component';
import { Router } from '@angular/router';
import { ActionEvent } from '../../../core/models/ActionEvent';
import { Query } from 'src/app/core/models/Query';
import { PageEvent } from '@angular/material/paginator';
import { Pagination } from 'src/app/core/models/Pagination';
import { UsersService } from '../../services/users.service';
import { AddFriend, FriendshipRequestActionTypes } from '../../actions/friendship-request.actions';
import { Actions } from '@ngrx/effects';
import { CoreActionTypes, Failure } from 'src/app/core/actions/core.actions';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-friend',
  templateUrl: './add-friend.component.html',
  styleUrls: ['./add-friend.component.css'],
  host: {
    class: 'd-flex flex-grow-1'
  }
})
export class AddFriendComponent implements OnInit, OnDestroy {

  searchSubject = new Subject<Query<User>>();
  searchResult$: Observable<User[]>;
  actions = ['add']
  displayedColumns: string[] = ['select', 'position', 'name', 'weight', 'symbol'];

  pagination: Pagination = Pagination.Default;
  previousQuery: Partial<User>;
  subscription: Subscription;

  constructor(
    private store: Store<any>, 
    private dialog: MatDialog, 
    private router: Router,
    private userService: UsersService,
    private action$: Actions,
    private snackBar: MatSnackBar) { }

  ngOnInit() {
    this.searchResult$ = merge(of([]), this.searchSubject.pipe(
      debounceTime(1000),
      switchMap(query => this.userService.getUsers(query).pipe(
          map(result => result.items))
      )
    ));

    this.subscription =  this.action$.pipe(
      filter(action => action.type == CoreActionTypes.Failure && (action as Failure).payload.action.type == FriendshipRequestActionTypes.AddFriend),
      tap((action: Failure) => this.snackBar.open(action.payload.errors[0], 'Dismiss', {duration: 5000})
      )
    ).subscribe();
  }

  onSearch(query: Partial<User>) {
    let paginatedQuery: Query<User> = {
      criteria: query,
      pagination: this.pagination
    }
    this.previousQuery = query;
    this.searchSubject.next(paginatedQuery);
  }

  onPaging(pageEvent: PageEvent) {
    this.pagination = new Pagination(pageEvent);
    if (this.previousQuery) {
      this.onSearch(this.previousQuery);
    }
  }

  onAction(actionEvent: ActionEvent) {
    if (!this.actions.includes(actionEvent.action)) {
      return;
    }
    
    let user = actionEvent.data.user;
    this.openConfirmationDialog(user.userName).subscribe((result: ConfirmationDialogResult) => {
      if (result === "continue") {
        this.store.dispatch(new AddFriend({user: user}));
        //this.router.navigate(["./chat"]);
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

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
