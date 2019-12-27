import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { User } from '../models/User';
import { UserQuery } from '../models/UserQuery';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private accountApi = environment.chatApi;

  constructor(private httpClient: HttpClient) { }

  getUsers(query: UserQuery): Observable<any> {
    throw new Error("Method not implemented.");
  }

  getSelf() {
    const url = `${this.accountApi}/users/me`;
    return this.httpClient.get<User>(url, {observe: 'response'}).pipe(
      map(res => res.body),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  confirmEmail(id: string) {
    const url = `${this.accountApi}/users/${id}/confirm-email`;
    return this.httpClient.post<any>(url, {}, {observe: 'response'}).pipe(
      map(_ => true),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  updateUser(id: string, patch: any) {
    const url = `${this.accountApi}/users/${id}`;
    return this.httpClient.patch<User>(url, patch, {observe: 'response'}).pipe(
      map(_ => true),
      catchError(res => throwError(this.resolveErrors(res)))
    )
  }

  private resolveErrors(res: HttpErrorResponse) {
    return (res.error && res.error.errors) ? res.error.errors : ["An error has occured"];
  }
}
