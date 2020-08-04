﻿using Common;
using Common.DatabaseModels.helpers;
using Common.DatabaseModels.models;
using Domain.Commands.notes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WriteContext.Repositories;

namespace BI.services
{
    public class NoteHandlerCommand : 
        IRequestHandler<NewPrivateNoteCommand, string>,
        IRequestHandler<ChangeColorNoteCommand, Unit>,
        IRequestHandler<SetDeleteNoteCommand, Unit>,
        IRequestHandler<DeleteNotesCommand, Unit>,
        IRequestHandler<RestoreNoteCommand, Unit>
    {

        private readonly UserRepository userRepository;
        private readonly NoteRepository noteRepository;
        public NoteHandlerCommand(UserRepository userRepository, NoteRepository noteRepository)
        {
            this.userRepository = userRepository;
            this.noteRepository = noteRepository;
        }
        public async Task<string> Handle(NewPrivateNoteCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByEmail(request.Email);

            var note = new Note()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Order = 1,
                Color = NoteColorPallete.Green,
                NoteType = NotesType.Private
            };

            await this.noteRepository.Add(note);

            return note.Id.ToString("N");
        }

        public async Task<Unit> Handle(ChangeColorNoteCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserWithNotes(request.Email);
            var notes = user.Notes.Where(x => request.Ids.Contains(x.Id.ToString("N"))).ToList();

            if(notes.Any())
            {
                notes.ForEach(x => x.Color = request.Color);
                await noteRepository.UpdateRangeNotes(notes);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(SetDeleteNoteCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserWithNotes(request.Email);
            var notes = user.Notes.Where(x => request.Ids.Contains(x.Id.ToString("N"))).ToList();

            if(notes.Any())
            {
                await noteRepository.SetDeletedNotes(notes, user.Notes, request.NoteType);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteNotesCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserWithNotes(request.Email);
            var deletednotes = user.Notes.Where(x => x.NoteType == NotesType.Deleted).ToList();
            var selectdeletenotes = user.Notes.Where(x => request.Ids.Contains(x.Id.ToString("N"))).ToList();

            if (selectdeletenotes.Any())
            {
                await noteRepository.DeleteRangeDeleted(selectdeletenotes, deletednotes);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(RestoreNoteCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserWithNotes(request.Email);
            var notesForRestore = user.Notes.Where(x => request.Ids.Contains(x.Id.ToString("N"))).ToList();

            if (notesForRestore.Any())
            {
                await noteRepository.RestoreRange(notesForRestore, user.Notes);
            }

            return Unit.Value;
        }
    }
}
