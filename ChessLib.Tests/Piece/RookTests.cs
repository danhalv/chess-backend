using ChessLib;

namespace ChessLib.Tests;

public class RookTests
{
  [Theory, MemberData(nameof(TestRookMovesData))]
  public void TestRookMoves(Board board, string rookTilePosition, List<Move> expected)
  {
    // Arrange
    Tile rookTile = board.GetTile(rookTilePosition);

    // Act
    var actual = rookTile.Piece.GetMoves(board, rookTile.Index);

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestRookMovesData()
  {
    var boardString = "________" +
                      "________" +
                      "__p___r_" +
                      "________" +
                      "________" +
                      "__P___R_" +
                      "________" +
                      "________";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whiteRookTile = "g3";
    var blackRookTile = "g6";

    var whiteRookMoves = new List<Move>()
    {
      new Move(whiteRookTile, "d3"),
      new Move(whiteRookTile, "e3"),
      new Move(whiteRookTile, "f3"),
      new Move(whiteRookTile, "g1"),
      new Move(whiteRookTile, "g2"),
      new Move(whiteRookTile, "g4"),
      new Move(whiteRookTile, "g5"),
      new Move(whiteRookTile, "g6"),
      new Move(whiteRookTile, "h3")
    };

    var blackRookMoves = new List<Move>()
    {
      new Move(blackRookTile, "d6"),
      new Move(blackRookTile, "e6"),
      new Move(blackRookTile, "f6"),
      new Move(blackRookTile, "g3"),
      new Move(blackRookTile, "g4"),
      new Move(blackRookTile, "g5"),
      new Move(blackRookTile, "g7"),
      new Move(blackRookTile, "g8"),
      new Move(blackRookTile, "h6")
    };

    yield return new object[] { whiteToPlayBoard, whiteRookTile, whiteRookMoves };
    yield return new object[] { blackToPlayBoard, blackRookTile, blackRookMoves };
  }
}
