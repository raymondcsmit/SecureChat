import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from '../../models/User';
import { UserSearchResultAction } from './UserSearchResultAction';

@Component({
  selector: 'app-user-search-result',
  templateUrl: './user-search-result.component.html',
  styleUrls: ['./user-search-result.component.css']
})
export class UserSearchResultComponent implements OnInit {

  @Input()
  actions: string[];
  @Input()
  searchResult: User[];
  @Output()
  action = new EventEmitter<UserSearchResultAction>();

  actionIcon = {
    add: "person_add"
  }
  selectedId: string;

  constructor() { }

  ngOnInit() {
  }

  onAction(action: string, user: User) {
    this.action.emit({
      action: action,
      data: user
    });
  }

  get displayedColumns() {
    return ['userName', 'email'];
  }

  onMouseOver(id: string) {
    this.selectedId = id;
  }

  isActionDisplayed(id: string) {
    return id === this.selectedId;
  }

}
