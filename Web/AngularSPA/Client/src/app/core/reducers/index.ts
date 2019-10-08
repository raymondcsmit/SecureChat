import {
    ActionReducerMap,
    ActionReducer,
    MetaReducer,
  } from '@ngrx/store';
import { environment } from '../../../environments/environment';
import * as fromRouter from '@ngrx/router-store';
import { storeFreeze } from 'ngrx-store-freeze';

export interface State {
    router: fromRouter.RouterReducerState;
}

export const reducers: ActionReducerMap<State> = {
    router: fromRouter.routerReducer,
};

export function logger(reducer: ActionReducer<State>): ActionReducer<State> {
    return function(state: State, action: any): State {
      console.log('state', state);
      console.log('action', action);
  
      return reducer(state, action);
    };
}

export const metaReducers: MetaReducer<State>[] = !environment.production
  ? [logger, storeFreeze]
  : [];