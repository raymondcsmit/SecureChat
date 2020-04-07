import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { notEmptyArrayValidator } from 'src/app/core/validators/notEmptyArrayValidator';
import { User } from '../../models/User';

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.css']
})
export class UserSearchComponent implements OnInit {

  @Output()
  search = new EventEmitter<Partial<User>>();

  searchForm: FormGroup;
  selectedId: string;

  constructor() { } 

  ngOnInit() {
    this.searchForm = new FormGroup({
      userName: new FormControl(''),
      email: new FormControl(null, [Validators.email])
    }, [notEmptyArrayValidator()])
  }

  onSearch() {
    this.search.emit(this.searchForm.value);
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
