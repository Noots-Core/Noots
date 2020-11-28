﻿using Common.DatabaseModels.helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Commands.users
{
    public class UpdateThemeCommand : BaseCommandEntity, IRequest<Unit>
    {
        [Required]
        public Theme Theme { set; get; }

        public UpdateThemeCommand(Theme Theme, string Email)
            : base(Email)
        {
            this.Theme = Theme;
        }
    }
}
