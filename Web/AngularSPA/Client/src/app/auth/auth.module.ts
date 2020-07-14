import { NgModule, ModuleWithProviders } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginPageComponent } from './containers/login-page/login-page.component';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule, routes } from './auth-routing.module';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { reducers } from './reducers';
import { ChatGuardService } from '../chat-root/services/chat-guard.service';
import { AuthEffects } from './effects/auth.effects';
import { UserManager } from 'oidc-client';
import { environment } from '../../environments/environment';
import { LoginPageGuardService } from './services/login-page-guard.service';
import { SignInCallbackComponent } from './containers/sign-in-callback/sign-in-callback.component';
import { SignOutCallbackComponent } from './containers/sign-out-callback/sign-out-callback.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AccessTokenInterceptorService } from './services/access-token-interceptor.service';
import { AuthorizationErrorResponseInterceptorService } from './services/authorization-error-response-interceptor.service';
import { MaterialModule } from '../material/material.module';
import { SignInSilentCallbackComponent } from './containers/sign-in-silent-callback/sign-in-silent-callback.component';

@NgModule({
  imports: [
    AuthRoutingModule,
    CommonModule,
    ReactiveFormsModule,
    StoreModule.forFeature('auth', reducers),
    EffectsModule.forFeature([AuthEffects]),
    MaterialModule
  ],
  declarations: [SignInCallbackComponent, LoginPageComponent, SignOutCallbackComponent, SignInSilentCallbackComponent]
})
export class AuthModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: AuthModule, 
      providers: [
        LoginPageGuardService,
        { 
          provide: UserManager,
          useValue: new UserManager(
            {
              authority: environment.authorityUrl,
              client_id: environment.clientId,
              redirect_uri: `${window.location.origin}/auth/sign-in-callback`,
              post_logout_redirect_uri: `${window.location.origin}/auth/sign-out-callback`,
              automaticSilentRenew: true,
              silent_redirect_uri: `${window.location.origin}/auth/sign-in-silent-callback`,
              response_type:"id_token token",
              scope:"openid profile account users messaging session permissions email",
              filterProtocolClaims: true,
              loadUserInfo: true,
              checkSessionInterval: 2000
            }
          )
        },
        {
          provide: HTTP_INTERCEPTORS,
          useClass: AccessTokenInterceptorService,
          multi: true
        },
        {
          provide: HTTP_INTERCEPTORS,
          useClass: AuthorizationErrorResponseInterceptorService,
          multi: true
        }
      ]
    };
  }
}
