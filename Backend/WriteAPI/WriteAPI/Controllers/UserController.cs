﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.DatabaseModels.helpers;
using Common.DTO.users;
using Domain;
using Domain.Commands.users;
using Domain.Ids;
using Domain.Models;
using Domain.Queries.users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WriteAPI.ControllerConfig;
using WriteAPI.Services;

namespace WriteAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper mapper;
        public UserController(IMediator _mediator, IMapper mapper)
        {
            this._mediator = _mediator;
            this.mapper = mapper;
        }


        [HttpPost]
        public async Task<ShortUser> Authorize(NewUser user)
        {
            var currentUserEmail = this.GetUserEmail();
            var command = mapper.Map<NewUserCommand>(user);
            command.Email = currentUserEmail;
            await _mediator.Send(command);
            return await _mediator.Send(new GetShortUser(currentUserEmail));
        }


        [HttpGet("short")]
        public async Task<ShortUser> GetShort()
        {
            var currentUserEmail = this.GetUserEmail();
            return await _mediator.Send(new GetShortUser(currentUserEmail));
        }

        [HttpPut("main")]
        public async Task UpdateMainInformation([FromBody]UpdateMainUserInfoCommand info)
        {
            var currentUserEmail = this.GetUserEmail();
            info.Email = currentUserEmail;
            await _mediator.Send(info);
        }

        [HttpPost("photo")]
        public async Task ChangeProfilePhoto(IFormFile photo)
        {
            var email = this.GetUserEmail();
            await _mediator.Send(new UpdatePhotoCommand(photo, email));
        }

        [HttpPost("language")]
        public async Task ChangeLanguage(UpdateLanguageCommand languageCommand)
        {
            var email = this.GetUserEmail();
            languageCommand.Email = email;
            await _mediator.Send(languageCommand);
        }

        [HttpPost("theme")]
        public async Task ChangeTheme(UpdateThemeCommand themeCommand)
        {
            var email = this.GetUserEmail();
            themeCommand.Email = email;
            await _mediator.Send(themeCommand);
        }

        [HttpPost("font")]
        public async Task ChangeFontSize(UpdateFontSizeCommand fontSizeCommand)
        {
            var email = this.GetUserEmail();
            fontSizeCommand.Email = email;
            await _mediator.Send(fontSizeCommand);
        }
    }
}