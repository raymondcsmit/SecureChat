import { NgModule } from '@angular/core';
import { StoreModule } from '@ngrx/store';
import { reducers } from './reducers';
import { EffectsModule } from '@ngrx/effects';
import { ChatEffects } from './effects/chat.effects';
import { MaterialModule } from '../material/material.module';
import { ChatControlPanelComponent } from './containers/chat-control-panel/chat-control-panel.component';
import { CreateChatroomComponent } from './containers/create-chatroom/create-chatroom.component';
import { InviteFriendComponent } from '../user/containers/invite-friend/invite-friend.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserModule } from '../user/user.module';

@NgModule({
  imports: [
    StoreModule.forFeature('chatModule', reducers),
    EffectsModule.forFeature([ChatEffects]),
    MaterialModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    UserModule
  ],
  entryComponents: [
    InviteFriendComponent
  ],
  declarations: [
    //ChatroomComponent, 
    //ChatroomMembersComponent, 
    //ChatMessagesComponent, 
    //ChatMessageComposeComponent, 
    //MessageComponent,
    //ChatTabComponent, 
    ChatControlPanelComponent, 
    //ChatsComponent, 
    //CreateChatroomComponent, 
    //ChatroomsComponent,
    //PrivateChatsComponent,
  ],
  exports: [
    StoreModule,
    ChatControlPanelComponent
  ],
  providers: [
  ]
})
export class ChatModule {}
