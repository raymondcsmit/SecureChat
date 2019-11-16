import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { mergeMap, tap, take, finalize } from 'rxjs/operators';
import { SetGlobalBusy } from '../actions/actions';

@Injectable({
  providedIn: 'root'
})
export class SetGlobalBusyInterceptorService implements HttpInterceptor {

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.includes("api")) {
      return this.store.pipe(
        take(1),
        tap(_ => this.store.dispatch(new SetGlobalBusy({value: true}))),
        mergeMap(_ => next.handle(req)),
        finalize(() => this.store.dispatch(new SetGlobalBusy({value: false})))
      )
    }
    
    return next.handle(req);
  }
  
  constructor(private store: Store<any>) { }

}
