import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { CreateChat } from '../../actions/chat.actions';
import { Router } from '@angular/router';
import { validateInteger } from 'src/app/core/validators/validateInteger';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-create-chatroom',
  templateUrl: './create-chatroom.component.html',
  styleUrls: ['./create-chatroom.component.css'],
  host: {
    class: 'd-flex flex-grow-1'
  }
})
export class CreateChatroomComponent implements OnInit {

  createChatroomForm: FormGroup;

  constructor(
    private store: Store<any>, 
    private router: Router, 
    private snackBar: MatSnackBar) { }

  ngOnInit() {
    this.createChatroomForm = new FormGroup({
      name: new FormControl('', [Validators.required]),
      capacity: new FormControl('', [Validators.required, validateInteger(3, 1000)])
    });
  }

  onCreateChatroom() {
    const formValue = this.createChatroomForm.value;
    this.store.dispatch(new CreateChat({name: formValue.name, capacity: parseInt(formValue.capacity)}));
    this.router.navigate(['./']);
  }

  hasError(controlName: string, error?: string) {
    const control = this.createChatroomForm.controls[controlName];
    if (!error)
      return control && control.touched && control.invalid;
    else
      return control && control.touched && control.invalid && control.errors[error];
  }
}
