import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Chat, chatListSchema, chatSchema } from '../models/Chat';
import { ArrayResult } from 'src/app/core/models/ArrayResult';
import { normalize } from 'normalizr';
import { UserEntity } from 'src/app/user/entities/UserEntity';
import { ConfigurationService } from 'src/app/core/services/configuration.service';
import { ChatEntity } from '../entities/ChatEntity';

@Injectable({
  providedIn: 'root'
})
export class ChatsService {

  constructor(
    private httpClient: HttpClient, 
    private configurationService: ConfigurationService) { }

  getChats() {
    const chatsApi = this.configurationService.getAppSettings().chatsApiUrl;
    const url = `${chatsApi}/chats`;

    return this.httpClient.get<ArrayResult<Chat>>(url, {observe: 'response'}).pipe(
      map(res => {
        if (res.body.items.length == 0) {
          return {
            chats: [],
            users: []
          };
        }
        const normalized = normalize(res.body.items, chatListSchema);
        return {
          chats: Object.values(normalized.entities.chats) as ChatEntity[],
          users: Object.values(normalized.entities.users) as UserEntity[]
        };
      }),
      catchError(res => throwError(this.resolveErrors(res)))
    );    
  }

  createChat(userId: string, chatName: string, chatCapacity: number) {
    const chatsApi = this.configurationService.getAppSettings().chatsApiUrl;
    const url = `${chatsApi}/chats`;

    const data = {ownerId: userId, name: chatName, capacity: chatCapacity};
    return this.httpClient.post<Chat>(url, data, {observe: 'response'}).pipe(
      map(res => {
        const normalized = normalize(res.body, chatSchema);
        return {
          chats: Object.values(normalized.entities.chats) as ChatEntity[],
          users: Object.values(normalized.entities.users) as UserEntity[]
        };
      }),
      catchError(res => throwError(this.resolveErrors(res)))
    );
  }

  private resolveErrors(res: HttpErrorResponse) {
    return (res.error && res.error.errors) ? res.error.errors : [res.message];
  }

}
