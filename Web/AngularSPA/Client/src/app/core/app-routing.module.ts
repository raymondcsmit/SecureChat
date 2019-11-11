import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundPageComponent } from './components/not-found-page/not-found-page.component';
import { ChatGuardService } from '../chat/services/chat-guard.service';

export const routes: Routes = [
  {
    path: '', redirectTo: '/chat', pathMatch: 'full'
  },
  { 
    path: 'chat',
    loadChildren: '../chat/chat.module#ChatModule',
    canActivate: [ChatGuardService]
  },
]

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
