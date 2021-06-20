import { ShortUser } from 'src/app/core/models/short-user';
import { Injectable } from '@angular/core';
import { State, Selector, Action, StateContext } from '@ngxs/store';
import { TranslateService } from '@ngx-translate/core';
import { BackgroundService } from 'src/app/content/profile/background.service';
import { environment } from 'src/environments/environment';
import { SetToken, TokenSetNoUpdate } from '../stateApp/app-action';
import {
  Login,
  Logout,
  ChangeTheme,
  ChangeLanguage,
  ChangeFontSize,
  SetCurrentBackground,
  SetDefaultBackground,
  UpdateUserName,
  UpdateUserPhoto,
  LoadUsedDiskSpace,
} from './user-action';
import { UserAPIService } from '../user-api.service';
import { ThemeENUM } from 'src/app/shared/enums/ThemeEnum';
import { FontSizeENUM } from 'src/app/shared/enums/FontSizeEnum';
import { LanguagesENUM } from 'src/app/shared/enums/LanguagesENUM';

interface UserState {
  user: ShortUser;
  isLogin: boolean;
  memory: number;
}

@State<UserState>({
  name: 'User',
  defaults: {
    user: null,
    isLogin: false,
    memory: 0,
  },
})
@Injectable()
export class UserStore {
  constructor(
    private api: UserAPIService,
    private translateService: TranslateService,
    private backgroundAPI: BackgroundService,
  ) {}

  @Selector()
  static getMemoryBytes(state: UserState): number {
    return state.memory;
  }

  @Selector()
  static getMemoryKBytes(state: UserState): number {
    return state.memory / Math.pow(1024, 1);
  }

  @Selector()
  static getMemoryMBytes(state: UserState): number {
    return state.memory / Math.pow(1024, 2);
  }

  @Selector()
  static getMemoryGBytes(state: UserState): number {
    return state.memory / Math.pow(1024, 3);
  }

  @Selector()
  static getStatus(state: UserState): boolean {
    return state.isLogin;
  }

  @Selector()
  static getUser(state: UserState): ShortUser {
    return state.user;
  }

  @Selector()
  static getUserTheme(state: UserState): ThemeENUM {
    return state.user.themeId;
  }

  @Selector()
  static getUserBackground(state: UserState): string {
    const path = state.user.currentBackground?.photoPath;
    if (path) {
      return `${environment.storage}/${state.user.id}/${escape(path)}`;
    }
    return null;
  }

  @Selector()
  static getUserFontSize(state: UserState): FontSizeENUM {
    return state.user.fontSizeId;
  }

  @Selector()
  static getUserLanguage(state: UserState): LanguagesENUM {
    return state.user.languageId;
  }

  @Action(Login)
  async login({ patchState, dispatch }: StateContext<UserState>, { token, user }: Login) {
    let userdb = await this.api.getUser().toPromise();
    if (userdb === null) {
      userdb = await this.api.newUser(user).toPromise();
      dispatch(new UpdateUserPhoto(user.photo));
    }
    dispatch(new SetToken(token));
    patchState({ user: userdb, isLogin: true });
  }

  @Action(Logout)
  // eslint-disable-next-line class-methods-use-this
  logout({ patchState, dispatch }: StateContext<UserState>) {
    dispatch(new TokenSetNoUpdate());
    patchState({ user: null, isLogin: false });
  }

  @Action(ChangeTheme)
  async changeTheme({ patchState, getState }: StateContext<UserState>, { theme }: ChangeTheme) {
    let { user } = getState();
    await this.api.changeTheme(theme).toPromise();
    user = { ...user, themeId: theme };
    patchState({ user });
  }

  @Action(ChangeLanguage)
  async changeLanguage(
    { patchState, getState }: StateContext<UserState>,
    { language }: ChangeLanguage,
  ) {
    await this.api.changeLanguage(language).toPromise();
    await this.translateService.use(LanguagesENUM[language].toLowerCase()).toPromise();
    patchState({ user: { ...getState().user, languageId: language } });
  }

  @Action(ChangeFontSize)
  async changeFontSize(
    { patchState, getState }: StateContext<UserState>,
    { fontSize }: ChangeFontSize,
  ) {
    let { user } = getState();
    await this.api.changeFontSize(fontSize).toPromise();
    user = { ...user, fontSizeId: fontSize };
    patchState({ user });
  }

  @Action(SetCurrentBackground)
  // eslint-disable-next-line class-methods-use-this
  setCurrent(
    { patchState, getState }: StateContext<UserState>,
    { background }: SetCurrentBackground,
  ) {
    patchState({ user: { ...getState().user, currentBackground: background } });
  }

  @Action(SetDefaultBackground)
  async setDefault({ patchState, getState }: StateContext<UserState>) {
    await this.backgroundAPI.defaultBackground().toPromise();
    patchState({
      user: { ...getState().user, currentBackground: null },
    });
  }

  @Action(UpdateUserName)
  async updateUserName(
    { patchState, getState }: StateContext<UserState>,
    { newName }: UpdateUserName,
  ) {
    await this.api.updateUserName(newName).toPromise();
    patchState({
      user: { ...getState().user, name: newName },
    });
  }

  @Action(UpdateUserPhoto)
  async updateUserPhoto(
    { patchState, getState }: StateContext<UserState>,
    { photo }: UpdateUserPhoto,
  ) {
    const newPhoto = await this.api.updateUserPhoto(photo).toPromise();
    patchState({
      user: { ...getState().user, photoId: newPhoto.id, photoPath: newPhoto.photoPath },
    });
  }

  @Action(LoadUsedDiskSpace)
  async loadUsedDiskSpace({ patchState, getState }: StateContext<UserState>) {
    const memory = await this.api.getMemory().toPromise();
    patchState({ memory: memory.totalSize });
  }
}
