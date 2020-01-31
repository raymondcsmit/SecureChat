import { Directive, ElementRef, HostListener, Input, TemplateRef } from '@angular/core';

export interface HoverData {
    isHoveredOver: boolean;
}

@Directive({
  selector: '[appHighlight]'
})
export class HoverDirective {

  constructor(private el: ElementRef) { }

  @Input('appHighlight') highlightColor: string;

  @HostListener('mouseenter') onMouseEnter() {
    this.highlight(this.highlightColor || 'red');
  }

  @HostListener('mouseleave') onMouseLeave() {
    this.highlight(null);
  }

  private highlight(color: string) {
    this.el.nativeElement.style.backgroundColor = color;
  }
}