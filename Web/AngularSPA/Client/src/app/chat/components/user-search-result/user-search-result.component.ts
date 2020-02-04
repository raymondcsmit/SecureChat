import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from '../../models/User';
import { ActionEvent } from '../../models/ActionEvent';
import { Pagination } from 'src/app/core/models/Pagination';
import { PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-user-search-result',
  templateUrl: './user-search-result.component.html',
  styleUrls: ['./user-search-result.component.css']
})
export class UserSearchResultComponent implements OnInit {

  @Input()
  actions: string[];

  _searchResult: User[] = [];
  @Input()
  get searchResult() {
    return this._searchResult;
  }
  set searchResult(result: User[]) {
    if (result) {
      this._searchResult = result;
      this.dataSource = new MatTableDataSource<User>(result);
    }
  }


  @Output()
  action = new EventEmitter<ActionEvent>();
  @Output()
  paging = new EventEmitter<PageEvent>();

  paginationDefaults = {
    pageSize: Pagination.Default.limit,
    pageSizeOptions: [5, 10, 25, 100]
  };

  actionIcon = {
    add: "person_add"
  }
  selectedId: string;
  dataSource: MatTableDataSource<User>;
  displayedColumns = ['action', 'userName', 'email'];

  constructor() { }

  ngOnInit() {
  }

  onAction(action: string, user: User) {
    this.action.emit({
      action: action,
      data: {
        user: user
      }
    });
  }

  onMouseOver(id: string) {
    this.selectedId = id;
  }

  isActionDisplayed(id: string) {
    return id === this.selectedId;
  }

  onPageEvent(pageEvent: PageEvent) {
    this.paging.emit(pageEvent);
  }

}
