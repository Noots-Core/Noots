﻿using System;
using Common.CQRS;
using MediatR;

namespace Domain.Commands.Labels
{
    public class NewLabelCommand : BaseCommandEntity, IRequest<Guid>
    {
        public NewLabelCommand(Guid userId)
            :base(userId)
        {

        }
    }
}
