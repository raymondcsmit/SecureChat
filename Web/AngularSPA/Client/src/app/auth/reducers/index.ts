import * as fromAuth from './auth.reducer';
import * as fromCore from '../../core/reducers';
import { ActionReducerMap, createFeatureSelector, createSelector } from '@ngrx/store';

export interface AuthState {
    status: fromAuth.State,
}

export interface State extends fromCore.State {
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
    fromAuth.selectSignedIn
);

export const getId = createSelector(
    selectAuthStatusState,
    getSignedIn,
    (state, signedIn) => signedIn ? fromAuth.selectId(state) : null
);

export const getOidcUser = createSelector(
    selectAuthStatusState,
    fromAuth.selectOidcUser
);

export const getPermissions = createSelector(
    selectAuthStatusState,
    fromAuth.selectPermissions
)

export const getEmailConfirmed = createSelector(
    selectAuthStatusState,
    fromAuth.selectEmailConfirmed
)