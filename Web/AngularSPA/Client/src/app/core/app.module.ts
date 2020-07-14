import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { EffectsModule } from '@ngrx/effects';
import { reducers, metaReducers } from './reducers';
import { StoreModule, Store } from '@ngrx/store';
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
import { ConfigurationService } from './services/configuration.service';
import { SignalrService } from './services/signalr.service';

export function initializeApp(configService: ConfigurationService) {
  return () => configService.loadSettings().toPromise();
}

@NgModule({
  declarations: [
    AppComponent,
    FooterComponent,
    NotFoundPageComponent,
    ErrorComponent
  ],
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
    ConfigurationService,
    SignalrService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: SetGlobalBusyInterceptorService,
      multi: true
    },
    { 
      provide: APP_INITIALIZER,
      useFactory: initializeApp, 
      deps: [ConfigurationService, HttpClient, Store], 
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
