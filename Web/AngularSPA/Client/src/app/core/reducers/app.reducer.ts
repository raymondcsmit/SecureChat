import { Action } from '@ngrx/store';
import { CoreActionTypes, CoreActionsUnion } from '../actions/actions';

export interface State {
    busy: boolean;
}

export const initialState: State = {
    busy: false,
}

export function reducer(state = initialState, action: CoreActionsUnion): State {
    switch (action.type) {
        case CoreActionTypes.SetGlobalBusy:
            return {
                ...state,
                busy: action.payload.value
            };
        default:
            return state;
    }
}

export const selectGlobalBusy = (state: State) => state.busy;