import { Component, OnInit } from '@angular/core';
import { Store, select } from '@ngrx/store';
import * as authActions from '../../../auth/actions/auth.actions';
import { Observable } from 'rxjs';
import { User } from 'src/app/user/models/User';
import { getSelf } from 'src/app/user/reducers';

@Component({
  selector: 'app-chat-root',
  templateUrl: './chat-root.component.html', 
  styleUrls: ['./chat-root.component.css'],
  host: {
    class: 'd-flex flex-grow-1'
  }
})
export class ChatRootComponent implements OnInit {

  user$: Observable<User>;
  
  constructor(private store: Store<any>) { }

  ngOnInit() {
    this.user$ = this.store.pipe(
      select(getSelf)
    );
  }

  signOut() {
    this.store.dispatch(new authActions.SignOut());
  }
}
