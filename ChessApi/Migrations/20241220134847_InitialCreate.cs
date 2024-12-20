using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChessApi.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "ChessGames",
          columns: table => new
          {
            Id = table.Column<long>(type: "bigint", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            Turn = table.Column<int>(type: "integer", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ChessGames", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "ChessMove",
          columns: table => new
          {
            Id = table.Column<long>(type: "bigint", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            ChessGameId = table.Column<long>(type: "bigint", nullable: true),
            Src = table.Column<int>(type: "integer", nullable: false),
            Dst = table.Column<int>(type: "integer", nullable: false)
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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "ChessMove");

      migrationBuilder.DropTable(
          name: "ChessGames");
    }
  }
}
