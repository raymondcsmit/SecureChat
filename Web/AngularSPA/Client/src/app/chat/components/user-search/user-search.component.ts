import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { notEmptyArrayValidator } from 'src/app/core/validators/notEmptyArrayValidator';
import { PaginatedQuery } from 'src/app/core/models/PaginatedQuery';
import { User } from '../../models/User';
import { PageEvent } from '@angular/material/paginator';
import { Pagination } from 'src/app/core/models/Pagination';

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.css']
})
export class UserSearchComponent implements OnInit {

  @Output()
  search = new EventEmitter<PaginatedQuery<User>>();

  paginationDefaults = {
    length: 100,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 100]
  };
  searchForm: FormGroup;
  selectedId: string;
  currentPage: PageEvent;

  constructor() { } 

  ngOnInit() {
    this.searchForm = new FormGroup({
      userName: new FormControl(''),
      email: new FormControl('', [Validators.email])
    }, [notEmptyArrayValidator()])
  }

  onSearch() {
    let query: PaginatedQuery<User> = {
      query: this.searchForm.value,
      pagination: new Pagination(this.currentPage)
    }
    this.search.emit(query);
  }

  onPageEvent(event: PageEvent) {
    if (this.currentPage) {
      this.currentPage = event;
      this.onSearch();
    }
    else {
      this.currentPage = event;
    }
  }

  hasError(controlName?: string, error?: string) {
    if (!controlName) {
      return this.searchForm.touched && this.searchForm.errors && this.searchForm.errors.mustHaveAtLeastOne;
    }
    const control = this.searchForm.controls[controlName];
    if (!error)
      return control && control.touched && control.invalid;
    else
      return control && control.touched && control.invalid && control.errors[error];
  }
}
