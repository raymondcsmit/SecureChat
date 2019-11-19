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
export class UserService {

  private usersApi = environment.usersApi;

  constructor(private httpClient: HttpClient) { }

  getUsers(query: UserQuery): Observable<any> {
    throw new Error("Method not implemented.");
  }

  getSelf() {
    const url = `${this.usersApi}/users/me`;
    return this.httpClient.get<User>(url, {observe: 'response'}).pipe(
      map(res => res.body),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  confirmEmail(id: string) {
    const url = `${this.usersApi}/users/${id}/confirm-email`;
    return this.httpClient.post<any>(url, {}, {observe: 'response'}).pipe(
      map(_ => true),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  updateUser(id: string, body: Partial<User>) {
    const url = `${this.usersApi}/users/${id}`;
    return this.httpClient.patch<User>(url, body, {observe: 'response'}).pipe(
      map(_ => true),
      catchError(res => throwError(this.resolveErrors(res)))
    )
  }

  private resolveErrors(res: HttpErrorResponse) {
    return (res.error && res.error.errors) ? res.error.errors : ["An error has occured"];
}
}
