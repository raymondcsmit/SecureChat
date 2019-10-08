import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as fromAuth from '../../reducers';
import * as AuthActions from '../../actions/auth.actions';

@Component({
  templateUrl: './sign-out-callback.component.html',
  styleUrls: ['./sign-out-callback.component.css']
})
export class SignOutCallbackComponent implements OnInit {

  constructor(private store: Store<fromAuth.State>) { }

  ngOnInit() {
    this.store.dispatch(new AuthActions.CompleteSignOut());
  }

}
