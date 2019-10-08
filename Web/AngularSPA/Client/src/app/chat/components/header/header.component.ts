import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/chat/models/User';
import { SignOut } from 'src/app/auth/actions/auth.actions';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  @Input()
  user: User;
  @Output()
  signOut = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  onSignOut($event: any) {
    this.signOut.emit($event)
  }
}
