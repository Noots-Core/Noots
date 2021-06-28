﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.DTO.Orders;
using Domain.Commands.Orders;
using MediatR;
using WriteContext.Repositories.Folders;
using WriteContext.Repositories.Labels;
using WriteContext.Repositories.Notes;
using WriteContext.Repositories.Users;

namespace BI.Services
{
    public class OrderHandlerCommand : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly LabelRepository labelRepository;
        private readonly NoteRepository noteRepository;
        private readonly FolderRepository folderRepository;
        private readonly UserRepository userRepository;
        public OrderHandlerCommand(
            LabelRepository labelRepository,
            NoteRepository noteRepository,
            UserRepository userRepository,
            FolderRepository folderRepository)
        {
            this.labelRepository = labelRepository;
            this.noteRepository = noteRepository;
            this.userRepository = userRepository;
            this.folderRepository = folderRepository;
        }
        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            switch (request.OrderEntity)
            {
                case OrderEntity.Label:
                    {
                        var labels = (await userRepository.GetUserWithLabels(request.Email)).Labels;

                        var label = labels.FirstOrDefault(x => x.Id == request.EntityId);
                        if (label != null)
                        {
                            var tempLabels = labels.Where(x => x.IsDeleted == label.IsDeleted).ToList();
                            if (label.Order < request.Position)
                            {
                                tempLabels.Where(x => x.Order <= request.Position && x.Order > label.Order).ToList().ForEach(x => x.Order = x.Order - 1);
                                label.Order = request.Position;
                            }
                            else if (label.Order > request.Position)
                            {
                                tempLabels.Where(x => x.Order >= request.Position && x.Order < label.Order).ToList().ForEach(x => x.Order = x.Order + 1);
                                label.Order = request.Position;
                            }
                            await labelRepository.UpdateRange(tempLabels);
                        }
                        break;
                    }
                case OrderEntity.Note:
                    {

                        var notes = (await userRepository.GetUserWithNotesIncludeNoteType(request.Email)).Notes;
                        var note = notes.FirstOrDefault(x => x.Id == request.EntityId);

                        if (note != null)
                        {
                            var notesWithType = notes.Where(x => x.NoteTypeId == note.NoteTypeId).ToList();

                            if (note.Order < request.Position)
                            {
                                notesWithType.Where(x => x.Order <= request.Position && x.Order > note.Order).ToList().ForEach(x => x.Order = x.Order - 1);
                                note.Order = request.Position;
                            }
                            else
                            {
                                notesWithType.Where(x => x.Order >= request.Position && x.Order < note.Order).ToList().ForEach(x => x.Order = x.Order + 1);
                                note.Order = request.Position;
                            }
                            await noteRepository.UpdateRange(notesWithType);
                        }

                        Console.WriteLine("Note");
                        break;
                    }
                case OrderEntity.Folder:
                    {

                        var folders = (await userRepository.GetUserWithFoldersIncludeFolderType(request.Email)).Folders;
                        var folder = folders.FirstOrDefault(x => x.Id == request.EntityId);

                        if (folder != null)
                        {
                            var foldersWithType = folders.Where(x => x.FolderTypeId == folder.FolderTypeId).ToList();

                            if (folder.Order < request.Position)
                            {
                                foldersWithType.Where(x => x.Order <= request.Position && x.Order > folder.Order).ToList().ForEach(x => x.Order = x.Order - 1);
                                folder.Order = request.Position;
                            }
                            else
                            {
                                foldersWithType.Where(x => x.Order >= request.Position && x.Order < folder.Order).ToList().ForEach(x => x.Order = x.Order + 1);
                                folder.Order = request.Position;
                            }
                            await folderRepository.UpdateRange(foldersWithType);
                        }
                        Console.WriteLine("Note");
                        break;
                    }
                default:
                    {
                        throw new Exception();
                    }
            }
            return Unit.Value;
        }
    }
}
