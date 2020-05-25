import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as fromAuth from '../../reducers';
import * as AuthActions from '../../actions/auth.actions';


@Component({
  selector: 'app-sign-in-silent-callback',
  templateUrl: './sign-in-silent-callback.component.html',
  styleUrls: ['./sign-in-silent-callback.component.css']
})
export class SignInSilentCallbackComponent implements OnInit {

  constructor(private store: Store<fromAuth.State>) { }

  ngOnInit() {
    this.store.dispatch(new AuthActions.CompleteSignInSilent());
  }

}
