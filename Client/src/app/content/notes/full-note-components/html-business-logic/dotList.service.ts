import { ElementRef, EventEmitter } from '@angular/core';
import { ApiBrowserTextService } from '../../api-browser-text.service';
import { LineBreakType } from '../../html-models';
import { MenuSelectionService } from '../../menu-selection.service';
import { ContentModel, ContentType, Html, HtmlType } from '../../models/ContentMode';
import { EnterEvent } from '../../models/enterEvent';
import { SelectionService } from '../../selection.service';
import { HtmlCommandsAbstract } from './command-interface';
import { TextService } from './default-text.service';

export class DotListService extends HtmlCommandsAbstract {

    defaultEmptyString = 'List';

    constructor(
         apiBrowserService: ApiBrowserTextService,
         selectionService: SelectionService,
         menuSelectionService: MenuSelectionService,
         contentHtml: ElementRef,
         content: ContentModel<Html>,
    ) {
        super(apiBrowserService, selectionService, menuSelectionService, contentHtml, content);
    }

    abstractonBlur(e: any) {
        throw new Error('Method not implemented.');
    }

    getContentChild(contentHtml: ElementRef): any {
        return contentHtml.nativeElement.children[0].children[1];
    }

    enter(emitter: EventEmitter<EnterEvent>, eventModel: EnterEvent) {
        eventModel.itemType = HtmlType.DOTLIST;
        emitter.emit(eventModel);
    }

    backDown(e: any) {
        throw new Error('Method not implemented.');
    }

    backUp(e: any) {
        throw new Error('Method not implemented.');
    }

}
