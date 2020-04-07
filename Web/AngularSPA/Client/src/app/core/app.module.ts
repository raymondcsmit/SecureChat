import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { EffectsModule } from '@ngrx/effects';
import { reducers, metaReducers } from './reducers';
import { StoreModule } from '@ngrx/store';
import { StoreRouterConnectingModule } from '@ngrx/router-store';
import { NotFoundPageComponent } from './components/not-found-page/not-found-page.component';
import { AppComponent } from './containers/app/app.component';
import { FooterComponent } from './components/footer/footer.component';
import { AppRoutingModule } from './app-routing.module';
import { AuthModule } from '../auth/auth.module';
import { MaterialModule } from '../material/material.module';
import { ChatGuardService } from '../chat-root/services/chat-guard.service';
import { SetGlobalBusyInterceptorService } from './services/set-global-busy-interceptor.service';
import { ErrorComponent } from './components/error/error.component';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from '../../environments/environment';
import { ConnectFormDirective } from './directives/connect-form.directive';
import { LoginPageGuardService } from '../auth/services/login-page-guard.service';
import { ChatRootModule } from '../chat-root/chat-root.module';

export const COMPONENTS = [
  AppComponent,
  FooterComponent,
  NotFoundPageComponent,
  ErrorComponent
]

@NgModule({
  declarations: COMPONENTS,
  imports: [
    AppRoutingModule,
    CommonModule,
    BrowserAnimationsModule,
    HttpClientModule,
    StoreModule.forRoot(reducers, { metaReducers }),
    StoreDevtoolsModule.instrument({
      maxAge: 100,
      logOnly: environment.production, // Restrict extension to log-only mode
    }),
    StoreRouterConnectingModule.forRoot({
      stateKey: 'router'
    }),
    EffectsModule.forRoot([]),
    AuthModule.forRoot(),
    MaterialModule
  ],
  providers: [
    ChatGuardService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: SetGlobalBusyInterceptorService,
      multi: true
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
