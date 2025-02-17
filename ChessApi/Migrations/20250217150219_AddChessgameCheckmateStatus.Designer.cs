﻿// <auto-generated />
using ChessApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChessApi.Migrations
{
    [DbContext(typeof(ChessDbContext))]
    [Migration("20250217150219_AddChessgameCheckmateStatus")]
    partial class AddChessgameCheckmateStatus
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChessApi.Models.ChessGame", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("IsCheckmate")
                        .HasColumnType("boolean");

                    b.Property<int>("Turn")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ChessGames");
                });

            modelBuilder.Entity("ChessLib.Move", b =>
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
                        .HasMaxLength(21)
                        .HasColumnType("character varying(21)");

                    b.Property<int>("Src")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ChessGameId");

                    b.ToTable("Move");

                    b.HasDiscriminator<string>("MoveType").HasValue("Move");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("ChessLib.CastlingMove", b =>
                {
                    b.HasBaseType("ChessLib.Move");

                    b.HasDiscriminator().HasValue("CastlingMove");
                });

            modelBuilder.Entity("ChessLib.EnpassantCapture", b =>
                {
                    b.HasBaseType("ChessLib.Move");

                    b.HasDiscriminator().HasValue("EnpassantCapture");
                });

            modelBuilder.Entity("ChessLib.PawnDoubleMove", b =>
                {
                    b.HasBaseType("ChessLib.Move");

                    b.HasDiscriminator().HasValue("PawnDoubleMove");
                });

            modelBuilder.Entity("ChessLib.PromotionMove", b =>
                {
                    b.HasBaseType("ChessLib.Move");

                    b.HasDiscriminator().HasValue("PromotionMove");
                });

            modelBuilder.Entity("ChessLib.Move", b =>
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
