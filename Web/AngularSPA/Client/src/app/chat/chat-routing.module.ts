import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ChatRootComponent } from './containers/chat-root/chat-root.component';

export const routes: Routes = [
  { 
    path: '', 
    component: ChatRootComponent,
    pathMatch: 'full'
    // children: [
    //   { path: 'chats', 
    //     component: ChatsComponent, 
    //     pathMatch: 'full'
    //   }
    // ]
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
