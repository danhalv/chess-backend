using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessApi.Migrations
{
  /// <inheritdoc />
  public partial class AddChessgameCheckmateStatus : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<bool>(
          name: "IsCheckmate",
          table: "ChessGames",
          type: "boolean",
          nullable: false,
          defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "IsCheckmate",
          table: "ChessGames");
    }
  }
}
