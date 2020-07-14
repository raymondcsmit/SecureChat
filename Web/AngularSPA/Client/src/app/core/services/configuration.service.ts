import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Subject, of, Observable, interval, BehaviorSubject } from 'rxjs';
import { AppSettings } from '../models/AppSettings';
import { tap, catchError, map, switchMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Store, select } from '@ngrx/store';
import { SetAppConfiguration } from '../actions/core.actions';
import { getAppSettings } from '../reducers';

const UPDATE_INTERVAL = 300000;

@Injectable()
export class ConfigurationService {

    private appSettings: AppSettings;

    constructor(private http: HttpClient, private store: Store<any>) { }
    
    loadSettings() {
        let url = `${window.location.origin}/api/configuration`;

        const observable = this.http.get<any>(url).pipe(
            tap(appSettings => {
                console.log('Server settings loaded');
                console.log(appSettings);
                this.store.dispatch(new SetAppConfiguration({appSettings: appSettings}));
            }),
            catchError(res => this.handleError(res))
        );

        interval(UPDATE_INTERVAL)
            .pipe(switchMap(_ => observable))
            .subscribe();

        this.store.pipe(
            select(getAppSettings),
            tap(appSettings => this.appSettings = appSettings)
        )
        .subscribe();

        return observable;
    }

    getAppSettings() {
        return this.appSettings;
    }

    private handleError(error: HttpErrorResponse) {
        console.log("Could not load app configuration from server.");
        return of({});
    }
}
