﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessApi.Migrations
{
  /// <inheritdoc />
  public partial class EnpassantMoveSubclasses : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<string>(
          name: "MoveType",
          table: "Move",
          type: "character varying(21)",
          maxLength: 21,
          nullable: false,
          oldClrType: typeof(string),
          oldType: "character varying(13)",
          oldMaxLength: 13);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<string>(
          name: "MoveType",
          table: "Move",
          type: "character varying(13)",
          maxLength: 13,
          nullable: false,
          oldClrType: typeof(string),
          oldType: "character varying(21)",
          oldMaxLength: 21);
    }
  }
}
