import { Injectable } from '@angular/core';
import { BreakEnterModel } from './full-note/content-editor-services/break-enter.model';

export interface SaveCursorPosition {
  startOffset: number;
  endOffset: number;
  startContainer: number[];
  endContainer: number[];
}

@Injectable({
  providedIn: 'root',
})
export class ApiBrowserTextService {
  pasteCommandHandler = (e) => {
    e.preventDefault();
    let text = (e.originalEvent || e).clipboardData.getData('text/plain');
    text = text.replace(/&nbsp;/g, '');

    const range = this.getSelection().getRangeAt(0);
    range.deleteContents();
    const textNode = document.createTextNode(text);
    range.insertNode(textNode);
    range.selectNodeContents(textNode);
    range.collapse(false);
    const selection = this.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);
  };

  copyInputLink(input: HTMLInputElement) {
    const text = input.value;
    this.copyTest(text);
  }

  copyTest = (text) => {
    navigator.clipboard.writeText(text).then(
      () => {
        /* clipboard successfully set */
      },
      () => {
        /* clipboard write failed */
      },
    );
  };

  getSelection = () => {
    return window.getSelection();
  };

  getSelectionCharacterOffsetsWithin = (element) => {
    if (!element) return null;

    let startOffset = 0;
    let endOffset = 0;
    const selection = this.getSelection();

    if (selection.type === 'None') {
      return null;
    }

    const range = this.getSelection().getRangeAt(0);
    const preCaretRange = range.cloneRange();
    preCaretRange.selectNodeContents(element);
    preCaretRange.setEnd(range.startContainer, range.startOffset);
    startOffset = preCaretRange.toString().length;
    endOffset = startOffset + range.toString().length;
    return { start: startOffset, end: endOffset };
  };

  pressEnterHandler(e): BreakEnterModel {
    let isFocusToNext = false;
    let nextContent: DocumentFragment;
    let nextHtml: string;
    const el = e as Node;
    const sel = this.getSelection();
    if (sel.rangeCount) {
      const selRange = sel.getRangeAt(0);
      const range = selRange.cloneRange();

      range.selectNodeContents(el);
      range.setEnd(selRange.startContainer, selRange.startOffset);
      const atStart = range.toString().replace(/^\s+|\s+$/g, '') === '';
      // eslint-disable-next-line no-empty
      if (atStart) {
      }

      range.selectNodeContents(el);
      range.setStart(selRange.endContainer, selRange.endOffset);
      const atEnd = range.toString().replace(/^\s+|\s+$/g, '') === '';
      if (atEnd) {
        isFocusToNext = true;
      }

      if (!atStart && !atEnd) {
        isFocusToNext = true;
        range.selectNodeContents(el);
        range.setStart(selRange.endContainer, selRange.startOffset);
        nextContent = range.extractContents();
        nextHtml = (nextContent.firstChild as HTMLElement).innerHTML;
      }
    }
    return { isFocusToNext, nextHtml, nextContent };
  }

  // eslint-disable-next-line consistent-return
  isStart(element) {
    const sel = this.getSelection();
    if (sel.rangeCount) {
      const selRange = sel.getRangeAt(0);
      const range = selRange.cloneRange();
      range.selectNodeContents(element);
      range.setEnd(selRange.startContainer, selRange.startOffset);
      const atStart = range.toString().replace(/^\s+|\s+$/g, '') === '';
      if (atStart) {
        return true;
      }
      return false;
    }
  }

  setCursor(e: HTMLElement, isStart: boolean): void {
    if (!e) return;
    const range = document.createRange(); // Create a range (a range is a like the selection but invisible)
    range.selectNodeContents(e); // Select the entire contents of the element with the range
    range.collapse(isStart); // collapse the range to the end point. false means collapse to end rather than the start
    this.updateRange(range);
  }

  updateRange(range: Range): void {
    const selection = this.getSelection();
    selection.removeAllRanges(); // remove any selections already made
    selection.addRange(range); // make the range you have just created the visible selection
  }

  setCaret(el: Node, pos: number): void {
    if (!el) return;
    const range = document.createRange();
    range.setStart(el.childNodes[0], pos);
    range.collapse(true);
    this.updateRange(range);
  }

  saveRangePosition(bE: HTMLElement): SaveCursorPosition {
    const range = window.getSelection().getRangeAt(0);
    let sC = range.startContainer;
    let eC = range.endContainer;

    const A: number[] = [];
    while (sC !== bE) {
      A.push(this.getNodeIndex(sC));
      sC = sC.parentNode;
    }
    const B: number[] = [];
    while (eC !== bE) {
      B.push(this.getNodeIndex(eC));
      eC = eC.parentNode;
    }

    const obj: SaveCursorPosition = {
      startContainer: A,
      endContainer: B,
      startOffset: range.startOffset,
      endOffset: range.endOffset,
    };

    return obj;
  }

  restoreRangePosition(bE: any, data: SaveCursorPosition) {
    bE.focus();
    const sel = this.getSelection();
    const range = sel.getRangeAt(0);
    let x;
    let C;
    let sC = bE;
    let eC = bE;

    C = data.startContainer;
    x = C.length;
    while (x--) sC = sC.childNodes[C[x]];
    C = data.endContainer;
    x = C.length;
    while (x--) eC = eC.childNodes[C[x]];

    range.setStart(sC, data.startOffset);
    range.setEnd(eC, data.endOffset);
    sel.removeAllRanges();
    sel.addRange(range);
  }

  private getNodeIndex(n): number {
    let i = 0;
    while ((n = n.previousSibling)) i++;
    return i;
  }
}
