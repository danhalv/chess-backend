using ChessLib;

namespace ChessLib.Tests;

public class PawnTests
{
  [Theory, MemberData(nameof(TestPawnForwardMovesData))]
  public void TestPawnForwardMoves(Board board, string pawnTilePosition, List<Move> expected)
  {
    // Arrange
    Tile pawnTile = board.GetTile(pawnTilePosition);

    // Act
    var actual = pawnTile.Piece.GetMoves(board, pawnTile.Index);

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestPawnForwardMovesData()
  {
    var boardString = "________" +
                      "_p______" +
                      "________" +
                      "_____p__" +
                      "________" +
                      "________" +
                      "_P______" +
                      "________";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);
    var blackToPlayBoardHasMoved = new Board(Color.Black, boardString);

    var whitePawnTile = "b2";
    var blackPawnTile = "b7";
    var blackPawnHasMovedTile = "f5";

    var whitePawnMoves = new List<Move>()
    {
      new Move(whitePawnTile, "b3"),
      new Move(whitePawnTile, "b4")
    };

    var blackPawnMoves = new List<Move>()
    {
      new Move(blackPawnTile, "b6"),
      new Move(blackPawnTile, "b5")
    };

    var blackPawnHasMovedMoves = new List<Move>()
    {
      new Move(blackPawnHasMovedTile, "f4")
    };

    blackToPlayBoardHasMoved.GetTile(blackPawnHasMovedTile).Piece!.HasMoved = true;

    yield return new object[] { whiteToPlayBoard, whitePawnTile, whitePawnMoves };
    yield return new object[] { blackToPlayBoard, blackPawnTile, blackPawnMoves };
    yield return new object[] { blackToPlayBoardHasMoved, blackPawnHasMovedTile, blackPawnHasMovedMoves };
  }

  [Theory, MemberData(nameof(TestPawnCaptureMovesData))]
  public void TestPawnCaptureMoves(Board board, string pawnTilePosition, List<Move> expected)
  {
    // Arrange
    Tile pawnTile = board.GetTile(pawnTilePosition);

    // Act
    var actual = pawnTile.Piece.GetMoves(board, pawnTile.Index);

    // Assert
    foreach (var move in expected)
    {
      Assert.Contains(move, actual);
    }
  }

  public static IEnumerable<object[]> TestPawnCaptureMovesData()
  {
    var boardString = "________" +
                      "___p____" +
                      "__r_R___" +
                      "________" +
                      "____r_r_" +
                      "_____P__" +
                      "________" +
                      "________";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whitePawnTile = "f3";
    var blackPawnTile = "d7";

    var whitePawnMoves = new List<Move>()
    {
      new Move(whitePawnTile, "e4"),
      new Move(whitePawnTile, "g4")
    };

    var blackPawnMoves = new List<Move>()
    {
      new Move(blackPawnTile, "e6")
    };

    yield return new object[] { whiteToPlayBoard, whitePawnTile, whitePawnMoves };
    yield return new object[] { blackToPlayBoard, blackPawnTile, blackPawnMoves };
  }

  [Theory, MemberData(nameof(TestPawnEnPassantCaptureMovesData))]
  public void TestPawnEnPassantCaptureMoves(Board board, string pawnTilePosition, List<Move> expected)
  {
    // Arrange
    Tile pawnTile = board.GetTile(pawnTilePosition);

    // Act
    var actual = pawnTile.Piece.GetMoves(board, pawnTile.Index);

    // Assert
    foreach (var move in expected)
    {
      Assert.Contains(move, actual);
    }
  }

  public static IEnumerable<object[]> TestPawnEnPassantCaptureMovesData()
  {
    var boardString = "________" +
                      "____rpP_" +
                      "________" +
                      "________" +
                      "________" +
                      "__pPp___" +
                      "________" +
                      "________";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whitePawnTile = "d3";
    var blackPawnTile = "f7";

    var whitePawnMoves = new List<Move>()
    {
      new Move(whitePawnTile, "e4") // right en passant
    };

    var blackPawnMoves = new List<Move>()
    {
      new Move(blackPawnTile, "g6") // left en passant
    };

    ((Pawn)whiteToPlayBoard.GetTile(Tile.StringToIndex(whitePawnTile) + 1).Piece!).IsEnpassantable = true;
    ((Pawn)blackToPlayBoard.GetTile(Tile.StringToIndex(blackPawnTile) + 1).Piece!).IsEnpassantable = true;

    yield return new object[] { whiteToPlayBoard, whitePawnTile, whitePawnMoves };
    yield return new object[] { blackToPlayBoard, blackPawnTile, blackPawnMoves };
  }

  [Theory, MemberData(nameof(TestPawnPromotionMovesData))]
  public void TestPawnPromotionMoves(Board board, string pawnTilePosition, List<Move> expected)
  {
    // Arrange
    Tile pawnTile = board.GetTile(pawnTilePosition);

    // Act
    var actual = pawnTile.Piece.GetMoves(board, pawnTile.Index);
    actual = actual.OfType<PromotionMove>().Cast<Move>().ToList();

    // Assert
    Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
  }

  public static IEnumerable<object[]> TestPawnPromotionMovesData()
  {
    var boardString = "__n_____" +
                      "_P______" +
                      "________" +
                      "________" +
                      "________" +
                      "________" +
                      "______p_" +
                      "_____n__";

    var whiteToPlayBoard = new Board(Color.White, boardString);
    var blackToPlayBoard = new Board(Color.Black, boardString);

    var whitePawnTile = "b7";
    var blackPawnTile = "g2";

    var whitePawnMoves = new List<Move>()
    {
      new PromotionMove(whitePawnTile, "b8"),
      new PromotionMove(whitePawnTile, "c8")
    };

    var blackPawnMoves = new List<Move>()
    {
      new PromotionMove(blackPawnTile, "g1")
    };

    yield return new object[] { whiteToPlayBoard, whitePawnTile, whitePawnMoves };
    yield return new object[] { blackToPlayBoard, blackPawnTile, blackPawnMoves };
  }
}
