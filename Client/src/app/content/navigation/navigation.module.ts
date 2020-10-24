import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SideBarComponent } from './side-bar/side-bar.component';
import { HeaderComponent } from './header/header.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';
import { MenuComponent } from './menu/menu.component';
import { ProfileSectionComponent } from './header-components/left-section-components/profile-section/profile-section.component';
import { RightSectionComponent } from './header-components/right-section-componets/right-section/right-section.component';
import { SearchComponent } from './header-components/right-section-componets/search/search.component';
import { ButtonThemeComponent } from './header-components/right-section-componets/button-theme/button-theme.component';
import { ButtonQuestionComponent } from './header-components/right-section-componets/button-question/button-question.component';
import { ButtonNotificationComponent } from './header-components/right-section-componets/button-notification/button-notification.component';
import { ButtonChangeViewComponent } from './header-components/right-section-componets/button-change-view/button-change-view.component';
import { HamburgMenuComponent } from './header-components/inner-section-components/hamburg-menu/hamburg-menu.component';
import { NewButtonComponent } from './header-components/inner-section-components/new-button/new-button.component';
import { ButtonSelectAllComponent } from './header-components/inner-section-components/button-select-all/button-select-all.component';
import { ButtonUnselectAllComponent } from './header-components/inner-section-components/button-unselect-all/button-unselect-all.component';
import { ButtonFilterComponent } from './header-components/inner-section-components/button-filter/button-filter.component';
// tslint:disable-next-line:max-line-length
import { ButtonDefaultBackgroundComponent } from './header-components/inner-section-components/button-default-background/button-default-background.component';


@NgModule({
  declarations: [SideBarComponent, HeaderComponent, MenuComponent,
    ProfileSectionComponent, RightSectionComponent,
    SearchComponent, ButtonThemeComponent, ButtonQuestionComponent,
    ButtonNotificationComponent, ButtonChangeViewComponent,
    HamburgMenuComponent, NewButtonComponent, ButtonSelectAllComponent, ButtonUnselectAllComponent,
    ButtonFilterComponent, ButtonDefaultBackgroundComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule
  ],
  exports: [SideBarComponent, HeaderComponent],
  providers: []
})
export class NavigationModule { }
