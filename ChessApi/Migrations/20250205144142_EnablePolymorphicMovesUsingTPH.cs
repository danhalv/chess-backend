using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChessApi.Migrations
{
  /// <inheritdoc />
  public partial class EnablePolymorphicMovesUsingTPH : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "ChessMove");

      migrationBuilder.CreateTable(
          name: "Move",
          columns: table => new
          {
            Id = table.Column<long>(type: "bigint", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            Src = table.Column<int>(type: "integer", nullable: false),
            Dst = table.Column<int>(type: "integer", nullable: false),
            ChessGameId = table.Column<long>(type: "bigint", nullable: true),
            MoveType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Move", x => x.Id);
            table.ForeignKey(
                      name: "FK_Move_ChessGames_ChessGameId",
                      column: x => x.ChessGameId,
                      principalTable: "ChessGames",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Move_ChessGameId",
          table: "Move",
          column: "ChessGameId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Move");

      migrationBuilder.CreateTable(
          name: "ChessMove",
          columns: table => new
          {
            Id = table.Column<long>(type: "bigint", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            ChessGameId = table.Column<long>(type: "bigint", nullable: true),
            Dst = table.Column<int>(type: "integer", nullable: false),
            Src = table.Column<int>(type: "integer", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ChessMove", x => x.Id);
            table.ForeignKey(
                      name: "FK_ChessMove_ChessGames_ChessGameId",
                      column: x => x.ChessGameId,
                      principalTable: "ChessGames",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_ChessMove_ChessGameId",
          table: "ChessMove",
          column: "ChessGameId");
    }
  }
}
