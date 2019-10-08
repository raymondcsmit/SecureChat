import { Action } from "@ngrx/store";
import { User as OidcUser } from 'oidc-client';
import { Error } from "src/app/core/models/Error";

export enum AuthActionTypes {
    SignIn = '[Auth] SignIn',
    SignOut = '[Auth] SignOut',
    SignInSuccess = '[Auth] SignIn Success',
    SignInFailure = '[Auth] Authentication Failure',
    CompleteSignIn = '[Auth] Complete SignIn',
    CompleteSignOut = '[Auth] Complete SignOut',
    SignOutSuccess = '[Auth] SignOut Success'
}

export class SignIn implements Action {
    readonly type = AuthActionTypes.SignIn;
}

export class SignInSuccess implements Action {
    readonly type = AuthActionTypes.SignInSuccess;
  
    constructor(public payload: { user: OidcUser }) {}
}

export class SignInFailure implements Action {
    readonly type = AuthActionTypes.SignInFailure;

    constructor(public payload: { error: Error }) {}
}

export class SignOut implements Action {
    readonly type = AuthActionTypes.SignOut;
}

export class CompleteSignIn implements Action {
    readonly type = AuthActionTypes.CompleteSignIn;
}

export class CompleteSignOut implements Action {
    readonly type = AuthActionTypes.CompleteSignOut;
}

export class SignOutSuccess implements Action {
    readonly type = AuthActionTypes.SignOutSuccess;

    constructor(public payload: { fromCallback: boolean }) {}
}

export type AuthActionsUnion =
  | SignIn
  | SignInSuccess
  | SignInFailure
  | CompleteSignIn
  | SignOut
  | CompleteSignOut
  | SignOutSuccess;