﻿using Common;
using Common.DatabaseModels.Models.Notes;
using Common.DatabaseModels.Models.WS;
using Common.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Noots.Billing.Impl;
using Noots.DatabaseContext.Repositories.WS;
using Noots.Permissions.Queries;
using Noots.SignalrUpdater.Entities;
using Noots.SignalrUpdater.Interfaces;

namespace Noots.SignalrUpdater.Impl;

[Authorize]
public class AppSignalRHub : Hub
{
    private readonly IFolderServiceStorage folderServiceStorage;
    private readonly INoteServiceStorage noteServiceStorage;
    private readonly UserIdentifierConnectionIdRepository userIdentifierConnectionIdRepository;
    private readonly ILogger<AppSignalRHub> logger;
    private readonly IMediator mediator;
    private readonly BillingPermissionService billingPermissionService;

    public AppSignalRHub(
        IFolderServiceStorage folderServiceStorage,
        INoteServiceStorage noteServiceStorage,
        UserIdentifierConnectionIdRepository userIdentifierConnectionIdRepository,
        ILogger<AppSignalRHub> logger,
        IMediator _mediator,
        BillingPermissionService billingPermissionService)
    {
        this.folderServiceStorage = folderServiceStorage;
        this.noteServiceStorage = noteServiceStorage;
        this.userIdentifierConnectionIdRepository = userIdentifierConnectionIdRepository;
        this.logger = logger;
        mediator = _mediator;
        this.billingPermissionService = billingPermissionService;
    }

    public async Task UpdateUpdateStatus() {
        var ent = await userIdentifierConnectionIdRepository.FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);
        if (ent == null)
        {
            return;
        }
        ent.UpdatedAt = DateTimeProvider.Time;
        await userIdentifierConnectionIdRepository.UpdateAsync(ent);
    }

    private Guid GetUserId()
    {
        return Guid.Parse(Context.UserIdentifier!);
    }

    public async Task JoinNote(Guid noteId)
    {
        var userId = GetUserId();
        var ent = await userIdentifierConnectionIdRepository.GetConnectionAsync(userId, Context.ConnectionId);
        if (ent == null)
        {
            var res = new OperationResult<bool>().SetNotFound();
            await Clients.Caller.SendAsync(ClientMethods.setJoinedToNote, new JoinEntityStatus(noteId, res));
            return;
        }

        var command = new GetUserPermissionsForNoteQuery(noteId, userId);
        var permission = await mediator.Send(command);
        var isCanRead = permission.CanRead;
     
        if (permission.IsOwner)
        {
            await JoinNoteIternalAsync(noteId, ent, permission.GetAllUsers());
            return;
        }

        if (!isCanRead)
        {
            var res = new OperationResult<bool>().SetNoPermissions();
            await Clients.Caller.SendAsync(ClientMethods.setJoinedToNote, new JoinEntityStatus(noteId, res));
            return;
        }

        var usersOnNote = await noteServiceStorage.UsersOnNoteAsync(noteId, permission.Author.Id);
        var availableUserOnNotes = await billingPermissionService.GetAvailableUserOnNotes(permission.Note.UserId);
        var places = availableUserOnNotes - usersOnNote;

        if(places <= 0)
        {
            var res = new OperationResult<bool>().SetBillingError();
            await Clients.Caller.SendAsync(ClientMethods.setJoinedToNote, new JoinEntityStatus(noteId, res));
            return;
        }

        await JoinNoteIternalAsync(noteId, ent, permission.GetAllUsers());
    }

    private async Task JoinNoteIternalAsync(Guid noteId, UserIdentifierConnectionId ent, List<Guid> userIds)
    {
        await noteServiceStorage.AddAsync(noteId, ent);

        var result = new OperationResult<bool>(true, true);
        await Clients.Caller.SendAsync(ClientMethods.setJoinedToNote, new JoinEntityStatus(noteId, result));

        var ids = userIds.Select(x => x.ToString());
        await Clients.Users(ids).SendAsync(ClientMethods.updateOnlineUsersNote, noteId);
    }

    public async Task LeaveNote(Guid noteId)
    {
        var userId = GetUserId();
        var ent = await userIdentifierConnectionIdRepository.GetConnectionAsync(userId, Context.ConnectionId);
        if (ent == null) return;

        await noteServiceStorage.RemoveAsync(noteId, ent.Id);

        var command = new GetUserPermissionsForNoteQuery(noteId, userId);
        var permission = await mediator.Send(command);

        await RemoveOnlineUsersNoteAsync(noteId, ent.Id, userId, permission.GetAllUsers());
    }

    private async Task RemoveOnlineUsersNoteAsync(Guid noteId, Guid userIdentifier, Guid userId, List<Guid> userIds)
    {
        var ids = userIds.Select(x => x.ToString());
        await Clients.Users(ids).SendAsync(ClientMethods.removeOnlineUsersNote, new LeaveFromEntity(noteId, userIdentifier, userId));
    }

    // FOLDERS
    public async Task JoinFolder(Guid folderId)
    {
        var userId = GetUserId();
        var ent = await userIdentifierConnectionIdRepository.GetConnectionAsync(userId, Context.ConnectionId);
        if (ent == null)
        {
            var res = new OperationResult<bool>().SetNotFound();
            await Clients.Caller.SendAsync(ClientMethods.setJoinedToFolder, new JoinEntityStatus(folderId, res));
            return;
        }

        var command = new GetUserPermissionsForFolderQuery(folderId, userId);
        var permission = await mediator.Send(command);
        var isCanRead = permission.CanRead;

        if (permission.IsOwner)
        {
            await JoinFolderIternalAsync(folderId, ent, permission.GetAllUsers());
            return;
        }

        if (!isCanRead) 
        {
            var res = new OperationResult<bool>().SetNoPermissions();
            await Clients.Caller.SendAsync(ClientMethods.setJoinedToFolder, new JoinEntityStatus(folderId, res));
            return;
        }

        var usersOnFolder = await folderServiceStorage.UsersOnFolderAsync(folderId, permission.Author.Id);
        var availableUserOnFolder = await billingPermissionService.GetAvailableCountFolders(permission.Folder.UserId);
        var places = availableUserOnFolder - usersOnFolder;

        if (places <= 0)
        {
            var res = new OperationResult<bool>().SetBillingError();
            await Clients.Caller.SendAsync(ClientMethods.setJoinedToFolder, new JoinEntityStatus(folderId, res));
            return;
        }

        await JoinFolderIternalAsync(folderId, ent, permission.GetAllUsers());
    }

    private async Task JoinFolderIternalAsync(Guid folderId, UserIdentifierConnectionId ent, List<Guid> userIds)
    {
        await folderServiceStorage.AddAsync(folderId, ent);

        var result = new OperationResult<bool>(true, true);
        await Clients.Caller.SendAsync(ClientMethods.setJoinedToFolder, new JoinEntityStatus(folderId, result));

        var ids = userIds.Select(x => x.ToString());
        await Clients.Users(ids).SendAsync(ClientMethods.updateOnlineUsersFolder, folderId);
    }

    public async Task LeaveFolder(Guid folderId)
    {
        var userId = GetUserId();
        var ent = await userIdentifierConnectionIdRepository.GetConnectionAsync(userId, Context.ConnectionId);
        if (ent == null) return;

        await folderServiceStorage.RemoveAsync(folderId, ent.Id);

        var command = new GetUserPermissionsForFolderQuery(folderId, userId);
        var permission = await mediator.Send(command);

        await RemoveOnlineUsersFolderAsync(folderId, ent.Id, userId, permission.GetAllUsers());
    }

    private async Task RemoveOnlineUsersFolderAsync(Guid folderId, Guid userIdentifier, Guid userId, List<Guid> userIds)
    {
        var ids = userIds.Select(x => x.ToString());
        await Clients.Users(ids).SendAsync(ClientMethods.removeOnlineUsersFolder, new LeaveFromEntity(folderId, userIdentifier, userId));
    }

    // override
    public override async Task OnConnectedAsync()
    {
        await AddConnectionAsync();

        await base.OnConnectedAsync();
    }

    private async Task AddConnectionAsync()
    {
        var httpContext = Context.Features.Get<IHttpContextFeature>();
        var headers = httpContext?.HttpContext?.Request?.Headers;
        string userAgent = null!;
        if (headers != null && headers.ContainsKey("User-Agent"))
        {
            userAgent = headers["User-Agent"];
        }

        var userId = GetUserId();
        var entity = await userIdentifierConnectionIdRepository.GetConnectionAsync(userId, Context.ConnectionId);

        if (entity != null) return;

        entity = new UserIdentifierConnectionId
        {
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            UserAgent = userAgent,
            ConnectedAt = DateTimeProvider.Time,
            UpdatedAt = DateTimeProvider.Time
        };

        await userIdentifierConnectionIdRepository.AddAsync(entity);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        await RemoveConnectionAsync();
    }

    private async Task RemoveConnectionAsync()
    {
        var userId = GetUserId();
        var ents = await userIdentifierConnectionIdRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.ConnectionId == Context.ConnectionId);

        if (ents != null)
        {
            await userIdentifierConnectionIdRepository.RemoveAsync(ents);
        }
    }
}
