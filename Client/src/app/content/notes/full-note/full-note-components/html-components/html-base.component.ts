import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ThemeENUM } from 'src/app/shared/enums/theme.enum';
import { BaseText } from '../../../models/editor-models/base-text';
import { HeadingTypeENUM } from '../../../models/editor-models/text-models/heading-type.enum';
import { NoteTextTypeENUM } from '../../../models/editor-models/text-models/note-text-type.enum';
import { TextBlock } from '../../../models/editor-models/text-models/text-block';
import { BreakEnterModel } from '../../content-editor-services/models/break-enter.model';
import { DeltaConverter } from '../../content-editor/converter/delta-converter';
import { DeltaListEnum } from '../../content-editor/converter/entities/delta-list.enum';
import { EnterEvent } from '../../models/enter-event.model';
import { ComponentType, ParentInteractionHTML } from '../../models/parent-interaction.interface';
import { SetFocus } from '../../models/set-focus';
import { TransformContent } from '../../models/transform-content.model';
import { BaseEditorElementComponent } from '../base-html-components';
import { HtmlComponentsFacadeService } from '../html-components-services/html-components.facade.service';
import { InputHtmlEvent } from './models/input-html-event';

export interface PasteEvent {
  element: BaseTextElementComponent;
  htmlElementsToInsert: HTMLElement[];
}
@Component({
  template: '',
})
// eslint-disable-next-line @angular-eslint/component-class-suffix
export abstract class BaseTextElementComponent
  extends BaseEditorElementComponent
  implements ParentInteractionHTML
{
  @Input()
  content: BaseText;

  @Output()
  enterEvent = new EventEmitter<EnterEvent>();

  @Output()
  transformTo = new EventEmitter<TransformContent>();

  @Output()
  concatThisWithPrev = new EventEmitter<ParentInteractionHTML>();

  @Output()
  pasteEvent = new EventEmitter<PasteEvent>();

  @Output()
  deleteThis = new EventEmitter<string>();

  @Output()
  // eslint-disable-next-line @angular-eslint/no-output-on-prefix
  onFocus = new EventEmitter<BaseTextElementComponent>();

  @Output()
  inputHtmlEvent = new EventEmitter<InputHtmlEvent>();

  @ViewChild('contentHtml') contentHtml: ElementRef;

  type = ComponentType.HTML;

  preFocus = false;

  textChanged: Subject<string> = new Subject();

  destroy = new Subject<void>();

  viewHtml: string;

  listeners = [];

  constructor(cdr: ChangeDetectorRef, facade: HtmlComponentsFacadeService) {
    super(cdr, facade);

    this.textChanged.pipe(takeUntil(this.destroy)).subscribe(() => {
      if (!this.contentHtml) return;
      this.inputHtmlEvent.emit({
        content: this.content,
        html: this.contentHtml.nativeElement.innerHTML,
      });
    });
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  theme: ThemeENUM;

  get isActiveState(): boolean {
    return this.getIsActive() && !this.isReadOnlyMode;
  }

  onInput() {
    this.syncHtmlWithLayout();
  }

  syncHtmlWithLayout() {
    this.textChanged.next();
  }

  getText(): string {
    return this.getEditableNative<HTMLElement>().textContent;
  }

  initBaseHTML(): void {
    if (this.content.contents?.length > 0) {
      const html = DeltaConverter.convertTextBlocksToHTML(this.content.contents);
      this.facade.sanitizer.bypassSecurityTrustHtml(html);
      const convertedHTML = this.facade.sanitizer.bypassSecurityTrustHtml(html) ?? '';
      this.viewHtml = convertedHTML as string;
      this.syncHtmlWithLayout();
    }
  }

  transformContent($event, contentType: NoteTextTypeENUM, heading?: HeadingTypeENUM) {
    $event?.preventDefault();
    this.transformTo.emit({
      textType: contentType,
      headingType: heading,
      contentId: this.content.id,
      setFocusToEnd: true,
    });
  }

  // emits changes for (ctrl + z)
  updateHTML(contents: TextBlock[], emitChanges: boolean): void {
    // TODO TEST IT
    this.transformOnUpdate(contents);
    const html = DeltaConverter.convertTextBlocksToHTML(contents);
    this.updateNativeHTML(html);
    if (emitChanges) {
      this.syncHtmlWithLayout();
    }
  }

  transformOnUpdate(contents: TextBlock[]): void {
    const content = contents?.find((x) => x.list !== null);
    if (content?.list) {
      if (content.list === DeltaListEnum.bullet) {
        let type = NoteTextTypeENUM.dotList;
        if (content.text?.startsWith('[ ]')) {
          content.text = content.text.slice(3);
          type = NoteTextTypeENUM.checkList;
        }
        this.transformContent(null, type);
      }
      if (content.list === DeltaListEnum.ordered) {
        this.transformContent(null, NoteTextTypeENUM.numberList);
      }
    }
    const headingType = contents?.find((x) => x.header !== null)?.header;
    if (headingType) {
      this.transformContent(null, NoteTextTypeENUM.heading, this.getHeadingNumber(headingType));
    }
  }

  getHeadingNumber(heading: number): HeadingTypeENUM {
    if (heading >= 3 && heading <= 4) {
      return HeadingTypeENUM.H2;
    }
    if (heading >= 5 && heading <= 6) {
      return HeadingTypeENUM.H3;
    }
    return HeadingTypeENUM.H1;
  }

  syncLayoutWithContent(emitChanges: boolean) {
    const el = this.contentHtml.nativeElement;
    const savedSel = this.facade.apiBrowserTextService.saveSelection(el);
    this.updateHTML(this.content.contents, emitChanges);
    this.facade.apiBrowserTextService.restoreSelection(el, savedSel);
  }

  updateWS(): void {
    const el = this.contentHtml.nativeElement;
    const savedSel = this.facade.apiBrowserTextService.saveSelection(el);
    const html = DeltaConverter.convertTextBlocksToHTML(this.content.contents);
    this.updateNativeHTML(html);
    this.facade.apiBrowserTextService.restoreSelection(el, savedSel);
    this.detectChanges();
  }

  getContent(): BaseText {
    return this.content;
  }

  getContentId(): string {
    return this.content.id;
  }

  getEditableNative<T>() {
    return this.contentHtml?.nativeElement as T;
  }

  getTextBlocks(): TextBlock[] {
    const html = this.getEditableNative<HTMLElement>().innerHTML;
    return DeltaConverter.convertHTMLToTextBlocks(html);
  }

  isContentEmpty() {
    return this.contentHtml?.nativeElement?.textContent.length === 0;
  }

  getIsActive() {
    return (
      (this.isContentEmpty() && document.activeElement === this.contentHtml.nativeElement) ||
      (this.preFocus && this.isContentEmpty())
    );
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  mouseEnter($event) {
    $event.preventDefault();
    this.preFocus = !this.isSelectModeActive;
    this.isMouseOver = true;
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  mouseLeave($event) {
    this.preFocus = false;
    this.isMouseOver = false;
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  setFocus($event) {
    this.contentHtml.nativeElement.focus();
    this.setFocusedElement();
    this.onFocus.emit(this);
  }

  setFocusToEnd() {
    this.facade.apiBrowserTextService.setCursor(this.contentHtml.nativeElement, false);
    this.setFocusedElement();
    this.onFocus.emit(this);
  }

  // LISTENERS
  isPasteLink(data: DataTransferItemList): boolean {
    for (const item of data as any) {
      if ((item as DataTransferItem).type === 'text/link-preview') {
        return true;
      }
    }
    return false;
  }

  async pasteCommandHandler(e: ClipboardEvent) {
    const isLink = this.isPasteLink(e.clipboardData.items);
    e.preventDefault();
    if (isLink) {
      this.convertTextToLink(e);
      this.textChanged.next();
      return;
    }
    const html = e.clipboardData.getData('text/html');
    if (html) {
      this.handleHtmlInserting(html);
      this.textChanged.next();
      return;
    }
    const text = e.clipboardData.getData('text/plain');
    if (text) {
      this.facade.apiBrowserTextService.pasteOnlyTextHandler(e);
      this.textChanged.next();
      return;
    }
  }

  handleHtmlInserting(html: string): void {
    const htmlElements = DeltaConverter.splitDeltaByDividers(html);
    if (htmlElements.length === 0) return;

    const htmlEl = htmlElements[0];
    this.facade.apiBrowserTextService.pasteHTMLHandler(htmlEl); // TODO DONT MUTATE ELEMENT
    const editableEl = this.getEditableNative<HTMLElement>().cloneNode(true) as HTMLElement;
    const resTextBlocks = DeltaConverter.convertHTMLToTextBlocks(editableEl.innerHTML);
    this.updateHTML(resTextBlocks, true);
    htmlElements.shift(); // remove first element

    if (htmlElements.length > 0) {
      const pasteObject: PasteEvent = {
        element: this,
        htmlElementsToInsert: htmlElements,
      };
      this.pasteEvent.emit(pasteObject);
    }
  }

  convertTextToLink(e: ClipboardEvent) {
    const json = e.clipboardData.getData('text/link-preview') as any;
    const data = JSON.parse(json);
    const title = data.title;
    const url = data.url;
    const pos = this.facade.apiBrowserTextService.getSelectionCharacterOffsetsWithin(
      this.getEditableNative(),
    );
    const html = DeltaConverter.convertTextBlocksToHTML(this.content.contents);
    const resultDelta = DeltaConverter.insertLink(html, pos.start, title, url);
    const resTextBlocks = DeltaConverter.convertDeltaToTextBlocks(resultDelta);
    this.updateHTML(resTextBlocks, true);
  }

  checkForDeleteOrConcatWithPrev($event) {
    if (this.facade.selectionService.isAnySelect()) {
      return;
    }

    const selection = this.facade.apiBrowserTextService.getSelection().toString();
    if (
      this.facade.apiBrowserTextService.isStart(this.getEditableNative()) &&
      !this.isContentEmpty() &&
      selection === ''
    ) {
      $event.preventDefault();
      this.concatThisWithPrev.emit(this);
    }

    if (this.isContentEmpty()) {
      $event.preventDefault();
      this.deleteThis.emit(this.content.id);
    }
  }

  setHandlers() {
    const el = this.getEditableNative();
    const blur = this.facade.renderer.listen(el, 'blur', (e) => {
      this.onBlur(e);
    });
    const paste = this.facade.renderer.listen(el, 'paste', (e) => {
      this.pasteCommandHandler(e);
    });
    const selectStart = this.facade.renderer.listen(el, 'selectstart', (e) => {
      this.onSelectStart(e);
    });
    const keydownEnter = this.facade.renderer.listen(el, 'keydown.enter', (e) => {
      this.enter(e);
    });
    const keydownBackspace = this.facade.renderer.listen(el, 'keydown.backspace', (e) => {
      this.checkForDeleteOrConcatWithPrev(e);
    });
    const keyupBackspace = this.facade.renderer.listen(el, 'keyup.backspace', (e) => {
      this.backUp(e);
    });
    const keydownDelete = this.facade.renderer.listen(el, 'keydown.delete', (e) => {
      this.checkForDeleteOrConcatWithPrev(e);
    });
    this.listeners.push(
      blur,
      paste,
      selectStart,
      keydownBackspace,
      keydownEnter,
      keyupBackspace,
      keydownDelete,
    );
  }

  destroysListeners() {
    for (const destroyFunc of this.listeners) {
      destroyFunc();
    }
  }

  eventEventFactory(
    breakModel: BreakEnterModel,
    nextItemType: NoteTextTypeENUM,
    contentId: string,
  ): EnterEvent {
    const eventModel: EnterEvent = {
      breakModel,
      nextItemType,
      contentId,
    };
    return eventModel;
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  backUp(e) {}

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onBlur(e) {}

  textClick(e: PointerEvent): void {
    const target = e.target as HTMLAnchorElement;
    if (target.localName === 'a' && target.href) {
      e.preventDefault();
      window.open(target.href, '_blank');
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onSelectStart(e) {}

  private updateNativeHTML(html: string): void {
    this.contentHtml.nativeElement.innerHTML = html;
  }

  abstract enter(e);

  abstract setFocusedElement(): void;

  abstract isFocusToNext(entity?: SetFocus);
  abstract getHost();
  abstract backspaceUp();
  abstract backspaceDown();
  abstract deleteDown();
}
