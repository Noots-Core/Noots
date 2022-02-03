﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;
using Common.DTO;
using MediatR;

namespace Domain.Commands.Notes
{
    public class AddLabelOnNoteCommand : BaseCommandEntity, IRequest<OperationResult<Unit>>
    {
        [ValidationGuid]
        public Guid LabelId { set; get; }

        [RequiredListNotEmptyAttribute]
        public List<Guid> NoteIds { set; get; }
    }
}
