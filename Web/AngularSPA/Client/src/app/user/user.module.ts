import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreModule } from '@ngrx/store';
import { reducers } from './reducers';
import { EffectsModule } from '@ngrx/effects';
import { UserEffects } from './effects/user.effects';
import { MaterialModule } from '../material/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InviteFriendComponent } from './containers/invite-friend/invite-friend.component';
import { FriendsControlPanelComponent } from './containers/friends-control-panel/friends-control-panel.component';
import { AddFriendComponent } from './containers/add-friend/add-friend.component';
import { UserSearchComponent } from './components/user-search/user-search.component';
import { UserSearchResultComponent } from './components/user-search-result/user-search-result.component';
import { PersonalInfoComponent } from './components/personal-info/personal-info.component';
import { ConnectFormDirective } from '../core/directives/connect-form.directive';
import { UsersService } from './services/users.service';
import { RouterModule } from '@angular/router';
import { AccountService } from './services/account.service';
import { SessionService } from './services/session.service';

@NgModule({
  imports: [
    StoreModule.forFeature('userModule', reducers),
    EffectsModule.forFeature([UserEffects]),
    MaterialModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
  entryComponents: [
    InviteFriendComponent
  ],
  declarations: [
    FriendsControlPanelComponent, 
    AddFriendComponent, 
    AddFriendComponent, 
    InviteFriendComponent, 
    UserSearchComponent,
    UserSearchResultComponent,
    PersonalInfoComponent,
    ConnectFormDirective
  ],
  exports: [
    FriendsControlPanelComponent, 
    AddFriendComponent, 
    AddFriendComponent, 
    InviteFriendComponent, 
    UserSearchComponent,
    UserSearchResultComponent,
    PersonalInfoComponent,
    StoreModule
  ],
  providers: [
    UsersService,
    AccountService,
    SessionService
  ]
})
export class UserModule { }
