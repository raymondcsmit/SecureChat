import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, FormArray, Validators } from '@angular/forms';
import { Store, select } from '@ngrx/store';
import { getSelf } from '../../reducers';
import { Observable, Subscription } from 'rxjs';
import { User } from '../../models/User';
import { getPermissions } from 'src/app/auth/reducers';
import { map, filter } from 'rxjs/operators';
import { ConfirmEmail, UserActionTypes } from '../../actions/user.actions';
import { Actions, ofType } from '@ngrx/effects';
import { Success, CoreActionTypes } from 'src/app/core/actions/actions';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-personal-info',
  templateUrl: './personal-info.component.html',
  styleUrls: ['./personal-info.component.css'],
  host: {
    class: 'd-flex flex-grow-1'
  }
})
export class PersonalInfoComponent implements OnInit, OnDestroy {
  editPersonalInfoForm: FormGroup;
  user$: Observable<User>;
  permissions$: Observable<string>;
  editing: boolean = false;
  confirmEmailSuccess$: Observable<boolean>;
  subscriptions: Subscription[] = [];

  constructor(
    private store: Store<any>, 
    private actions: Actions,
    private snackBar: MatSnackBar) 
  { 
    this.editPersonalInfoForm = new FormGroup({
      userName: new FormControl({value: '', disabled: true}, [Validators.required]),
      email: new FormControl({value: '', disabled: true}, [Validators.required, Validators.email])
    });
    this.editPersonalInfoForm.disable();
  }

  ngOnInit() {
    this.user$ = this.store.pipe(
      select(getSelf)
    );

    this.permissions$ = this.store.pipe(
      select(getPermissions),
      map(p => p.join(', '))
    );

    this.confirmEmailSuccess$ = this.actions.pipe(
      ofType<Success>(CoreActionTypes.Success),
      filter(action => action.payload.action instanceof ConfirmEmail),
      map(_ => true)
    );
    this.subscriptions.push(this.confirmEmailSuccess$.subscribe(_ =>
      this.snackBar.open("Email confirmation email sent", '', {duration: 3000})
    ));
  }

  ngOnDestroy() {
    for (let s of this.subscriptions) {
      s.unsubscribe();
    }
  }

  onEdit() {
    this.editing = true;
    this.editPersonalInfoForm.reset();
    this.editPersonalInfoForm.enable();
  }

  onConfirmEmail() {
    this.store.dispatch(new ConfirmEmail());
  }

}
