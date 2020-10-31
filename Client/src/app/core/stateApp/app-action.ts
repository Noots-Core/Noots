import { EntityType } from 'src/app/shared/enums/EntityTypes';
import { NoteType } from 'src/app/shared/enums/NoteTypes';


export class UpdateRoute {
    static type = '[App] Update route';
    constructor(public type: EntityType) {    }
}

export class UpdateRouteWithNoteType {
    static type = '[App] Update routeNoteType';
    constructor(public type: EntityType, public noteType: NoteType) {    }
}


export class UpdateMenuActive {
    static type = '[App] Update menu';
    constructor(public flag: boolean) {    }
}


export class SpinnerChangeStatus {
    static type = '[App] Spinner status';
    constructor(public flag: boolean) {    }
}
