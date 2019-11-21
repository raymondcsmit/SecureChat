import { NgModule } from '@angular/core';
import { ChatRoutingModule } from './chat-routing.module';
import { StoreModule } from '@ngrx/store';
import { reducers } from './reducers';
import { EffectsModule } from '@ngrx/effects';
import { ChatEffects } from './effects/chat.effects';
import { HeaderComponent } from './components/header/header.component';
import { MaterialModule } from '../material/material.module';
import { FriendsControlPanelComponent } from './containers/friends-control-panel/friends-control-panel.component';
import { ChatControlPanelComponent } from './containers/chat-control-panel/chat-control-panel.component';
import { AddFriendComponent } from './containers/add-friend/add-friend.component';
import { CreateChatroomComponent } from './containers/create-chatroom/create-chatroom.component';
import { InviteFriendComponent } from './containers/invite-friend/invite-friend.component';
import { ChatRootComponent } from './containers/chat-root/chat-root.component';
import { ChatService } from './services/chat.service';
import { UserSearchComponent } from './components/user-search/user-search.component';
import { UserSearchResultComponent } from './components/user-search-result/user-search-result.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserEffects } from './effects/user.effects';
import { ChatGuardService } from './services/chat-guard.service';
import { PersonalInfoComponent } from './containers/personal-info/personal-info.component';
import { ConnectFormDirective } from '../core/directives/connect-form.directive';

@NgModule({
  imports: [
    ChatRoutingModule,
    StoreModule.forFeature('chatModule', reducers),
    EffectsModule.forFeature([ChatEffects, UserEffects]),
    MaterialModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  entryComponents: [
    InviteFriendComponent
  ],
  declarations: [
    ChatRootComponent, 
    HeaderComponent, 
    FriendsControlPanelComponent, 
    //ChatroomComponent, 
    //ChatroomMembersComponent, 
    //ChatMessagesComponent, 
    //ChatMessageComposeComponent, 
    //MessageComponent,
    //ChatTabComponent, 
    ChatControlPanelComponent, 
    AddFriendComponent, 
    //ChatsComponent, 
    AddFriendComponent, 
    CreateChatroomComponent, 
    InviteFriendComponent, 
    ChatControlPanelComponent,
    //ChatroomsComponent,
    //PrivateChatsComponent,
    UserSearchComponent,
    UserSearchResultComponent,
    PersonalInfoComponent,
    ConnectFormDirective
  ],
  providers: [
    ChatService
  ]
})
export class ChatModule {}
