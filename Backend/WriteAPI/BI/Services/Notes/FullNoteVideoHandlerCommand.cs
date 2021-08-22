﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BI.Helpers;
using BI.Services.History;
using BI.SignalR;
using Common.DatabaseModels.Models.NoteContent;
using Common.DTO.Notes.FullNoteContent;
using Domain.Commands.Files;
using Domain.Commands.NoteInner.FileContent.Videos;
using Domain.Queries.Permissions;
using MediatR;
using WriteContext.Repositories;
using WriteContext.Repositories.NoteContent;

namespace BI.Services.Notes
{
    public class FullNoteVideoHandlerCommand :
        IRequestHandler<InsertVideosToNoteCommand, OperationResult<VideoNoteDTO>>,
        IRequestHandler<RemoveVideoCommand, OperationResult<Unit>>,
        IRequestHandler<TransformToVideosCommand, OperationResult<VideoNoteDTO>>
    {

        private readonly IMediator _mediator;

        private readonly BaseNoteContentRepository baseNoteContentRepository;

        private readonly FileRepository fileRepository;

        private readonly VideoNoteRepository videoNoteRepository;

        private readonly HistoryCacheService historyCacheService;

        private readonly AppSignalRService appSignalRService;

        public FullNoteVideoHandlerCommand(
            IMediator _mediator,
            BaseNoteContentRepository baseNoteContentRepository,
            FileRepository fileRepository,
            VideoNoteRepository videoNoteRepository,
            HistoryCacheService historyCacheService,
            AppSignalRService appSignalRService)
        {
            this._mediator = _mediator;
            this.baseNoteContentRepository = baseNoteContentRepository;
            this.fileRepository = fileRepository;
            this.videoNoteRepository = videoNoteRepository;
            this.historyCacheService = historyCacheService;
            this.appSignalRService = appSignalRService;
        }

        public async Task<OperationResult<VideoNoteDTO>> Handle(InsertVideosToNoteCommand request, CancellationToken cancellationToken)
        {
            var command = new GetUserPermissionsForNoteQuery(request.NoteId, request.Email);
            var permissions = await _mediator.Send(command);
            var note = permissions.Note;

            if (permissions.CanWrite)
            {
                // PERMISSION MEMORY
                var uploadPermission = await _mediator.Send(new GetPermissionUploadFileQuery(request.Video.Length, permissions.Author.Id));
                if (uploadPermission == PermissionUploadFileEnum.NoCanUpload)
                {
                    return new OperationResult<VideoNoteDTO>().SetNoEnougnMemory();
                }

                // FILES LOGIC
                var filebyte = await request.Video.GetFilesBytesAsync();
                var file = await _mediator.Send(new SaveVideoToNoteCommand(permissions.Author.Id, filebyte, note.Id));

                if (cancellationToken.IsCancellationRequested)
                {
                    await _mediator.Send(new RemoveFilesFromStorageCommand(file.GetNotNullPathes(), permissions.Author.Id.ToString()));
                    return new OperationResult<VideoNoteDTO>().SetRequestCancelled();
                }

                // UPDATING
                var contents = await baseNoteContentRepository.GetWhereAsync(x => x.NoteId == note.Id);
                var contentForRemove = contents.First(x => x.Id == request.ContentId);

                using var transaction = await baseNoteContentRepository.context.Database.BeginTransactionAsync();

                try
                {
                    await baseNoteContentRepository.RemoveAsync(contentForRemove);

                    await fileRepository.AddAsync(file);

                    var videoNote = new VideoNote()
                    {
                        AppFile = file,
                        AppFileId = file.Id,
                        Name = request.Video.FileName,
                        Note = note,
                        Order = contentForRemove.Order,
                    };

                    await videoNoteRepository.AddAsync(videoNote);

                    await transaction.CommitAsync();

                    var result = new VideoNoteDTO(videoNote.Name, videoNote.AppFileId, 
                        file.PathNonPhotoContent, videoNote.Id, videoNote.UpdatedAt, videoNote.AppFile.UserId);

                    historyCacheService.UpdateNote(permissions.Note.Id, permissions.User.Id, permissions.Author.Email);
                    await appSignalRService.UpdateContent(request.NoteId, permissions.User.Email);

                    return new OperationResult<VideoNoteDTO>(success: true, result);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(e);
                    await _mediator.Send(new RemoveFilesCommand(permissions.User.Id.ToString(), file));
                }
            }

            return new OperationResult<VideoNoteDTO>().SetNoPermissions();
        }

        public async Task<OperationResult<Unit>> Handle(RemoveVideoCommand request, CancellationToken cancellationToken)
        {
            var command = new GetUserPermissionsForNoteQuery(request.NoteId, request.Email);
            var permissions = await _mediator.Send(command);
            var note = permissions.Note;

            if (permissions.CanWrite)
            {
                var contents = await baseNoteContentRepository.GetAllContentByNoteIdOrdered(note.Id);
                var contentForRemove = contents.FirstOrDefault(x => x.Id == request.ContentId) as VideoNote;
                contents.Remove(contentForRemove);

                var orders = Enumerable.Range(1, contents.Count);
                contents = contents.Zip(orders, (content, order) =>
                {
                    content.Order = order;
                    content.UpdatedAt = DateTimeOffset.Now;
                    return content;
                }).ToList();

                using var transaction = await baseNoteContentRepository.context.Database.BeginTransactionAsync();

                try
                {
                    await baseNoteContentRepository.RemoveAsync(contentForRemove);
                    await baseNoteContentRepository.UpdateRangeAsync(contents);

                    await transaction.CommitAsync();

                    await _mediator.Send(new RemoveFilesCommand(permissions.User.Id.ToString(), contentForRemove.AppFile));

                    historyCacheService.UpdateNote(permissions.Note.Id, permissions.User.Id, permissions.Author.Email);
                    await appSignalRService.UpdateContent(request.NoteId, permissions.User.Email);

                    return new OperationResult<Unit>(success: true, Unit.Value);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(e);
                }
            }

            return new OperationResult<Unit>().SetNoPermissions();
        }

        public Task<OperationResult<VideoNoteDTO>> Handle(TransformToVideosCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
