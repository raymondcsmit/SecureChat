import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/user/models/User';
import { SignOut } from 'src/app/auth/actions/auth.actions';
import { Observable } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { getSelf } from 'src/app/user/reducers';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  user$: Observable<User>;
  
  constructor(private store: Store<any>) { }

  ngOnInit() {
    this.user$ = this.store.pipe(
      select(getSelf)
    );
  }

  signOut() {
    this.store.dispatch(new SignOut());
  }
}
