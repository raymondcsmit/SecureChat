import { Component, OnInit } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { Observable } from 'rxjs';
import { getGlobalBusy } from '../../reducers';
import { delay } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  isBusy$: Observable<boolean>;

  ngOnInit() {
    this.isBusy$ = this.store.pipe(
      select(getGlobalBusy),
      delay(0) // prevents ExpressionChangedAfterItHasBeenCheckedError
    );
  }

  
  constructor(private store: Store<any>) {}
}
