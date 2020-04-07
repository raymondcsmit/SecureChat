import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-chats',
  templateUrl: './chats.component.html',
  styleUrls: ['./chats.component.css'],
  host: {
    class: 'd-flex flex-grow-1'
  }
})
export class ChatsComponent implements OnInit {

  ngOnInit() {
  }
  
}
