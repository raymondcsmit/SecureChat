import { CoreActionTypes, CoreActionsUnion } from '../actions/core.actions';

export interface State {
    busy: boolean[];
}

export const initialState: State = {
    busy: [],
}

export function reducer(state = initialState, action: CoreActionsUnion): State {
    switch (action.type) {
        case CoreActionTypes.SetGlobalBusy:
            let busy = [...state.busy];
            if (action.payload.value) {
                busy.push(true);
            }
            else {
                busy.pop();
            }
            return {
                ...state,
                busy: busy
            };
        default:
            return state;
    }
}

export const selectGlobalBusy = (state: State) => state.busy.length > 0;