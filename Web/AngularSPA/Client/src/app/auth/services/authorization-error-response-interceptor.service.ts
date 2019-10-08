import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Store } from '@ngrx/store';
import * as fromAuth from '../reducers';
import * as fromAuthActions from '../actions/auth.actions';
import { tap, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationErrorResponseInterceptorService {

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(event => {
        if (event instanceof HttpErrorResponse && (event.status == 401 || event.status == 403)) {
          this.store.dispatch(new fromAuthActions.SignInFailure({ error: event.error }));
        }
        return throwError(event);
      })
    )
  }
  
  constructor(private store: Store<fromAuth.State>) { }

}
