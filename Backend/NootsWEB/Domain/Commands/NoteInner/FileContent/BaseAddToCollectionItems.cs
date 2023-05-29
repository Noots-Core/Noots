﻿using Common.Attributes;
using Common.CQRS;
using Common.DTO;
using Domain.Commands.NoteInner.FileContent.Texts.Entities;
using MediatR;
using System;
using System.Collections.Generic;


namespace Domain.Commands.NoteInner.FileContent;

public class BaseAddToCollectionItems : BaseCommandEntity, IRequest<OperationResult<UpdateCollectionContentResult>>
{
    [ValidationGuid]
    public Guid NoteId { set; get; }

    [ValidationGuid]
    public Guid ContentId { set; get; }

    [ValidationGuid]
    public List<Guid> FileIds { set; get; }

    public BaseAddToCollectionItems(Guid noteId, Guid contentId, List<Guid> fileIds)
    {
        NoteId = noteId;
        ContentId = contentId;
        FileIds = fileIds;
    }
}
