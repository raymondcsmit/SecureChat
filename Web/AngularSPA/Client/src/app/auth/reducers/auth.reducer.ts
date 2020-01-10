import { User as OidcUser } from 'oidc-client';
import { AuthActionsUnion, AuthActionTypes } from "../actions/auth.actions";

export interface State {
    oidcUser: OidcUser;
}

export const initialState: State = {
    oidcUser: null,
}

export function reducer(state = initialState, action: AuthActionsUnion): State {
    switch (action.type) {
        case AuthActionTypes.SignInSuccess: {
          return {
            ...state,
            oidcUser: action.payload.user
          };
        }
        
        case AuthActionTypes.SignOutSuccess: {
          return initialState;
        }

        default: {
          return state;
        }
      }
}

export const selectSignedIn = (state: State) => state.oidcUser != null && !state.oidcUser.expired;
export const selectOidcUser = (state: State) => state.oidcUser;
export const selectId = (state: State) => state.oidcUser.profile.sub;
export const selectPermissions = (state: State) => selectOidcUser(state).profile.permission as string[];
export const selectEmailConfirmed = (state: State) => selectOidcUser(state).profile.email_confirmed != null