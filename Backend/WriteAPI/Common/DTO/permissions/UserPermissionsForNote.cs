﻿using Common.DatabaseModels.models.Notes;
using Common.DatabaseModels.models.Users;

namespace Common.DTO.permissions
{
    public class UserPermissionsForNote
    {
        public User User { set; get; }
        public Note Note { set; get; }
        public bool CanRead { set; get; }
        public bool CanWrite { set; get; }
        public bool UserNotFound { set; get; }
        public bool NoteNotFound { set; get; }
        public bool IsOwner { set; get; }

        public UserPermissionsForNote SetFullAccess(User user, Note note, bool isOwner)
        {
            User = user;
            Note = note;
            CanRead = true;
            CanWrite = true;
            IsOwner = isOwner;
            return this;
        }

        public UserPermissionsForNote SetOnlyRead(User user, Note note)
        {
            User = user;
            Note = note;
            CanRead = true;
            CanWrite = false;
            return this;
        }

        public UserPermissionsForNote SetNoAccessRights(User user, Note note)
        {
            User = user;
            Note = note;
            CanRead = false;
            CanWrite = false;
            return this;
        }

        public UserPermissionsForNote SetUserNotFounded()
        {
            UserNotFound = true;
            return this;
        }

        public UserPermissionsForNote SetNoteNotFounded()
        {
            NoteNotFound = true;
            return this;
        }

    }
}
