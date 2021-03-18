﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WriteContext;

namespace WriteContext.Migrations
{
    [DbContext(typeof(WriteContextDB))]
    [Migration("20210318191103_add-fileds")]
    partial class addfileds
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("AlbumNoteAppFile", b =>
                {
                    b.Property<Guid>("AlbumNotesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PhotosId")
                        .HasColumnType("uuid");

                    b.HasKey("AlbumNotesId", "PhotosId");

                    b.HasIndex("PhotosId");

                    b.ToTable("AlbumNoteAppFile");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.AppFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Backgrounds", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("UserId");

                    b.ToTable("Backgrounds");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Folder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FolderTypeId")
                        .HasColumnType("uuid");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<Guid>("RefTypeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FolderTypeId");

                    b.HasIndex("RefTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.FolderType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FoldersTypes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("381428f6-0568-4fb4-9c86-2d9e0f381308"),
                            Name = "private"
                        },
                        new
                        {
                            Id = new Guid("96c416cd-94d1-4f6c-9dd6-3b1f1e1e14e9"),
                            Name = "shared"
                        },
                        new
                        {
                            Id = new Guid("e3ea1cb2-5301-42fd-b283-2fe6133755c1"),
                            Name = "deleted"
                        },
                        new
                        {
                            Id = new Guid("3e00dc8e-1030-4022-bc73-9d5c13b363d3"),
                            Name = "archive"
                        });
                });

            modelBuilder.Entity("Common.DatabaseModels.models.FoldersNotes", b =>
                {
                    b.Property<Guid>("NoteId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FolderId")
                        .HasColumnType("uuid");

                    b.HasKey("NoteId", "FolderId");

                    b.HasIndex("FolderId");

                    b.ToTable("FoldersNotes");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.FontSize", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FontSizes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5c335a93-7aa7-40ff-b995-6c90f2536e98"),
                            Name = "medium"
                        },
                        new
                        {
                            Id = new Guid("656e1f08-bb0e-406c-a0b9-77dc3e10a86b"),
                            Name = "big"
                        });
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Labels");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.LabelsNotes", b =>
                {
                    b.Property<Guid>("NoteId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("LabelId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("AddedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("NoteId", "LabelId");

                    b.HasIndex("LabelId");

                    b.ToTable("LabelsNotes");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Language", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Languages");

                    b.HasData(
                        new
                        {
                            Id = new Guid("38b402a0-e1b1-42d7-b472-db788a1a3924"),
                            Name = "ukraine"
                        },
                        new
                        {
                            Id = new Guid("01a4f567-b5cd-4d98-8d55-b49df9415d99"),
                            Name = "russian"
                        },
                        new
                        {
                            Id = new Guid("6579263d-c4db-446a-8223-7d895dc45f1b"),
                            Name = "english"
                        });
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Note", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("NoteTypeId")
                        .HasColumnType("uuid");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<Guid>("RefTypeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("NoteTypeId");

                    b.HasIndex("RefTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NoteContent.BaseNoteContent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("NextId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("NoteId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PrevId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("NextId");

                    b.HasIndex("NoteId");

                    b.HasIndex("PrevId");

                    b.ToTable("BaseNoteContents");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NoteType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("NotesTypes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d01e34ef-3bc0-4fd4-b4cf-0996101e9d87"),
                            Name = "private"
                        },
                        new
                        {
                            Id = new Guid("ad503d43-c28e-405a-aa20-bcb4e2b1a2a5"),
                            Name = "shared"
                        },
                        new
                        {
                            Id = new Guid("1f384f3c-1aa8-4664-ac8d-e264e68164dc"),
                            Name = "deleted"
                        },
                        new
                        {
                            Id = new Guid("556a3f0d-1edd-4ccc-bd7e-b087b033849a"),
                            Name = "archive"
                        });
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NotificationSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("NotificationSettings");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.RefType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RefTypes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7c247026-36c6-4c17-b227-afb37e8ec7cd"),
                            Name = "viewer"
                        },
                        new
                        {
                            Id = new Guid("397821bf-74d5-4bdf-81e4-0698d5a92476"),
                            Name = "editor"
                        });
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Theme", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Themes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5b08dced-b041-4a77-b290-f08e36af1d70"),
                            Name = "light"
                        },
                        new
                        {
                            Id = new Guid("f52a188b-5422-4144-91f6-bde40b82ce22"),
                            Name = "dark"
                        });
                });

            modelBuilder.Entity("Common.DatabaseModels.models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CurrentBackgroundId")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("FontSizeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("LanguageId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PersonalKey")
                        .HasColumnType("text");

                    b.Property<Guid?>("PhotoId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ThemeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CurrentBackgroundId")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("FontSizeId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("PhotoId")
                        .IsUnique();

                    b.HasIndex("ThemeId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.UserOnNoteNow", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("NoteId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "NoteId");

                    b.HasIndex("NoteId");

                    b.ToTable("UserOnNoteNow");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.UserOnPrivateNotes", b =>
                {
                    b.Property<Guid>("NoteId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccessTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("NoteId", "UserId");

                    b.HasIndex("AccessTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("UserOnPrivateNotes");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.UsersOnPrivateFolders", b =>
                {
                    b.Property<Guid>("FolderId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccessTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("FolderId", "UserId");

                    b.HasIndex("AccessTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersOnPrivateFolders");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NoteContent.AlbumNote", b =>
                {
                    b.HasBaseType("Common.DatabaseModels.models.NoteContent.BaseNoteContent");

                    b.Property<string>("Height")
                        .HasColumnType("text");

                    b.Property<string>("Width")
                        .HasColumnType("text");

                    b.ToTable("AlbumNote");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NoteContent.TextNote", b =>
                {
                    b.HasBaseType("Common.DatabaseModels.models.NoteContent.BaseNoteContent");

                    b.Property<bool>("Checked")
                        .HasColumnType("boolean");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<string>("HeadingType")
                        .HasColumnType("text");

                    b.Property<string>("TextType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("TextNote");
                });

            modelBuilder.Entity("AlbumNoteAppFile", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.NoteContent.AlbumNote", null)
                        .WithMany()
                        .HasForeignKey("AlbumNotesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.AppFile", null)
                        .WithMany()
                        .HasForeignKey("PhotosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Backgrounds", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.AppFile", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.User", "User")
                        .WithMany("Backgrounds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Folder", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.FolderType", "FolderType")
                        .WithMany("Folders")
                        .HasForeignKey("FolderTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.RefType", "RefType")
                        .WithMany("Folders")
                        .HasForeignKey("RefTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.User", "User")
                        .WithMany("Folders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FolderType");

                    b.Navigation("RefType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.FoldersNotes", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.Folder", "Folder")
                        .WithMany("FoldersNotes")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.Note", "Note")
                        .WithMany("FoldersNotes")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Folder");

                    b.Navigation("Note");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Label", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.User", "User")
                        .WithMany("Labels")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.LabelsNotes", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.Label", "Label")
                        .WithMany("LabelsNotes")
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.Note", "Note")
                        .WithMany("LabelsNotes")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Label");

                    b.Navigation("Note");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Note", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.NoteType", "NoteType")
                        .WithMany("Notes")
                        .HasForeignKey("NoteTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.RefType", "RefType")
                        .WithMany("Notes")
                        .HasForeignKey("RefTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.User", "User")
                        .WithMany("Notes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NoteType");

                    b.Navigation("RefType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NoteContent.BaseNoteContent", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.NoteContent.BaseNoteContent", "Next")
                        .WithMany()
                        .HasForeignKey("NextId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Common.DatabaseModels.models.Note", "Note")
                        .WithMany("Contents")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.NoteContent.BaseNoteContent", "Prev")
                        .WithMany()
                        .HasForeignKey("PrevId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Next");

                    b.Navigation("Note");

                    b.Navigation("Prev");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NotificationSetting", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.User", "User")
                        .WithOne("NotificationSettings")
                        .HasForeignKey("Common.DatabaseModels.models.NotificationSetting", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.User", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.Backgrounds", "CurrentBackground")
                        .WithOne("CurrentUserBackground")
                        .HasForeignKey("Common.DatabaseModels.models.User", "CurrentBackgroundId");

                    b.HasOne("Common.DatabaseModels.models.FontSize", "FontSize")
                        .WithMany("Users")
                        .HasForeignKey("FontSizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.Language", "Language")
                        .WithMany("Users")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.AppFile", "Photo")
                        .WithOne("User")
                        .HasForeignKey("Common.DatabaseModels.models.User", "PhotoId");

                    b.HasOne("Common.DatabaseModels.models.Theme", "Theme")
                        .WithMany("Users")
                        .HasForeignKey("ThemeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrentBackground");

                    b.Navigation("FontSize");

                    b.Navigation("Language");

                    b.Navigation("Photo");

                    b.Navigation("Theme");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.UserOnNoteNow", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.Note", "Note")
                        .WithMany("UserOnNotesNow")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.User", "User")
                        .WithMany("UserOnNotes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Note");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.UserOnPrivateNotes", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.RefType", "AccessType")
                        .WithMany("UserOnPrivateNotes")
                        .HasForeignKey("AccessTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.Note", "Note")
                        .WithMany("UsersOnPrivateNotes")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.User", "User")
                        .WithMany("UserOnPrivateNotes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccessType");

                    b.Navigation("Note");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.UsersOnPrivateFolders", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.RefType", "AccessType")
                        .WithMany("UsersOnPrivateFolders")
                        .HasForeignKey("AccessTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.Folder", "Folder")
                        .WithMany("UsersOnPrivateFolders")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Common.DatabaseModels.models.User", "User")
                        .WithMany("UsersOnPrivateFolders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccessType");

                    b.Navigation("Folder");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NoteContent.AlbumNote", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.NoteContent.BaseNoteContent", null)
                        .WithOne()
                        .HasForeignKey("Common.DatabaseModels.models.NoteContent.AlbumNote", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NoteContent.TextNote", b =>
                {
                    b.HasOne("Common.DatabaseModels.models.NoteContent.BaseNoteContent", null)
                        .WithOne()
                        .HasForeignKey("Common.DatabaseModels.models.NoteContent.TextNote", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Common.DatabaseModels.models.AppFile", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Backgrounds", b =>
                {
                    b.Navigation("CurrentUserBackground");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Folder", b =>
                {
                    b.Navigation("FoldersNotes");

                    b.Navigation("UsersOnPrivateFolders");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.FolderType", b =>
                {
                    b.Navigation("Folders");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.FontSize", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Label", b =>
                {
                    b.Navigation("LabelsNotes");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Language", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Note", b =>
                {
                    b.Navigation("Contents");

                    b.Navigation("FoldersNotes");

                    b.Navigation("LabelsNotes");

                    b.Navigation("UserOnNotesNow");

                    b.Navigation("UsersOnPrivateNotes");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.NoteType", b =>
                {
                    b.Navigation("Notes");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.RefType", b =>
                {
                    b.Navigation("Folders");

                    b.Navigation("Notes");

                    b.Navigation("UserOnPrivateNotes");

                    b.Navigation("UsersOnPrivateFolders");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.Theme", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Common.DatabaseModels.models.User", b =>
                {
                    b.Navigation("Backgrounds");

                    b.Navigation("Folders");

                    b.Navigation("Labels");

                    b.Navigation("Notes");

                    b.Navigation("NotificationSettings");

                    b.Navigation("UserOnNotes");

                    b.Navigation("UserOnPrivateNotes");

                    b.Navigation("UsersOnPrivateFolders");
                });
#pragma warning restore 612, 618
        }
    }
}
