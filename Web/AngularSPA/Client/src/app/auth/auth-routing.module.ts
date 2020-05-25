import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginPageComponent } from './containers/login-page/login-page.component';
import { LoginPageGuardService } from './services/login-page-guard.service';
import { SignInCallbackComponent } from './containers/sign-in-callback/sign-in-callback.component';
import { SignOutCallbackComponent } from './containers/sign-out-callback/sign-out-callback.component';
import { SignInSilentCallbackComponent } from './containers/sign-in-silent-callback/sign-in-silent-callback.component';

export const routes: Routes = [
  {
    path: '',
    children: [
      { path: 'login', component: LoginPageComponent, canActivate: [LoginPageGuardService] },
      { path: 'sign-in-callback', component: SignInCallbackComponent },
      { path: 'sign-in-silent-callback', component: SignInSilentCallbackComponent },
      { path: 'sign-out-callback', component: SignOutCallbackComponent },
    ]
  }  
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class AuthRoutingModule { }
