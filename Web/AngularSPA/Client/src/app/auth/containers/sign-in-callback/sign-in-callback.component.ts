import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as fromAuth from '../../reducers';
import * as AuthActions from '../../actions/auth.actions';

@Component({
  templateUrl: './sign-in-callback.component.html',
  styleUrls: ['./sign-in-callback.component.css']
})
export class SignInCallbackComponent implements OnInit {

  constructor(private store: Store<fromAuth.State>) { }

  ngOnInit() {
    this.store.dispatch(new AuthActions.CompleteSignIn());
  }
}
