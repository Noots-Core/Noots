﻿using System;
using Common.Attributes;
using Common.DTO.Notes.FullNoteContent;
using MediatR;

namespace Domain.Commands.NoteInner
{
    public class NewLineTextContentNoteCommand : BaseCommandEntity, IRequest<OperationResult<TextNoteDTO>>
    {
        [ValidationGuid]
        public Guid NoteId { set; get; }
    }
}
