import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { CreateChatroom } from '../../actions/chat.actions';
import { Router } from '@angular/router';

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

  constructor(private store: Store<any>, private router: Router) { }

  ngOnInit() {
    this.createChatroomForm = new FormGroup({
      name: new FormControl('', [Validators.required])
    });
  }

  onCreateChatroom() {
    this.store.dispatch(new CreateChatroom(this.createChatroomForm.value));
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
