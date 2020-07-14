import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { ArrayResult } from 'src/app/core/models/ArrayResult';
import { Query } from 'src/app/core/models/Query';
import { User } from '../models/User';
import { friendshipRequestSchema, FriendshipRequest, friendshipRequestListSchema } from '../models/FriendshipRequest';
import { normalize } from 'normalizr';
import { FriendshipRequestEntity } from '../entities/FriendshipRequestEntity';
import { UserEntity } from '../entities/UserEntity';
import { Friendship, friendshipListSchema } from '../models/Friendship';
import { FriendshipEntity } from '../entities/FriendshipEntity';
import { Session } from '../models/Session';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private usersApi = environment.usersApi;

  constructor(private httpClient: HttpClient) { }

  getSelf() {
    const url = `${this.usersApi}/users/me`;
    return this.httpClient.get<User>(url, {observe: 'response'}).pipe(
      map(res => res.body),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  updateUser(id: string, patch: any) {
    const url = `${this.usersApi}/users/${id}`;
    return this.httpClient.patch<User>(url, patch, {observe: 'response'}).pipe(
      map(_ => true),
      catchError(res => throwError(this.resolveErrors(res)))
    )
  }

  getUsers(query: Query<User>) {
    const url = `${this.usersApi}/users`;
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

  addFriend(requesterId: string, requesteeId: string) {
    const url = `${this.usersApi}/users/${requesterId}/friendship-requests`;
    const formData = new FormData();
    formData.append('requesteeId', requesteeId);
    return this.httpClient.post<any>(url, formData, {observe: 'response'}).pipe(
      map(res => {
        const normalized = normalize(res.body, friendshipRequestSchema);
        return {
          friendshipRequest: Object.values(normalized.entities.friendshipRequests)[0] as FriendshipRequestEntity,
          requestee: normalized.entities.users[requesteeId] as UserEntity
        };
      }),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  getFriendshipRequests(requesteeId: string) {
    const url = `${this.usersApi}/users/${requesteeId}/friendship-requests`;
    return this.httpClient.get<ArrayResult<FriendshipRequest>>(url, {observe: 'response'}).pipe(
      map(res => {
        if (res.body.items.length == 0) {
          return {
            friendshipRequests: [],
            requesters: []
          };
        }
        const normalized = normalize(res.body.items, friendshipRequestListSchema);
        return {
          friendshipRequests: Object.values(normalized.entities.friendshipRequests) as FriendshipRequestEntity[],
          requesters: Object.values(normalized.entities.users).filter(u => u.id !== requesteeId) as UserEntity[]
        };
      }),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }
  
  updateFriendshipRequest(id: string, status: string) {
    const url = `${this.usersApi}/friendship-requests/${id}`;
    return this.httpClient.patch<User>(url, {outcome: status}, {observe: 'response'}).pipe(
      map(_ => true),
      catchError(res => throwError(this.resolveErrors(res)))
    )
  }

  getFriendships(userId: string) {
    const url = `${this.usersApi}/users/${userId}/friendships`;
    return this.httpClient.get<ArrayResult<Friendship>>(url, {observe: 'response'}).pipe(
      map(res => {
        if (res.body.items.length == 0) {
          return {
            friendships: [],
            friends: []
          };
        }
        const normalized = normalize(res.body.items, friendshipListSchema);
        return {
          friendships: Object.values(normalized.entities.friendships) as FriendshipEntity[],
          friends: Object.values(normalized.entities.users).filter(u => u.id !== userId) as UserEntity[]
        };
      }),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  deleteFriendship(userId, friendId) {
    const url = `${this.usersApi}/users/${userId}/friendships`;
    let queryParams = {
      friendId: friendId
    };

    return this.httpClient.delete(url, {observe: 'response', params: queryParams}).pipe(
      map(res => true),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  private resolveErrors(res: HttpErrorResponse) {
    return (res.error && res.error.errors) ? res.error.errors : [res.message];
  }
}
