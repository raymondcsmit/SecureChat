import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { UserQuery } from '../../models/UserQuery';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { notEmptyArrayValidator } from 'src/app/core/helpers/notEmptyArrayValidator';

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.css']
})
export class UserSearchComponent implements OnInit {

  @Output()
  search = new EventEmitter<UserQuery>();

  searchForm: FormGroup;
  selectedId: string;

  constructor() { } 

  ngOnInit() {
    this.searchForm = new FormGroup({
      userName: new FormControl(''),
      email: new FormControl('', [Validators.email])
    }, [notEmptyArrayValidator()])
  }

  onSearch() {
    let query: UserQuery = this.searchForm.value;
    this.search.emit(query);
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
