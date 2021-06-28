﻿using System;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;
using Common.DatabaseModels.Models.NoteContent.ContentParts;
using Common.DTO.Notes.FullNoteContent;
using MediatR;

namespace Domain.Commands.NoteInner
{
    public class InsertLineCommand : BaseCommandEntity, IRequest<OperationResult<TextNoteDTO>>
    {
        [ValidationGuid]
        public Guid NoteId { set; get; }

        [ValidationGuid]
        public Guid ContentId { set; get; }

        [Required]
        public string LineBreakType { set; get; }

        public string NextText { set; get; }

        [RequiredEnumField(ErrorMessage = "NoteTextType is required.")]
        public NoteTextTypeENUM NoteTextType { set; get; }
    }
}
