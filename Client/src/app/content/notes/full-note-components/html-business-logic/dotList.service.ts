import { ElementRef, EventEmitter, Injectable } from '@angular/core';
import { ContentModel, ContentType, DotList, HtmlText } from '../../models/ContentMode';
import { EnterEvent } from '../../models/enterEvent';
import { HtmlService } from './html.service';

@Injectable()
export class DotListService extends HtmlService {

    onInput(content: ContentModel<DotList>, contentHtml: ElementRef) {
        content.data.content = contentHtml.nativeElement.innerText;
    }

    onBlur(e: any) {
        // BLUR HANDLER
    }

    pasteCommandHandler(e: any) {
        throw new Error('Method not implemented.');
    }

    mouseUp(e: any) {
        super.mouseUp(e);
    }

    onSelectStart(e: any) {
        // SELECTIION
    }

    enter($event: any, content: ContentModel<DotList>, contentHtml: ElementRef, enterEvent: EventEmitter<EnterEvent>) {
        $event.preventDefault();
        const breakModel = this.contEditService.enterService(this.getNativeElement(contentHtml));
        content.data.content = contentHtml.nativeElement.innerText;
        const event = super.eventEventFactory(content.contentId, breakModel, ContentType.DOTLIST);
        enterEvent.emit(event);
    }

    backDown(e: any) {
        throw new Error('Method not implemented.');
    }

    backUp(e: any) {
        throw new Error('Method not implemented.');
    }

}
