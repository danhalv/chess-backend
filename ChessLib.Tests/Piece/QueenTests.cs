using ChessLib;

namespace ChessLib.Tests;

public class QueenTests
{
  [Theory, MemberData(nameof(TestQueenMovesData))]
  public void TestQueenMoves(Board board, string queenTilePosition, List<Move> expected)
  {
    // Arrange
    Tile queenTile = board.GetTile(queenTilePosition);

    // Act
    var actual = queenTile.Piece.GetMoves(board, queenTile.Index);

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestQueenMovesData()
  {
    var boardString = "________" +
                      "________" +
                      "__q_p___" +
                      "_____P__" +
                      "__p_____" +
                      "___P_Q__" +
                      "________" +
                      "________";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whiteQueenTile = "f3";
    var blackQueenTile = "c6";

    var whiteQueenMoves = new List<Move>()
    {
      new Move(whiteQueenTile, "c6"), // capture
      new Move(whiteQueenTile, "d1"),
      new Move(whiteQueenTile, "d5"),
      new Move(whiteQueenTile, "e2"),
      new Move(whiteQueenTile, "e3"),
      new Move(whiteQueenTile, "e4"),
      new Move(whiteQueenTile, "f1"),
      new Move(whiteQueenTile, "f2"),
      new Move(whiteQueenTile, "f4"),
      new Move(whiteQueenTile, "g2"),
      new Move(whiteQueenTile, "g3"),
      new Move(whiteQueenTile, "g4"),
      new Move(whiteQueenTile, "h1"),
      new Move(whiteQueenTile, "h3"),
      new Move(whiteQueenTile, "h5")
    };

    var blackQueenMoves = new List<Move>()
    {
      new Move(blackQueenTile, "a4"),
      new Move(blackQueenTile, "a6"),
      new Move(blackQueenTile, "a8"),
      new Move(blackQueenTile, "b5"),
      new Move(blackQueenTile, "b6"),
      new Move(blackQueenTile, "b7"),
      new Move(blackQueenTile, "c5"),
      new Move(blackQueenTile, "c7"),
      new Move(blackQueenTile, "c8"),
      new Move(blackQueenTile, "d5"),
      new Move(blackQueenTile, "d6"),
      new Move(blackQueenTile, "d7"),
      new Move(blackQueenTile, "e4"),
      new Move(blackQueenTile, "e8"),
      new Move(blackQueenTile, "f3")  // capture
    };

    yield return new object[] { whiteToPlayBoard, whiteQueenTile, whiteQueenMoves };
    yield return new object[] { blackToPlayBoard, blackQueenTile, blackQueenMoves };
  }
}
