import { Action } from '@ngrx/store';

export enum CoreActionTypes {
    SetGlobalBusy = '[Root] Set Global Busy',
    NoOp = '[Root] NoOp',
    Success = '[Root] Success',
    Failure = '[Root] Failure',
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

    constructor(public payload: {action: Action}) {}
}

export class Failure implements Action {
    readonly type = CoreActionTypes.Failure;

    constructor(public payload: {action: Action}) {}
}

export type CoreActionsUnion = 
    SetGlobalBusy
    | NoOp
    | Success
    | Failure;