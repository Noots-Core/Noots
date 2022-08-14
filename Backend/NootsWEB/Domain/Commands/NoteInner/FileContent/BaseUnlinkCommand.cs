﻿using Common.Attributes;
using Common.CQRS;
using Common.DTO;
using MediatR;
using System;
using System.Collections.Generic;

namespace Domain.Commands.NoteInner.FileContent
{
    public class BaseUnlinkCommand : BaseCommandEntity, IRequest<OperationResult<Unit>>
    {
        [ValidationGuid]
        public Guid NoteId { set; get; }

        [ValidationGuid]
        public List<Guid> ContentIds { set; get; }

        public bool IsCheckPermissions { set; get; } = true;

        public BaseUnlinkCommand(Guid noteId, List<Guid> contentIds, Guid userId)
        {
            this.NoteId = noteId;
            this.ContentIds = contentIds;
            this.UserId = userId;
        }
    }
}
