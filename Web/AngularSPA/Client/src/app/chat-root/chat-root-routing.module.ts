import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ChatRootComponent } from './containers/chat-root/chat-root.component';
import { PersonalInfoComponent } from '../user/components/personal-info/personal-info.component';
import { AddFriendComponent } from '../user/containers/add-friend/add-friend.component';
import { CreateChatroomComponent } from '../chat/containers/create-chatroom/create-chatroom.component';

export const routes: Routes = [
  { 
    path: '', 
    component: ChatRootComponent,
    children: [
      // { 
      //   path: 'chats', 
      //   component: ChatsComponent, 
      //   pathMatch: 'full'
      // }
      {
        path: 'personal-info',
        component: PersonalInfoComponent
      },
      {
        path: 'add-friend',
        component: AddFriendComponent
      },
      {
        path: 'create-chatroom',
        component: CreateChatroomComponent
      }
    ]
  }
]

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class ChatRootRoutingModule { }