import {
    ActionReducerMap,
    ActionReducer,
    MetaReducer,
    createSelector,
  } from '@ngrx/store';
import { environment } from '../../../environments/environment';
import * as fromRouter from '@ngrx/router-store';
import { storeFreeze } from 'ngrx-store-freeze';
import { selectGlobalBusy } from './app.reducer';
import * as fromCore from './app.reducer';
export * from './entity.reducer';

export interface State {
    router: fromRouter.RouterReducerState;
    core: fromCore.State;
}

export const reducers: ActionReducerMap<State> = {
    router: fromRouter.routerReducer,
    core: fromCore.reducer
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

export const selectCoreState = createSelector(
  (state: State) => state.core
);

export const getGlobalBusy = createSelector(
    selectCoreState,
    selectGlobalBusy
);