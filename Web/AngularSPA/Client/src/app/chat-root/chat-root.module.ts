import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatModule } from '../chat/chat.module';
import { UserModule } from '../user/user.module';
import { ChatRootComponent } from './containers/chat-root/chat-root.component';
import { ChatRootRoutingModule } from './chat-root-routing.module';
import { HeaderComponent } from './containers/header/header.component';
import { MaterialModule } from '../material/material.module';
import { Observable } from 'rxjs';
import { User } from '../user/models/User';
import { Store, select } from '@ngrx/store';
import { getSelf } from '../user/reducers';

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
