import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Pagination } from 'src/app/core/models/Pagination';
import { ArrayResult } from 'src/app/core/models/ArrayResult';
import { Query } from 'src/app/core/models/Query';
import { User } from '../models/User';
import { buildQueryParams } from 'src/app/core/helpers/buildQueryParams';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private chatApi = environment.chatApi;

  constructor(private httpClient: HttpClient) { }

  getSelf() {
    const url = `${this.chatApi}/users/me`;
    return this.httpClient.get<User>(url, {observe: 'response'}).pipe(
      map(res => res.body),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  updateUser(id: string, patch: any) {
    const url = `${this.chatApi}/users/${id}`;
    return this.httpClient.patch<User>(url, patch, {observe: 'response'}).pipe(
      map(_ => true),
      catchError(res => throwError(this.resolveErrors(res)))
    )
  }

  getUsers(query: Query<User>) {
    const url = `${this.chatApi}/users`;
    let queryParams = {
      query: JSON.stringify(query, (_, value) => {
        if (value !== null && value !== "") return value
      })
    };

    return this.httpClient.get<ArrayResult<User>>(url, {observe: 'response', params: queryParams}).pipe(
      map(res => res.body),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  private resolveErrors(res: HttpErrorResponse) {
    return (res.error && res.error.errors) ? res.error.errors : ["An error has occured"];
  }
}
