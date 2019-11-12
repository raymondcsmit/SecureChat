import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../models/User';
import { UserQuery } from '../models/UserQuery';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private usersApi = environment.usersApi;

  getUsers(query: UserQuery): Observable<any> {
    throw new Error("Method not implemented.");
  }

  getSelf() {
    const url = `${this.usersApi}/users/me`;
    return this.httpClient.get<User>(url, {observe: 'response'}).pipe(
      map(res => res.body)
    );
  }

  constructor(private httpClient: HttpClient) { }
}
