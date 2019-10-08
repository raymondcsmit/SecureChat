import * as fromAuth from './auth.reducer';
import * as fromRoot from '../../core/reducers';
import { ActionReducerMap, createFeatureSelector, createSelector } from '@ngrx/store';

export interface AuthState {
    status: fromAuth.State,
}

export interface State extends fromRoot.State {
    auth: AuthState
}

export const reducers: ActionReducerMap<AuthState> = {
    status: fromAuth.reducer,
};

export const selectAuthState = createFeatureSelector<AuthState>('auth');

export const selectAuthStatusState = createSelector(
    selectAuthState,
    (state: AuthState) => state.status
);

export const getSignedIn = createSelector(
    selectAuthStatusState,
    fromAuth.selectedSignedIn
);

export const getUser = createSelector(
    selectAuthStatusState, 
    fromAuth.selectUser
);

export const getOidcUser = createSelector(
    selectAuthStatusState,
    fromAuth.selectOidcUser
);