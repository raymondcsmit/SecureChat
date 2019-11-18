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
import { ChatGuardService } from '../chat/services/chat-guard.service';
import { SetGlobalBusyInterceptorService } from './services/set-global-busy-interceptor.service';
import { ErrorComponent } from './components/error/error.component';


export const COMPONENTS = [
  AppComponent,
  FooterComponent,
  NotFoundPageComponent,
  ErrorComponent
]

@NgModule({
  declarations: COMPONENTS,
  imports: [
    CommonModule,
    BrowserAnimationsModule,
    HttpClientModule,
    StoreModule.forRoot(reducers, { metaReducers }),
    StoreRouterConnectingModule.forRoot({
      stateKey: 'router'
    }),
    EffectsModule.forRoot([]),
    AppRoutingModule,
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
