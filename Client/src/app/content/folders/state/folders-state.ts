import { Folder } from '../models/folder';
import { State, Selector, Action, StateContext } from '@ngxs/store';
import { Injectable } from '@angular/core';
import { ApiFoldersService } from '../api-folders.service';
import { OrderService } from 'src/app/shared/services/order.service';
import { FullFolder } from '../models/FullFolder';
import { LoadPrivateFolders, LoadSharedFolders, LoadArchiveFolders,
    LoadDeletedFolders, LoadAllFolders, AddFolder, SelectIdFolder,
    UnSelectIdFolder, UnSelectAllFolder, SelectAllFolder } from './folders-actions';
import { tap } from 'rxjs/operators';
import { FolderColorPallete } from 'src/app/shared/enums/FolderColors';
import { FolderType } from 'src/app/shared/enums/FolderTypes';


interface FolderState {
    privateFolders: Folder[];
    sharedFolders: Folder[];
    deletedFolders: Folder[];
    archiveFolders: Folder[];
    countPrivate: number;
    countShared: number;
    countDeleted: number;
    countArchive: number;
    privateLoaded: boolean;
    sharedLoaded: boolean;
    archiveLoaded: boolean;
    deletedLoaded: boolean;
    selectedIds: string[];
}


@State<FolderState>({
    name: 'Folders',
    defaults: {
        privateFolders: [],
        sharedFolders: [],
        deletedFolders: [],
        archiveFolders: [],
        countPrivate: 0,
        countShared: 0,
        countArchive: 0,
        countDeleted: 0,
        privateLoaded: false,
        sharedLoaded: false,
        archiveLoaded: false,
        deletedLoaded: false,
        selectedIds: []
    }
})

@Injectable()
export class FolderStore {

    constructor(private api: ApiFoldersService,
                private orderService: OrderService) {
    }

    // Get folders
    @Selector()
    static privateFolders(state: FolderState): Folder[] {
        return state.privateFolders;
    }

    @Selector()
    static sharedFolders(state: FolderState): Folder[] {
        return state.sharedFolders;
    }

    @Selector()
    static deletedFolders(state: FolderState): Folder[] {
        return state.deletedFolders;
    }

    @Selector()
    static archiveFolders(state: FolderState): Folder[] {
        return state.archiveFolders;
    }


    // Get count folders
    @Selector()
    static privateCount(state: FolderState): number {
        return state.countPrivate;
    }

    @Selector()
    static archiveCount(state: FolderState): number {
        return state.countArchive;
    }

    @Selector()
    static deletedCount(state: FolderState): number {
        return state.countDeleted;
    }

    @Selector()
    static sharedCount(state: FolderState): number {
        return state.countShared;
    }

    // Get selected Ids

    @Selector()
    static selectedIds(state: FolderState): string[] {
        return state.selectedIds;
    }


    // LOAD CONTENT
    @Action(LoadPrivateFolders)
    loadPrivateFolders({ getState, patchState }: StateContext<FolderState>) {
        if (!getState().privateLoaded) {
            return this.api.getPrivateFolders().pipe(tap(content => {
                patchState({
                    privateFolders: content,
                    privateLoaded: true,
                    countPrivate: content.length
                });
            }));
        }
    }

    @Action(LoadSharedFolders)
    loadSharedFolders({ getState, patchState }: StateContext<FolderState>) {
        if (!getState().sharedLoaded) {
            return this.api.getSharedFolders().pipe(tap(content => {
                patchState({
                    sharedFolders: content,
                    sharedLoaded: true,
                    countShared: content.length
                });
            }));
        }
    }

    @Action(LoadArchiveFolders)
    loadArchiveFolders({ getState, patchState }: StateContext<FolderState>) {
        if (!getState().archiveLoaded) {
            return this.api.getArchiveFolders().pipe(tap(content => {
                patchState({
                    archiveFolders: content,
                    archiveLoaded: true,
                    countArchive: content.length
                });
            }));
        }
    }

    @Action(LoadDeletedFolders)
    loadDeletedFolders({ getState, patchState }: StateContext<FolderState>) {
        if (!getState().deletedLoaded) {
            return this.api.getDeletedFolders().pipe(tap(content => {
                patchState({
                    deletedFolders: content,
                    deletedLoaded: true,
                    countDeleted: content.length
                });
            }));
        }
    }

    @Action(LoadAllFolders)
    async loadAllFolders({ dispatch }: StateContext<FolderState>) {
        dispatch([LoadPrivateFolders, LoadSharedFolders, LoadArchiveFolders, LoadDeletedFolders]);
    }


    // FUNCTIONS
    @Action(AddFolder)
    async newFolder({ getState, patchState }: StateContext<FolderState>) {
        const id = await this.api.new().toPromise();
        const folders = getState().privateFolders;
        patchState({
            privateFolders: [{ id, title: '', color: FolderColorPallete.Green }, ...folders],
            countPrivate: getState().countPrivate + 1
        });
    }

    // SELECTIONS

    @Action(SelectIdFolder)
    select({ patchState, getState }: StateContext<FolderState>, { id }: SelectIdFolder) {
        const ids = getState().selectedIds;
        patchState({ selectedIds: [id, ...ids] });
    }

    @Action(UnSelectIdFolder)
    unSelect({ getState, patchState }: StateContext<FolderState>, { id }: UnSelectIdFolder) {
        let ids = getState().selectedIds;
        ids = ids.filter(x => x !== id);
        patchState({ selectedIds: [...ids] });
    }

    @Action(UnSelectAllFolder)
    unselectAll({ patchState, getState }: StateContext<FolderState>) {
        patchState({ selectedIds: [] });
    }

    @Action(SelectAllFolder)
    selectAll({ patchState, getState }: StateContext<FolderState>, { typeNote }: SelectAllFolder) {
        let ids;
        switch (typeNote) {
            case FolderType.Archive: {
                ids = getState().archiveFolders.map(x => x.id);
                break;
            }
            case FolderType.Private: {
                ids = getState().privateFolders.map(x => x.id);
                break;
            }
            case FolderType.Deleted: {
                ids = getState().deletedFolders.map(x => x.id);
                break;
            }
            case FolderType.Shared: {
                ids = getState().sharedFolders.map(x => x.id);
                break;
            }
        }
        patchState({ selectedIds: [...ids] });
    }
}
