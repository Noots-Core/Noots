﻿using Common.DatabaseModels.Models.Files;
using Common.DTO.Notes.AdditionalContent;
using MediatR;
using Noots.DatabaseContext.Repositories.Folders;
using Noots.DatabaseContext.Repositories.Histories;
using Noots.DatabaseContext.Repositories.NoteContent;
using Noots.DatabaseContext.Repositories.Notes;
using Noots.Notes.Queries;

namespace Noots.Notes.Handlers.Queries;

public class GetAdditionalContentNoteInfoQueryHandler : IRequestHandler<GetAdditionalContentNoteInfoQuery, List<BottomNoteContent>>
{
    private readonly UsersOnPrivateNotesRepository usersOnPrivateNotesRepository;
    private readonly FoldersNotesRepository foldersNotesRepository;
    private readonly CollectionNoteRepository collectionNoteRepository;
    private readonly NoteSnapshotRepository noteSnapshotRepository;

    public GetAdditionalContentNoteInfoQueryHandler(
        UsersOnPrivateNotesRepository usersOnPrivateNotesRepository,
        FoldersNotesRepository foldersNotesRepository,
        CollectionNoteRepository collectionNoteRepository,
        NoteSnapshotRepository noteSnapshotRepository)
    {
        this.usersOnPrivateNotesRepository = usersOnPrivateNotesRepository;
        this.foldersNotesRepository = foldersNotesRepository;
        this.collectionNoteRepository = collectionNoteRepository;
        this.noteSnapshotRepository = noteSnapshotRepository;
    }
    
    public async Task<List<BottomNoteContent>> Handle(GetAdditionalContentNoteInfoQuery request, CancellationToken cancellationToken)
    {
        long GetSize(Guid noteId, params Dictionary<Guid, (Guid, IEnumerable<AppFile>)>[] filesDict)
        {
            return filesDict
                .Where(x => x.ContainsKey(noteId))
                .SelectMany(x => x[noteId].Item2)
                .DistinctBy(x => x.Id)
                .Sum(x => x.Size);
        }

        var usersOnNotes = await usersOnPrivateNotesRepository.GetByNoteIdsWithUser(request.NoteIds);
        var notesFolder = await foldersNotesRepository.GetByNoteIdsIncludeFolder(request.NoteIds);
        var size = await collectionNoteRepository.GetMemoryOfNotes(request.NoteIds);
        var sizeSnapshots = await noteSnapshotRepository.GetMemoryOfNotesSnapshots(request.NoteIds);

        var usersOnNotesDict = usersOnNotes.ToLookup(x => x.NoteId);
        var notesFolderDict = notesFolder.ToLookup(x => x.NoteId);

        return request.NoteIds.Select(noteId => new BottomNoteContent
        {
            IsHasUserOnNote = usersOnNotesDict.Contains(noteId),
            NoteId = noteId,
            NoteFolderInfos = notesFolderDict.Contains(noteId) ? notesFolderDict[noteId].Select(x => new NoteFolderInfo(x.FolderId, x.Folder.GetTitle()?.ReadStr())).ToList() : null,
            TotalSize = GetSize(noteId, size, sizeSnapshots)
        }).ToList();
    }
}