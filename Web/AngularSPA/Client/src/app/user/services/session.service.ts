import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Session } from '../models/Session';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  private sessionApi = environment.sessionApi;

  constructor(private httpClient: HttpClient) { }

  getFriendSessions() {
    const url = `${this.sessionApi}/sessions`;
    return this.httpClient.get<{[index: string]: Session}>(url, {observe: 'response'}).pipe(
      map(res => res.body)
    );
  }

}
