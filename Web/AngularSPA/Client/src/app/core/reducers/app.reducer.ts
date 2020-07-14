import { CoreActionTypes, CoreActionsUnion } from '../actions/core.actions';
import { AppSettings } from '../models/AppSettings';
import { environment } from 'src/environments/environment';

export interface State {
    busy: boolean[];
    appSettings: AppSettings
}

export const initialState: State = {
    busy: [],
    appSettings: {
        accountApiUrl: environment.accountApi,
        usersApiUrl: environment.usersApi,
        authUrl: environment.authorityUrl,
        messagingUrl: environment.messagingUrl,
        clientId: environment.clientId,
        sessionApiUrl: environment.sessionApi,
        chatsApiUrl: environment.chatsApi
    }
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
        case CoreActionTypes.SetAppConfiguration:
            return {
                ...state,
                appSettings: {
                    ...state.appSettings,
                    ...action.payload.appSettings
                }
            };
        default:
            return state;
    }
}

export const selectGlobalBusy = (state: State) => state.busy.length > 0;
export const selectAppSettings = (state: State) => state.appSettings;