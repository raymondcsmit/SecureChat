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

export const selectedSignedIn = (state: State) => state.oidcUser != null && !state.oidcUser.expired;
export const selectOidcUser = (state: State) => state.oidcUser;
export const selectUser = (state: State) => {
  return {
    id: state.oidcUser.profile.sub,
    userName: state.oidcUser.profile.preferred_username,
    email: state.oidcUser.profile.email,
    isOnline: true
  };
}