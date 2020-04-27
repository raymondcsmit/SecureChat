import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Subject, of } from 'rxjs';
import { AppSettings } from '../models/AppSettings';
import { tap, catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Store } from '@ngrx/store';
import { SetAppConfiguration } from '../actions/core.actions';

@Injectable()
export class ConfigurationService {

    constructor(private http: HttpClient, private store: Store<any>) { }
    
    load() {
        let url = `${window.location.origin}/api/configuration`;
        return this.http.get<AppSettings>(url).pipe(
            tap(appSettings => {
                console.log('Server settings loaded');
                console.log(appSettings);
                this.store.dispatch(new SetAppConfiguration({appSettings: appSettings}));
            }),
            map(_ => true),
            catchError(res => this.handleError(res))
        );
    }

    private handleError(error: HttpErrorResponse) {
        console.log("Could not load app configuration from server.");
        return of(false);
    }
}
