import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  constructor(private store: Store<any>) { }
}
