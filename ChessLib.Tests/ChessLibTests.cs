using ChessLib;

namespace ChessLib.Tests;

public class ChessLibTests
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
                                              string tileString,
                                              IPiece? expected)
  {
    var board = new Board(Color.White, boardString);

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

    yield return new object[] { boardString, "a1", new Pawn(Color.Black) };
    yield return new object[] { boardString, "h8", new Pawn(Color.White) };
    yield return new object[] { boardString, "a3", new Pawn(Color.White) };
    yield return new object[] { boardString, "a1", new Pawn(Color.Black) };
    yield return new object[] { boardString, "h8", new Pawn(Color.White) };
    yield return new object[] { boardString, "e5", null! };
  }

  [Theory, MemberData(nameof(TestChecksData))]
  public void TestChecks(string boardString,
                         Color playerTurn,
                         bool expected)
  {
    var board = new Board(playerTurn, boardString);

    bool actual = board.IsCheck(playerTurn);

    Assert.Equal(expected, actual);
  }

  public static IEnumerable<object[]> TestChecksData()
  {
    var whiteInCheckBoardStr = """
    ________
    ________
    _____P__
    ________
    ___b____
    __K_____
    _____r__
    ________
    """
    .Replace("\n", String.Empty);

    var blackInCheckBoardStr = """
    ___K____
    ________
    _____P__
    ________
    ___b____
    ________
    _____r__
    Rk______
    """
    .Replace("\n", String.Empty);

    yield return new object[] { whiteInCheckBoardStr, Color.White, true };
    yield return new object[] { blackInCheckBoardStr, Color.White, false };
    yield return new object[] { blackInCheckBoardStr, Color.Black, true };
  }

  [Theory, MemberData(nameof(TestCheckmateData))]
  public void TestCheckmate(string boardString,
                            Color playerTurn,
                            bool expected)
  {
    var board = new Board(playerTurn, boardString);

    board.UpdateCheckmate();
    bool actual = board.IsCheckmate;

    Assert.Equal(expected, actual);
  }

  public static IEnumerable<object[]> TestCheckmateData()
  {
    var checkmateBoardStr = """
    k_______
    ________
    RR______
    ________
    ______rr
    ________
    ________
    _______K
    """
    .Replace("\n", String.Empty);

    var notCheckmateBoardStr = """
    k_______
    ________
    R_______
    ________
    _______r
    ________
    ________
    _______K
    """
    .Replace("\n", String.Empty);

    yield return new object[] { checkmateBoardStr, Color.White, true };
    yield return new object[] { checkmateBoardStr, Color.Black, true };
    yield return new object[] { notCheckmateBoardStr, Color.White, false };
    yield return new object[] { notCheckmateBoardStr, Color.Black, false };
  }

  [Theory, MemberData(nameof(TestLegalMovesData))]
  public void TestLegalMoves(string boardString,
                             Color playerTurn,
                             string tileStr,
                             List<Move> expected)
  {
    var board = new Board(playerTurn, boardString);

    var actual = (tileStr == "")
                 ? board.LegalMoves()
                 : board.LegalMoves(tileStr);

    Assert.Equal(expected.Count, actual.Count);

    actual = actual.OrderBy(m => m.Dst).ToList();
    expected = expected.OrderBy(m => m.Dst).ToList();
    for (int i = 0; i < actual.Count; i++)
    {
      Assert.Equal(expected[i].Src, actual[i].Src);
      Assert.Equal(expected[i].Dst, actual[i].Dst);
    }
  }

  public static IEnumerable<object[]> TestLegalMovesData()
  {
    var boardStr = """
    ________
    ________
    ________
    _____R_R
    ________
    ______k_
    _p______
    ________
    """
    .Replace("\n", String.Empty);

    var blackLegalMoves = new List<Move>()
    {
      new Move("g3", "g2"), // king moves
      new Move("g3", "g4"),
      new Move("b2", "b1")  // pawn moves
    };

    var blockingCheckBoardStr = """
    b_______
    _Q______
    __K_____
    ________
    ________
    ________
    ________
    ________
    """
    .Replace("\n", String.Empty);

    var b7WhiteQueenLegalMoves = new List<Move>()
    {
      new Move("b7", "a8")
    };

    yield return new object[] { boardStr, Color.Black, "", blackLegalMoves };
    yield return new object[] { blockingCheckBoardStr, Color.White, "b7", b7WhiteQueenLegalMoves };
  }
}
