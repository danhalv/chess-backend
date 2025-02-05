﻿// <auto-generated />
using ChessApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChessApi.Migrations
{
    [DbContext(typeof(ChessDbContext))]
    partial class ChessDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChessApi.Models.Chess.Move", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("ChessGameId")
                        .HasColumnType("bigint");

                    b.Property<int>("Dst")
                        .HasColumnType("integer");

                    b.Property<string>("MoveType")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

                    b.Property<int>("Src")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ChessGameId");

                    b.ToTable("Move");

                    b.HasDiscriminator<string>("MoveType").HasValue("Move");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("ChessApi.Models.ChessGame", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("Turn")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ChessGames");
                });

            modelBuilder.Entity("ChessApi.Models.Chess.CastlingMove", b =>
                {
                    b.HasBaseType("ChessApi.Models.Chess.Move");

                    b.HasDiscriminator().HasValue("CastlingMove");
                });

            modelBuilder.Entity("ChessApi.Models.Chess.PromotionMove", b =>
                {
                    b.HasBaseType("ChessApi.Models.Chess.Move");

                    b.HasDiscriminator().HasValue("PromotionMove");
                });

            modelBuilder.Entity("ChessApi.Models.Chess.Move", b =>
                {
                    b.HasOne("ChessApi.Models.ChessGame", null)
                        .WithMany("Moves")
                        .HasForeignKey("ChessGameId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ChessApi.Models.ChessGame", b =>
                {
                    b.Navigation("Moves");
                });
#pragma warning restore 612, 618
        }
    }
}
