﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tf2Matchmaker.Database;

#nullable disable

namespace Tf2Matchmaker.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240608170648_AddFetchedTimes")]
    partial class AddFetchedTimes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tf2Matchmaker.Common.Servers.TFServer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<short>("AppId")
                        .HasColumnType("smallint");

                    b.Property<byte>("Bots")
                        .HasColumnType("smallint");

                    b.Property<int>("Environment")
                        .HasColumnType("integer");

                    b.Property<string>("Folder")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Game")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("GameId")
                        .HasColumnType("bigint");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string[]>("Keywords")
                        .HasColumnType("text[]");

                    b.Property<DateTimeOffset>("LastFetched")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Map")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte>("MaxPlayers")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte>("Players")
                        .HasColumnType("smallint");

                    b.Property<short?>("Port")
                        .HasColumnType("smallint");

                    b.Property<byte>("Protocol")
                        .HasColumnType("smallint");

                    b.Property<int>("ServerPort")
                        .HasColumnType("integer");

                    b.Property<long?>("ServerSteamId")
                        .HasColumnType("bigint");

                    b.Property<short?>("SpectatorPort")
                        .HasColumnType("smallint");

                    b.Property<string>("SpectatorServerName")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<int>("Vac")
                        .HasColumnType("integer");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Visibility")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("Tf2Matchmaker.Common.Servers.TFServerPlayerList", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastFetched")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PlayersJson")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int>("ServerPort")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("PlayerLists");
                });

            modelBuilder.Entity("Tf2Matchmaker.Common.Servers.TFServerRules", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastFetched")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Port")
                        .HasColumnType("integer");

                    b.Property<string>("RulesString")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("Rules");
                });
#pragma warning restore 612, 618
        }
    }
}
