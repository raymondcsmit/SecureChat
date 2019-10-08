import { Component, OnInit, Input, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-chat-tab',
  templateUrl: './chat-tab.component.html',
  styleUrls: ['./chat-tab.component.css']
})
export class ChatTabComponent implements OnInit {

  @Input()
  chatName: string;

  @Input()
  newMessageCount: number;
  
  @Output()
  chatClosed = new EventEmitter();

  isCloseShown: boolean;

  @ViewChild("closeButtonDiv")
  closeButtonDiv: ElementRef;

  constructor() { }

  ngOnInit() {
  }

  get isBadgeHidden(): boolean {
    return this.newMessageCount == 0;
  }

  onMouseOver() {
    this.isCloseShown = true;
  }

  onMouseOut() {
    this.isCloseShown = false;
  }

  onClick($event: any) {
    let closeButtonRect = this.closeButtonDiv.nativeElement.getBoundingClientRect();
    if ($event.clientX > closeButtonRect.left && $event.clientX < closeButtonRect.right && $event.clientY > closeButtonRect.top && $event.clientY < closeButtonRect.bottom) {
      this.chatClosed.emit();
      $event.stopPropagation();
    }
  }
}
