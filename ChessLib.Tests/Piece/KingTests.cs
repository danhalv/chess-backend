using ChessLib;

namespace ChessLib.Tests;

public class KingTests
{
  [Theory, MemberData(nameof(TestKingRegularMovesData))]
  public void TestKingRegularMoves(Board board, string kingTilePosition, List<Move> expected)
  {
    // Arrange
    Tile kingTile = board.GetTile(kingTilePosition);

    // Act
    var actual = kingTile.Piece.GetMoves(board, kingTile.Index);

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestKingRegularMovesData()
  {
    var boardString = "________" +
                      "________" +
                      "________" +
                      "________" +
                      "________" +
                      "P_k_____" +
                      "_K_p____" +
                      "________";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whiteKingTile = "b2";
    var blackKingTile = "c3";

    var whiteKingMoves = new List<Move>()
    {
      new Move(whiteKingTile, "a1"),
      new Move(whiteKingTile, "a2"),
      new Move(whiteKingTile, "b1"),
      new Move(whiteKingTile, "b3"),
      new Move(whiteKingTile, "c1"),
      new Move(whiteKingTile, "c2"),
      new Move(whiteKingTile, "c3")  // capture
    };

    var blackKingMoves = new List<Move>()
    {
      new Move(blackKingTile, "b2"), // capture
      new Move(blackKingTile, "b3"),
      new Move(blackKingTile, "b4"),
      new Move(blackKingTile, "c2"),
      new Move(blackKingTile, "c4"),
      new Move(blackKingTile, "d3"),
      new Move(blackKingTile, "d4")
    };

    yield return new object[] { whiteToPlayBoard, whiteKingTile, whiteKingMoves };
    yield return new object[] { blackToPlayBoard, blackKingTile, blackKingMoves };
  }

  [Theory, MemberData(nameof(TestKingLegalCastlingMovesData))]
  public void TestKingLegalCastlingMoves(Board board, string kingTilePosition, List<Move> expected)
  {
    // Arrange
    Tile kingTile = board.GetTile(kingTilePosition);

    // Act
    var actual = kingTile.Piece.GetMoves(board, kingTile.Index);
    actual = actual.OfType<CastlingMove>().Cast<Move>().ToList();

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestKingLegalCastlingMovesData()
  {
    var boardString = "r___k__R" +
                      "________" +
                      "________" +
                      "________" +
                      "________" +
                      "________" +
                      "________" +
                      "R___K__R";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whiteKingTile = "e1";
    var blackKingTile = "e8";

    var whiteCastlingMoves = new List<Move>()
    {
      new CastlingMove(whiteKingTile, "a1"),
      new CastlingMove(whiteKingTile, "h1")
    };

    var blackCastlingMoves = new List<Move>()
    {
      new CastlingMove(blackKingTile, "a8")
    };

    yield return new object[] { whiteToPlayBoard, whiteKingTile, whiteCastlingMoves };
    yield return new object[] { blackToPlayBoard, blackKingTile, blackCastlingMoves };
  }

  [Theory, MemberData(nameof(TestKingIllegalCastlingMovesData))]
  public void TestKingIllegalCastlingMoves(Board board, string kingTilePosition, List<Move> expected)
  {
    // Arrange
    Tile kingTile = board.GetTile(kingTilePosition);

    // Act
    var actual = kingTile.Piece.GetMoves(board, kingTile.Index);
    actual = actual.OfType<CastlingMove>().Cast<Move>().ToList();

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestKingIllegalCastlingMovesData()
  {
    var boardString = "r___k__r" +
                      "________" +
                      "________" +
                      "________" +
                      "________" +
                      "________" +
                      "________" +
                      "R_P_K__R";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whiteKingTile = "e1";
    var blackKingTile = "e8";

    whiteToPlayBoard.GetTile("h1").Piece!.HasMoved = true;
    blackToPlayBoard.GetTile(blackKingTile).Piece.HasMoved = true;

    yield return new object[] { whiteToPlayBoard, whiteKingTile, new List<Move>() };
    yield return new object[] { blackToPlayBoard, blackKingTile, new List<Move>() };
  }
}
