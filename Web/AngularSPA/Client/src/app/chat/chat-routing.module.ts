import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ChatRootComponent } from './containers/chat-root/chat-root.component';
import { PersonalInfoComponent } from './containers/personal-info/personal-info.component';

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
export class ChatRoutingModule { }
