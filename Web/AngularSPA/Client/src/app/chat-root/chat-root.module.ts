import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatModule } from '../chat/chat.module';
import { UserModule } from '../user/user.module';
import { ChatRootComponent } from './containers/chat-root/chat-root.component';
import { ChatRootRoutingModule } from './chat-root-routing.module';
import { HeaderComponent } from './containers/header/header.component';
import { MaterialModule } from '../material/material.module';

@NgModule({
  declarations: [
    ChatRootComponent,
    HeaderComponent
  ],
  imports: [
    ChatRootRoutingModule,
    CommonModule,
    ChatModule,
    UserModule,
    MaterialModule
  ]
})
export class ChatRootModule {
}
