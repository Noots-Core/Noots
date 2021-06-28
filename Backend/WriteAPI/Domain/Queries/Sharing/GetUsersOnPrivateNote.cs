﻿using System;
using System.Collections.Generic;
using Common.DTO.Users;
using MediatR;

namespace Domain.Queries.Sharing
{
    public class GetUsersOnPrivateNote : BaseQueryEntity, IRequest<List<InvitedUsersToFoldersOrNote>>
    {
        public Guid NoteId { set; get; }
    }
}
