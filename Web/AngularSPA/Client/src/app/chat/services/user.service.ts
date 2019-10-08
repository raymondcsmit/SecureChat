import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../models/User';
import { UserQuery } from '../models/UserQuery';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  searchUsers(query: UserQuery): Observable<any> {
    throw new Error("Method not implemented.");
  }

  constructor() { }
}
