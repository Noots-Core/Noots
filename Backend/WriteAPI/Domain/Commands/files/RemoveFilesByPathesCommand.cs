﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Commands.files
{
    public class RemoveFilesByPathesCommand : IRequest<Unit>
    {
        public List<string> Pathes { set; get; }
        public RemoveFilesByPathesCommand(List<string> Pathes)
        {
            this.Pathes = Pathes;
        }
    }
}
