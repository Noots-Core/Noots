import { Injectable } from '@angular/core';
import { BaseText, ContentModel } from './models/ContentMode';

@Injectable()
export class MenuSelectionService {

  public currentItem: ContentModel<BaseText>;
  public menuActive = false;
  public startTop = 0;
  public startScroll = 0;
  public currentScroll = 0;
  public left = 0;
  constructor() { }

  get getTop()
  {
    const positionNumber = this.startTop + this.startScroll - this.currentScroll;
    /*
    if (positionNumber < 0)
    {
      return 5;
    }*/
    return positionNumber;
  }
}
