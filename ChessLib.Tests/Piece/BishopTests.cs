using ChessLib;

namespace ChessLib.Tests;

public class BishopTests
{
  [Theory, MemberData(nameof(TestBishopMovesData))]
  public void TestBishopMoves(Board board, string bishopTilePosition, List<Move> expected)
  {
    // Arrange
    Tile bishopTile = board.GetTile(bishopTilePosition);

    // Act
    var actual = bishopTile.Piece.GetMoves(board, bishopTile.Index);

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestBishopMovesData()
  {
    var boardString = "_______P" +
                      "________" +
                      "_____B__" +
                      "________" +
                      "________" +
                      "__b_____" +
                      "________" +
                      "p_______";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whiteBishopTile = "c3";
    var blackBishopTile = "f6";

    var whiteBishopMoves = new List<Move>()
    {
      new Move(whiteBishopTile, "b2"),
      new Move(whiteBishopTile, "b4"),
      new Move(whiteBishopTile, "a5"),
      new Move(whiteBishopTile, "d2"),
      new Move(whiteBishopTile, "e1"),
      new Move(whiteBishopTile, "d4"),
      new Move(whiteBishopTile, "e5"),
      new Move(whiteBishopTile, "f6")
    };

    var blackBishopMoves = new List<Move>()
    {
      new Move(blackBishopTile, "g7"),
      new Move(blackBishopTile, "e7"),
      new Move(blackBishopTile, "d8"),
      new Move(blackBishopTile, "g5"),
      new Move(blackBishopTile, "h4"),
      new Move(blackBishopTile, "e5"),
      new Move(blackBishopTile, "d4"),
      new Move(blackBishopTile, "c3")
    };

    yield return new object[] { whiteToPlayBoard, whiteBishopTile, whiteBishopMoves };
    yield return new object[] { blackToPlayBoard, blackBishopTile, blackBishopMoves };
  }
}
