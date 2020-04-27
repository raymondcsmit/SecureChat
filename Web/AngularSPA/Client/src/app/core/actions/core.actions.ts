import { Action } from '@ngrx/store';
import { AppSettings } from '../models/AppSettings';

export enum CoreActionTypes {
    SetGlobalBusy = '[Root] Set Global Busy',
    NoOp = '[Root] NoOp',
    Success = '[Root] Success',
    Failure = '[Root] Failure',
    SetAppConfiguration = '[Root] SetAppConfiguration'
}

export class SetGlobalBusy implements Action {
    readonly type = CoreActionTypes.SetGlobalBusy;

    constructor(public payload: {value: boolean}) {}
}

export class NoOp implements Action {
    readonly type = CoreActionTypes.NoOp;
}

export class Success implements Action {
    readonly type = CoreActionTypes.Success;

    constructor(public payload: {action: Action, data?: any}) {}
}

export class Failure implements Action {
    readonly type = CoreActionTypes.Failure;

    constructor(public payload: {action: Action, errors: string[]}) {}
}

export class SetAppConfiguration implements Action {
    readonly type = CoreActionTypes.SetAppConfiguration;

    constructor(public payload: {appSettings: AppSettings}) {}
}

export type CoreActionsUnion = 
    SetGlobalBusy
    | NoOp
    | Success
    | Failure
    | SetAppConfiguration;