import { Component, OnInit, Output, EventEmitter, ElementRef, Renderer2, ViewChild } from '@angular/core';
import { interval } from 'rxjs';

@Component({
  selector: 'app-chat-message-compose',
  templateUrl: './chat-message-compose.component.html',
  styleUrls: ['./chat-message-compose.component.css'],

})
export class ChatMessageComposeComponent implements OnInit {

  @Output()
  messageSubmitted = new EventEmitter<string>();

  @ViewChild("textField")
  textField: ElementRef;

  get content(): string {
    return this.textField.nativeElement.innerText;
  }

  set content(c: string) {
    this.textField.nativeElement.innerText = c;
  }

  _test = false;
  get test() {
    return this._test;
  }
  set test(val: boolean) {
    this._test = val;
  }

  constructor() {   
  }

  ngOnInit() {
    interval(2000).subscribe(x => this.test = !this.test);
  }

  onSubmit() {
    this.messageSubmitted.emit(this.content);
    this.content = "";
  }

  get buttonDisabled(): boolean {
    return this.content === "";
  }
}
