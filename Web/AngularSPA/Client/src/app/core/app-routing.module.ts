import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundPageComponent } from './components/not-found-page/not-found-page.component';
import { ChatGuardService } from '../chat-root/services/chat-guard.service';
import { ErrorComponent } from './components/error/error.component';

export const routes: Routes = [
  { 
    path: 'chat',
    loadChildren: '../chat-root/chat-root.module#ChatRootModule',
    canActivate: [ChatGuardService]
  },
  { 
    path: 'auth',
    loadChildren: '../auth/auth.module#AuthModule'
  },
  {
    path: 'error', component: ErrorComponent
  },
  {
    path: '', redirectTo: '/chat', pathMatch: 'full'
  },
  {
    path: '**', component: NotFoundPageComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
