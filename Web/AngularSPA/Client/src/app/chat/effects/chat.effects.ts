import { Injectable } from "@angular/core";
import { Actions, Effect, ofType } from "@ngrx/effects";
import * as fromActions from "../actions/chat.actions";
import * as fromChat from '../reducers';
import { tap, exhaustMap, map, withLatestFrom } from 'rxjs/operators';
import { } from 'rxjs';
import { Store } from "@ngrx/store";
import { HttpClient } from "@angular/common/http";
import { apiConfig } from "../apiConfig";

@Injectable()
export class ChatEffects {
    // TODO

    constructor(
        private actions$: Actions,
        private httpClient: HttpClient,
        private store: Store<fromChat.State>
    ) {
    }
}