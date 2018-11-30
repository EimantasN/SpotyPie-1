﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Migrations
{
    [DbContext(typeof(SpotyPieIDbContext))]
    partial class SpotyPieIDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Models.BackEnd.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AlbumType");

                    b.Property<int?>("ArtistId");

                    b.Property<string>("Artists");

                    b.Property<int?>("CopyrightId");

                    b.Property<string>("Copyrights");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Genres");

                    b.Property<string>("Label");

                    b.Property<DateTime>("LastActiveTime");

                    b.Property<string>("Name");

                    b.Property<long>("Popularity");

                    b.Property<DateTimeOffset>("ReleaseDate");

                    b.Property<long>("TotalTracks");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("CopyrightId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("Models.BackEnd.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Genres");

                    b.Property<DateTime>("LastActiveTime");

                    b.Property<string>("Name");

                    b.Property<long>("Popularity");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("Models.BackEnd.Copyright", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Text");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("Copyrights");
                });

            modelBuilder.Entity("Models.BackEnd.CurrentSong", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlbumId");

                    b.Property<int>("ArtistId");

                    b.Property<int>("CurrentMs");

                    b.Property<long>("DurationMs");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("LocalUrl");

                    b.Property<string>("Name");

                    b.Property<int>("PlaylistId");

                    b.Property<int>("SongId");

                    b.HasKey("Id");

                    b.ToTable("CurrentSong");
                });

            modelBuilder.Entity("Models.BackEnd.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AlbumId");

                    b.Property<int?>("ArtistId");

                    b.Property<long>("Height");

                    b.Property<string>("Url");

                    b.Property<long>("Width");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ArtistId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Models.BackEnd.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AlbumId");

                    b.Property<int?>("ArtistId");

                    b.Property<string>("Artists");

                    b.Property<long>("DiscNumber");

                    b.Property<long>("DurationMs");

                    b.Property<bool>("Explicit");

                    b.Property<string>("ImageUrl");

                    b.Property<bool>("IsLocal");

                    b.Property<bool>("IsPlayable");

                    b.Property<DateTime>("LastActiveTime");

                    b.Property<string>("LocalUrl");

                    b.Property<string>("Name");

                    b.Property<int?>("PlaylistId");

                    b.Property<long>("Popularity");

                    b.Property<long>("TrackNumber");

                    b.Property<int?>("TracksId");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ArtistId");

                    b.HasIndex("PlaylistId");

                    b.HasIndex("TracksId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("Models.BackEnd.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("ImageUrl");

                    b.Property<DateTime>("LastActiveTime");

                    b.Property<long>("Limit");

                    b.Property<string>("Name");

                    b.Property<long>("Popularity");

                    b.Property<long>("Total");

                    b.HasKey("Id");

                    b.ToTable("Playlist");
                });

            modelBuilder.Entity("Models.BackEnd.Tracks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Total");

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("Models.BackEnd.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("Birthdate");

                    b.Property<string>("Country");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email");

                    b.Property<string>("Images");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Models.BackEnd.Album", b =>
                {
                    b.HasOne("Models.BackEnd.Artist")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistId");

                    b.HasOne("Models.BackEnd.Copyright")
                        .WithMany("Albums")
                        .HasForeignKey("CopyrightId");
                });

            modelBuilder.Entity("Models.BackEnd.Image", b =>
                {
                    b.HasOne("Models.BackEnd.Album")
                        .WithMany("Images")
                        .HasForeignKey("AlbumId");

                    b.HasOne("Models.BackEnd.Artist")
                        .WithMany("Images")
                        .HasForeignKey("ArtistId");
                });

            modelBuilder.Entity("Models.BackEnd.Item", b =>
                {
                    b.HasOne("Models.BackEnd.Album")
                        .WithMany("Songs")
                        .HasForeignKey("AlbumId");

                    b.HasOne("Models.BackEnd.Artist")
                        .WithMany("Songs")
                        .HasForeignKey("ArtistId");

                    b.HasOne("Models.BackEnd.Playlist")
                        .WithMany("Items")
                        .HasForeignKey("PlaylistId");

                    b.HasOne("Models.BackEnd.Tracks")
                        .WithMany("Items")
                        .HasForeignKey("TracksId");
                });
#pragma warning restore 612, 618
        }
    }
}
