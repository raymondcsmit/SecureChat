import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, FormArray, Validators } from '@angular/forms';
import { Store, select } from '@ngrx/store';
import { getSelf } from '../../reducers';
import { Observable, Subscription } from 'rxjs';
import { User } from '../../models/User';
import { getPermissions, getEmailConfirmed } from 'src/app/auth/reducers';
import { map, filter, tap } from 'rxjs/operators';
import { ConfirmEmail, UserActionTypes, UpdateUser, LoadSelf } from '../../actions/user.actions';
import { Actions, ofType, Effect } from '@ngrx/effects';
import { Success, CoreActionTypes, Failure } from 'src/app/core/actions/actions';
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
  subscriptions: Subscription[] = [];
  editProfileForm: FormGroup;
  emailConfirmed$: Observable<boolean>;

  constructor(
    private store: Store<any>, 
    private actions: Actions,
    private snackBar: MatSnackBar) 
  { 
    this.editPersonalInfoForm = new FormGroup({
      userName: new FormControl({value: '', disabled: true}, [Validators.required]),
      email: new FormControl({value: '', disabled: true}, [Validators.required, Validators.email]),
    });
    this.editPersonalInfoForm.disable();
  }

  ngOnInit() {
    this.user$ = this.store.pipe(
      select(getSelf)
    );

    this.emailConfirmed$ = this.store.pipe(
      select(getEmailConfirmed)
    );

    this.permissions$ = this.store.pipe(
      select(getPermissions),
      map(p => p.join(', '))
    );

    let s = this.actions.pipe(
      ofType<Success>(CoreActionTypes.Success),
      filter(action => action.payload.action instanceof ConfirmEmail)
    ).subscribe(_ =>
      this.snackBar.open("Email confirmation email sent", 'Dismiss', {duration: 5000})
    );
    this.subscriptions.push(s);

    let s2 = this.actions.pipe(
      ofType<Success>(CoreActionTypes.Success),
      filter(action => action.payload.action instanceof UpdateUser)
    ).subscribe(_ => {
      this.snackBar.open("User successfully updated", 'Dismiss', {duration: 5000});
      this.store.dispatch(new LoadSelf());
      this.stopEditing();
    });
    this.subscriptions.push(s2);

    let s3 = this.actions.pipe(
      ofType<Failure>(CoreActionTypes.Failure),
      filter(action => action.payload.action instanceof UpdateUser)
    ).subscribe(action => {
      this.snackBar.open("User update failed", 'Dismiss', {duration: 5000})
      this.stopEditing();
    });
    this.subscriptions.push(s3);

    let s4 = this.user$.pipe(
      filter(u => u.profile != null)
    ).subscribe(u => this.createProfile());
    this.subscriptions.push(s4);
  }

  ngOnDestroy() {
    for (let s of this.subscriptions) {
      s.unsubscribe();
    }
  }

  onEdit() {
    this.startEditing();
  }

  onSubmit(id: string) {
    let user = this.editPersonalInfoForm.value;
    this.store.dispatch(new UpdateUser({id: id, user: user}));
  }

  onConfirmEmail() {
    this.store.dispatch(new ConfirmEmail());
  }

  createProfile() {
    this.editProfileForm = new FormGroup({
      age: new FormControl({value: ''}, [Validators.required]),
      sex: new FormControl({value: ''}, [Validators.required, Validators.pattern('^M|F|m|f$')]),
      location: new FormControl({value: ''}, [Validators.required]),
    });
    this.editProfileForm.disable();
  }

  private startEditing() {
    this.editing = true;
    this.editPersonalInfoForm.markAsUntouched();
    this.editPersonalInfoForm.enable();
    if (this.editProfileForm)
      this.editProfileForm.enable();
  }

  private stopEditing() {
    this.editing = false;
    this.editPersonalInfoForm.markAsUntouched();
    this.editPersonalInfoForm.disable();
    if (this.editProfileForm)
      this.editProfileForm.disable();
  }
}
