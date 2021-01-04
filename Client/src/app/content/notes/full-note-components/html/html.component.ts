import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ApiBrowserTextService } from '../../api-browser-text.service';
import { ContentEditableService } from '../../content-editable.service';
import { LineBreakType } from '../../html-models';
import { ContentModel, Html } from '../../models/ContentMode';
import { SelectionService } from '../../selection.service';

@Component({
  selector: 'app-html',
  templateUrl: './html.component.html',
  styleUrls: ['./html.component.scss']
})
export class HtmlComponent implements OnInit, AfterViewInit {

  @ViewChild('contentHtml') contentHtml: ElementRef;

  visible = false;

  @Output()
  enterEvent = new EventEmitter<{ id: string, typeBreak: LineBreakType, html?: DocumentFragment }>();

  @Output()
  deleteThis = new EventEmitter<string>();

  @Output()
  concatThisWithPrev = new EventEmitter<string>();

  @Input()
  content: ContentModel<Html>;

  constructor(private contEditService: ContentEditableService,
              private apiBrowserService: ApiBrowserTextService,
              private selectionService: SelectionService) { }


  ngAfterViewInit(): void {
    this.contentHtml.nativeElement.innerHTML = this.content.data.html;
  }

  ngOnInit(): void {
  }


  buttonHandler($event) {
    $event.preventDefault();
  }

  preventClick($event) {
    $event.preventDefault();
  }

  async onInput(event) {
    this.content.data.html = this.contentHtml.nativeElement.innerHTML;
    this.visible = this.isContentEmpty();
    this.textClearing();
  }

  updateHTML(html: string)
  {
    this.content.data.html = html;
    this.contentHtml.nativeElement.innerHTML = html;
  }

  onSelect($event) {

  }

  pasteCommandHandler(e) {
    this.apiBrowserService.pasteCommandHandler(e);
  }

  async enter($event) {
    $event.preventDefault();
    const model = this.contEditService.enterService(this.contentHtml);
    this.content.data.html = this.contentHtml.nativeElement.innerHTML;
    this.enterEvent.emit({ id: this.content.contentId, typeBreak: model.typeBreakLine, html: model.nextContent });
  }

  async backDown($event: KeyboardEvent) {

    const selection = window.getSelection().toString();
    if (this.contEditService.isStart(this.contentHtml) && !this.isContentEmpty() && selection === '') {
      $event.preventDefault();
      this.concatThisWithPrev.emit(this.content.contentId);
    }

    if (this.isContentEmpty()) {
      this.deleteThis.emit(this.content.contentId);
    }

  }


  async backUp($event: KeyboardEvent) {
    if (this.isContentOneSymbol()) {
      this.visible = true;
      this.contentHtml.nativeElement.innerHTML = '';
    }
  }


  textClearing() {
    if (this.isContentEmpty()) {
      this.contentHtml.nativeElement.innerHTML = '';
    }
  }

  // PLACEHOLDER VISIBLE
  isContentEmpty() {
    return this.contentHtml.nativeElement.textContent.length === 0;
  }
  isContentOneSymbol() {
    return this.contentHtml.nativeElement.textContent === ' ';
  }

  mouseEnter($event) {
    this.visible = true && this.isContentEmpty() && !this.selectionService.ismousedown;
  }

  mouseOut($event) {
    this.visible = (document.activeElement === this.contentHtml.nativeElement) && this.isContentEmpty();
  }

  onBlur($event) {
    this.visible = false;
  }

  setFocus($event?) {
    this.contentHtml.nativeElement.focus();
    this.visible = true && this.isContentEmpty();
  }

  setFocusToEnd() {
    this.contEditService.setCursor(this.contentHtml.nativeElement, false);
    this.visible = true && this.isContentEmpty();
  }

}
