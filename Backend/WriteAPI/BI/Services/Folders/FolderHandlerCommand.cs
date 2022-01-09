﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BI.Mapping;
using BI.SignalR;
using Common;
using Common.DatabaseModels.Models.Folders;
using Common.DatabaseModels.Models.Systems;
using Common.DTO;
using Common.DTO.Folders;
using Common.DTO.WebSockets;
using Domain.Commands.Folders;
using Domain.Queries.Permissions;
using MediatR;
using WriteContext.Repositories.Folders;
using WriteContext.Repositories.Users;

namespace BI.Services.Folders
{
    public class FolderHandlerCommand : 
        IRequestHandler<NewFolderCommand, SmallFolder>,
        IRequestHandler<ArchiveFolderCommand, OperationResult<Unit>>,
        IRequestHandler<ChangeColorFolderCommand, OperationResult<Unit>>,
        IRequestHandler<SetDeleteFolderCommand, OperationResult<Unit>>,
        IRequestHandler<CopyFolderCommand, List<SmallFolder>>,
        IRequestHandler<DeleteFoldersCommand, Unit>,
        IRequestHandler<MakePrivateFolderCommand, OperationResult<Unit>>
    {
        private readonly FolderRepository folderRepository;
        private readonly FoldersNotesRepository foldersNotesRepository;
        private readonly AppSignalRService appSignalRService;
        private readonly UserRepository userRepository;
        private readonly AppCustomMapper appCustomMapper;
        private readonly IMediator _mediator;
        public FolderHandlerCommand(
            FolderRepository folderRepository,
            UserRepository userRepository, 
            AppCustomMapper appCustomMapper, 
            IMediator _mediator,
            FoldersNotesRepository foldersNotesRepository,
            AppSignalRService appSignalRService)
        {
            this.folderRepository = folderRepository;
            this.userRepository = userRepository;
            this.appCustomMapper = appCustomMapper;
            this._mediator = _mediator;
            this.foldersNotesRepository = foldersNotesRepository;
            this.appSignalRService = appSignalRService;
        }

        public async Task<SmallFolder> Handle(NewFolderCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FirstOrDefaultAsync(x => x.Email == request.Email);

            var folder = new Folder()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Order = 1,
                Color = FolderColorPallete.Green,
                FolderTypeId = FolderTypeENUM.Private,
                RefTypeId = RefTypeENUM.Viewer,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            };

            await folderRepository.Add(folder, FolderTypeENUM.Private);

            var newFolder = await folderRepository.GetOneById(folder.Id);

            return appCustomMapper.MapFolderToSmallFolder(newFolder);
        }

        public async Task<OperationResult<Unit>> Handle(ArchiveFolderCommand request, CancellationToken cancellationToken)
        {
            var command = new GetUserPermissionsForFoldersManyQuery(request.Ids, request.Email);
            var permissions = await _mediator.Send(command);

            var user = await userRepository.GetUserWithFoldersIncludeFolderType(request.Email);
            var isCanDelete = permissions.All(x => x.Item2.IsOwner);

            if (isCanDelete)
            {
                var folders = permissions.Select(x => x.perm.Folder).ToList();
                folders.ForEach(note => note.DeletedAt = null);
                await folderRepository.CastFolders(folders, user.Folders, folders.FirstOrDefault().FolderTypeId, FolderTypeENUM.Archived);
                return new OperationResult<Unit>(true, Unit.Value);
            }
            return new OperationResult<Unit>().SetNoPermissions();
        }

        public async Task<OperationResult<Unit>> Handle(ChangeColorFolderCommand request, CancellationToken cancellationToken)
        {
            var command = new GetUserPermissionsForFoldersManyQuery(request.Ids, request.Email);
            var permissions = await _mediator.Send(command);
            var isCanEdit = permissions.All(x => x.perm.CanWrite);
            if (isCanEdit)
            {
                var foldersForUpdate = permissions.Select(x => x.perm.Folder);
                foreach (var folder in foldersForUpdate)
                {
                    folder.Color = request.Color;
                    folder.UpdatedAt = DateTimeOffset.Now;
                }

                await folderRepository.UpdateRangeAsync(foldersForUpdate);

                // WS UPDATES
                var folders = permissions.Select(x => x.perm.Folder);
                var userIds = folders.SelectMany(x =>
                {
                    var list = new List<Guid>() { x.UserId };
                    list.AddRange(x.UsersOnPrivateFolders.Select(x => x.UserId));
                    return list;
                });
                var users = await userRepository.GetWhereAsync(x => userIds.Contains(x.Id));
                var emails = users.Select(x => x.Email);
                var updates = folders.Select(x => new UpdateFolderWS { Color = x.Color, FolderId = x.Id });
                await appSignalRService.UpdateFoldersInManyUsers(updates, emails);
                //
                return new OperationResult<Unit>(true, Unit.Value);
            }
            return new OperationResult<Unit>().SetNoPermissions();
        }


        public async Task<OperationResult<Unit>> Handle(SetDeleteFolderCommand request, CancellationToken cancellationToken)
        {
            var command = new GetUserPermissionsForFoldersManyQuery(request.Ids, request.Email);
            var permissions = await _mediator.Send(command);

            var user = await userRepository.GetUserWithFoldersIncludeFolderType(request.Email);
            var isCanDelete = permissions.All(x => x.Item2.IsOwner);

            if (isCanDelete)
            {
                var folders = permissions.Select(x => x.perm.Folder).ToList();
                folders.ForEach(note => note.DeletedAt = DateTimeOffset.UtcNow);
                await folderRepository.CastFolders(folders, user.Folders, folders.FirstOrDefault().FolderTypeId, FolderTypeENUM.Deleted);
                return new OperationResult<Unit>(true, Unit.Value);
            }
            return new OperationResult<Unit>().SetNoPermissions();
        }

        public async Task<List<SmallFolder>> Handle(CopyFolderCommand request, CancellationToken cancellationToken)
        {
            var resultIds = new List<Guid>();
            var order = -1;

            var command = new GetUserPermissionsForFoldersManyQuery(request.Ids, request.Email);
            var permissions = await _mediator.Send(command);

            if (permissions.Any())
            {
                var idsForCopy = permissions.Where(x => x.Item2.CanRead).Select(x => x.Item1).ToList();
                var permission = permissions.First().Item2;
                if (idsForCopy.Any())
                {
                    var foldersForCopy = await folderRepository.GetFoldersByIdsForCopy(idsForCopy);
                    foreach(var folderForCopy in foldersForCopy)
                    {
                        var newFolder = new Folder()
                        {
                            Title = folderForCopy.Title,
                            Color = folderForCopy.Color,
                            FolderTypeId = FolderTypeENUM.Private,
                            RefTypeId = folderForCopy.RefTypeId,
                            Order = order--,
                            CreatedAt = DateTimeOffset.Now,
                            UpdatedAt = DateTimeOffset.Now,
                            UserId = permission.User.Id
                        };
                        var dbFolder = await folderRepository.AddAsync(newFolder);
                        resultIds.Add(dbFolder.Entity.Id);
                        var foldersNotes = folderForCopy.FoldersNotes.Select(note => new FoldersNotes()
                        {
                            FolderId = dbFolder.Entity.Id,
                            NoteId = note.NoteId
                        });
                        await foldersNotesRepository.AddRangeAsync(foldersNotes);
                    }

                    var dbFolders = await folderRepository.GetFoldersByUserIdAndTypeIdNotesIncludeNote(permission.User.Id, FolderTypeENUM.Private);
                    var orders = Enumerable.Range(1, dbFolders.Count);
                    dbFolders = dbFolders.Zip(orders, (folder, order) => {
                        folder.Order = order;
                        return folder;
                    }).ToList();

                    await folderRepository.UpdateRangeAsync(dbFolders);
                    var resultFolders = dbFolders.Where(dbFolder => resultIds.Contains(dbFolder.Id)).ToList();
                    return appCustomMapper.MapFoldersToSmallFolders(resultFolders);
                }
            }

            return new List<SmallFolder>();
        }

        public async Task<Unit> Handle(DeleteFoldersCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserWithFoldersIncludeFolderType(request.Email);

            var deletedFolders = user.Folders.Where(x => x.FolderTypeId == FolderTypeENUM.Deleted).ToList();
            var folders = user.Folders.Where(x => request.Ids.Any(z => z == x.Id)).ToList();

            if (folders.Count == request.Ids.Count)
            {
                await folderRepository.DeleteRangeDeleted(folders, deletedFolders);
            }
            else
            {
                throw new Exception("Incorrect count");
            }

            return Unit.Value;
        }


        public async Task<OperationResult<Unit>> Handle(MakePrivateFolderCommand request, CancellationToken cancellationToken)
        {
            var command = new GetUserPermissionsForFoldersManyQuery(request.Ids, request.Email);
            var permissions = await _mediator.Send(command);

            var user = await userRepository.GetUserWithFoldersIncludeFolderType(request.Email);
            var isCanDelete = permissions.All(x => x.Item2.IsOwner);
            if (isCanDelete)
            {
                var folders = permissions.Select(x => x.perm.Folder).ToList();
                folders.ForEach(note => note.DeletedAt = null);
                await folderRepository.CastFolders(folders, user.Folders, folders.FirstOrDefault().FolderTypeId, FolderTypeENUM.Private);
                return new OperationResult<Unit>(true, Unit.Value);
            }
            return new OperationResult<Unit>().SetNoPermissions();
        }
    }
}
