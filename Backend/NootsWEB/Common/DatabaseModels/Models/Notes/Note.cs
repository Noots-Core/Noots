﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Common.DatabaseModels.Models.Folders;
using Common.DatabaseModels.Models.History;
using Common.DatabaseModels.Models.Labels;
using Common.DatabaseModels.Models.NoteContent;
using Common.DatabaseModels.Models.NoteContent.FileContent;
using Common.DatabaseModels.Models.NoteContent.TextContent;
using Common.DatabaseModels.Models.Systems;
using Common.DatabaseModels.Models.Users;
using Common.DatabaseModels.Models.WS;
using Common.Interfaces;
using Common.Interfaces.Note;

namespace Common.DatabaseModels.Models.Notes
{
    [Table(nameof(Note), Schema = SchemeConfig.Note)]
    public class Note : BaseEntity<Guid>, IDateCreator, IDateUpdater, IDateDeleter, IBaseNote
    {
        public NoteTypeENUM NoteTypeId { set; get; }
        public NoteType NoteType { set; get; }

        public RefTypeENUM RefTypeId { set; get; }
        public RefType RefType { set; get; }

        public string Title { set; get; }
        public string Color { set; get; }
        public int Order { set; get; }

        [NotMapped]
        public bool IsLocked { get => !string.IsNullOrEmpty(Password); }

        public string Password { set; get; }

        public DateTimeOffset? UnlockTime { set; get; }

        public Guid UserId { set; get; }
        public User User { set; get; }

        public CacheNoteHistory CacheNoteHistory { set; get; }

        public List<UserOnPrivateNotes> UsersOnPrivateNotes { set; get; }
        public List<LabelsNotes> LabelsNotes { get; set; }
        public List<FoldersNotes> FoldersNotes { set; get; }
        public List<RelatedNoteToInnerNote> ReletatedNoteToInnerNotesFrom { set; get; }
        public List<RelatedNoteToInnerNote> ReletatedNoteToInnerNotesTo { set; get; }
        public List<BaseNoteContent> Contents { set; get; }
        public List<NoteSnapshot> History { set; get; }

        public List<NoteConnection> NoteConnections { set; get; }

        public List<TextNoteIndex> TextNoteIndexs { set; get; }

        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public  DateTimeOffset CreatedAt { get; set; }

        public IEnumerable<TextNote> GetTextContents()
        {
            return Contents?.Where(x => x.ContentTypeId == ContentTypeENUM.Text).Select(x => x as TextNote);
        }

        public IEnumerable<CollectionNote> GetCollectionContents()
        {
            return Contents?.Where(x => x.ContentTypeId == ContentTypeENUM.Collection).Select(x => x as CollectionNote);
        }

        public void ToType(NoteTypeENUM noteTypeId, DateTimeOffset? deletedAt = null)
        {
            DeletedAt = deletedAt;
            NoteTypeId = noteTypeId;
            UpdatedAt = DateTimeProvider.Time;
        }

        public bool IsShared() => NoteTypeId == NoteTypeENUM.Shared;
    }
}
