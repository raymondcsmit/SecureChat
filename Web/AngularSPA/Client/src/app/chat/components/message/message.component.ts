// import { Component, OnInit, Input } from '@angular/core';
// import { Message } from '../../models/Message';
// import { User } from 'src/app/user/models/User';

// @Component({
//   selector: 'app-message',
//   templateUrl: './message.component.html',
//   styleUrls: ['./message.component.css']
// })
// export class MessageComponent implements OnInit {

//   @Input()
//   message: Message;
//   @Input()
//   author: User;
//   @Input()
//   myId: string;

//   constructor() { }

//   ngOnInit() {
//   }

//   getTimeString() {
//     const date = new Date(this.message.createdAt);
//     return `${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`;
//   }

//   get color() {
//     return this.author.id === this.myId ? 'red' : 'blue';
//   }
// }
