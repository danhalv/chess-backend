using ChessApi.Models;

namespace ChessApi.Tests;

public class ChessApiModelsTests
{
  [Theory]
  [InlineData(0, Color.Black)]
  [InlineData(1, Color.White)]
  [InlineData(8, Color.White)]
  [InlineData(64, Color.Black)]
  public void TestTileColorConstructor(int tileIndex, Color expected)
  {
    var tile = new Tile(tileIndex);

    var actual = tile.Color;

    Assert.Equal(actual, expected);
  }

  [Theory, MemberData(nameof(TestChessBoardStringConstructorData))]
  public void TestChessBoardStringConstructor(string boardString,
                                              Color playerColor,
                                              string tileString,
                                              IPiece? expected)
  {
    var board = new Board(playerColor, boardString);

    var actual = board.GetTile(tileString).Piece;

    if (expected == null)
    {
      Assert.Null(actual);
    }
    else
    {
      Assert.NotNull(actual);
      Assert.Equal(actual.Color, expected.Color);
      Assert.Equal(actual.GetType(), expected.GetType());
    }
  }

  public static IEnumerable<object[]> TestChessBoardStringConstructorData()
  {
    var boardString = """
    _______P
    ________
    ________
    ________
    ________
    P_______
    ________
    p_______
    """
    .Replace("\n", String.Empty);

    yield return new object[] { boardString, Color.White, "a1", new Pawn(Color.Black) };
    yield return new object[] { boardString, Color.White, "h8", new Pawn(Color.White) };
    yield return new object[] { boardString, Color.White, "a3", new Pawn(Color.White) };
    yield return new object[] { boardString, Color.Black, "a1", new Pawn(Color.Black) };
    yield return new object[] { boardString, Color.Black, "h8", new Pawn(Color.White) };
    yield return new object[] { boardString, Color.Black, "e5", null! };
  }

  [Theory, MemberData(nameof(TestPossiblePawnMovesData))]
  public void TestPossiblePawnMoves(string boardString,
                                    Color playerColor,
                                    string pawnTileStr,
                                    List<Move> expected)
  {
    var board = new Board(playerColor, boardString);
    board.GetTile("f4").Piece!.HasMoved = true;
    board.GetTile("b5").Piece!.HasMoved = true;
    ((Pawn)board.GetTile("g4").Piece!).IsEnpassantable = true;
    ((Pawn)board.GetTile("c5").Piece!).IsEnpassantable = true;

    Tile pawnTile = board.GetTile(pawnTileStr);
    List<Move> actual = pawnTile.Piece!.GetMoves(board, pawnTile.Index);

    Assert.Equal(expected.Count, actual.Count);

    actual = actual.OrderBy(m => m.To).ToList();
    expected = expected.OrderBy(m => m.To).ToList();
    for (int i = 0; i < actual.Count; i++)
    {
      Assert.Equal(expected[i].From, actual[i].From);
      Assert.Equal(expected[i].To, actual[i].To);
    }
  }

  public static IEnumerable<object[]> TestPossiblePawnMovesData()
  {
    var boardString = """
    ________
    ______p_
    _____p_P
    _Pp___p_
    _____pP_
    p_p_____
    _P______
    ________
    """
    .Replace("\n", String.Empty);

    var b2WhitePawnMoves = new List<Move>()
    {
      new Move("b2", "b3"), // one step forwards
      new Move("b2", "b4"), // two step forwards
      new Move("b2", "a3"), // capture
      new Move("b2", "c3")  // capture
    };

    var g7BlackPawnMoves = new List<Move>()
    {
      new Move("g7", "g6"), // one step forwards
      new Move("g7", "h6")  // capture
    };

    var f4BlackPawnMoves = new List<Move>()
    {
      new Move("f4", "f3"), // one step forwards
      new Move("f4", "g3")  // en passant capture
    };

    var b5WhitePawnMoves = new List<Move>()
    {
      new Move("b5", "b6"),  // one step forwads
      new Move("b5", "c6")   // en passant capture
    };

    yield return new object[] { boardString, Color.White, "b2", b2WhitePawnMoves };
    yield return new object[] { boardString, Color.White, "g7", g7BlackPawnMoves };
    yield return new object[] { boardString, Color.White, "f4", f4BlackPawnMoves };
    yield return new object[] { boardString, Color.White, "b5", b5WhitePawnMoves };
  }
}
