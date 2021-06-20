import { RefTypeENUM } from "src/app/shared/enums/refTypeEnum";

export interface InvitedUsersToNoteOrFolder {
  id: string;
  photoId: string;
  photoPath: string;
  name: string;
  email: string;
  accessTypeId: RefTypeENUM;
}
