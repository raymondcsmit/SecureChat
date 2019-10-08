import { Action } from '@ngrx/store';

export enum CoreActionTypes {
    SetGlobalBusy = '[Root] Set Global Busy',
    NoOp = '[Root] NoOp',
}

export class SetGlobalBusy implements Action {
    readonly type = CoreActionTypes.SetGlobalBusy;

    constructor(public payload: {value: boolean}) {}
}

export class NoOp implements Action {
    readonly type = CoreActionTypes.NoOp;
}

export type CoreActionsUnion = 
    SetGlobalBusy
    | NoOp;