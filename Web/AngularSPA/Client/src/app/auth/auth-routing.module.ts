import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginPageComponent } from './containers/login-page/login-page.component';
import { LoginPageGuardService } from './services/login-page-guard.service';
import { SignInCallbackComponent } from './containers/sign-in-callback/sign-in-callback.component';
import { SignOutCallbackComponent } from './containers/sign-out-callback/sign-out-callback.component';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      { path: 'login', component: LoginPageComponent, canActivate: [LoginPageGuardService] },
      { path: 'sign-in-callback', component: SignInCallbackComponent },
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
