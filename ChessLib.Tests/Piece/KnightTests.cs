using ChessLib;

namespace ChessLib.Tests;

public class KnightTests
{
  [Theory, MemberData(nameof(TestKnightMovesData))]
  public void TestKnightMoves(Board board, string knightTilePosition, List<Move> expected)
  {
    // Arrange
    Tile knightTile = board.GetTile(knightTilePosition);

    // Act
    var actual = knightTile.Piece.GetMoves(board, knightTile.Index);

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestKnightMovesData()
  {
    var boardString = "_p_P____" +
                      "________" +
                      "__n_____" +
                      "________" +
                      "_____p__" +
                      "____P___" +
                      "______N_" +
                      "________";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whiteKnightTile = "g2";
    var blackKnightTile = "c6";

    var whiteKnightMoves = new List<Move>()
    {
      new Move(whiteKnightTile, "h4"),
      new Move(whiteKnightTile, "f4"), // capture
      new Move(whiteKnightTile, "e1")
    };

    var blackKnightMoves = new List<Move>()
    {
      new Move(blackKnightTile, "a5"),
      new Move(blackKnightTile, "a7"),
      new Move(blackKnightTile, "d8"), // capture
      new Move(blackKnightTile, "e7"),
      new Move(blackKnightTile, "e5"),
      new Move(blackKnightTile, "d4"),
      new Move(blackKnightTile, "b4")
    };

    yield return new object[] { whiteToPlayBoard, whiteKnightTile, whiteKnightMoves };
    yield return new object[] { blackToPlayBoard, blackKnightTile, blackKnightMoves };
  }
}
