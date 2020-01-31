import { Component, OnInit } from '@angular/core';
import { Observable, Subject, merge, of } from 'rxjs';
import { User } from 'src/app/chat/models/User';
import { switchMap, map, debounceTime } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { AddFriend } from '../../actions/user.actions';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent, ConfirmationDialogResult } from 'src/app/core/components/confirmation-dialog/confirmation-dialog.component';
import { Router } from '@angular/router';
import { ActionEvent } from '../../models/ActionEvent';
import { ChatService } from '../../services/chat.service';
import { Query } from 'src/app/core/models/Query';
import { PageEvent } from '@angular/material/paginator';
import { Pagination } from 'src/app/core/models/Pagination';

@Component({
  selector: 'app-add-friend',
  templateUrl: './add-friend.component.html',
  styleUrls: ['./add-friend.component.css'],
  host: {
    class: 'd-flex flex-grow-1'
  }
})
export class AddFriendComponent implements OnInit {

  searchSubject = new Subject<Query<User>>();
  searchResult$: Observable<User[]>;
  actions = ['add']
  displayedColumns: string[] = ['select', 'position', 'name', 'weight', 'symbol'];

  pagination: Pagination = Pagination.Default;
  previousQuery: Partial<User>;

  constructor(
    private store: Store<any>, 
    private dialog: MatDialog, 
    private router: Router,
    private chatService: ChatService) { }

  ngOnInit() {
    this.searchResult$ = merge(of([]), this.searchSubject.pipe(
      debounceTime(1000),
      switchMap(query => this.chatService.getUsers(query).pipe(
          map(result => result.items))
      )
    ));
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
