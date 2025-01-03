using ChessApi.Models.Chess;

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

    actual = actual.OrderBy(m => m.Dst).ToList();
    expected = expected.OrderBy(m => m.Dst).ToList();
    for (int i = 0; i < actual.Count; i++)
    {
      Assert.Equal(expected[i].Src, actual[i].Src);
      Assert.Equal(expected[i].Dst, actual[i].Dst);
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

  [Theory, MemberData(nameof(TestPossibleKnightMovesData))]
  public void TestPossibleKnightMoves(string boardString,
                                      Color playerColor,
                                      string knightTileString,
                                      List<Move> expected)
  {
    var board = new Board(playerColor, boardString);

    Tile knightTile = board.GetTile(knightTileString);
    List<Move> actual = knightTile.Piece!.GetMoves(board, knightTile.Index);

    Assert.Equal(expected.Count, actual.Count);

    actual = actual.OrderBy(m => m.Dst).ToList();
    expected = expected.OrderBy(m => m.Dst).ToList();
    for (int i = 0; i < actual.Count; i++)
    {
      Assert.Equal(expected[i].Src, actual[i].Src);
      Assert.Equal(expected[i].Dst, actual[i].Dst);
    }
  }

  public static IEnumerable<object[]> TestPossibleKnightMovesData()
  {
    var boardString = """
    ________
    ________
    ________
    ____N___
    ________
    _n_n_N__
    __N_____
    n_______
    """
    .Replace("\n", String.Empty);

    var e5WhiteKnightMoves = new List<Move>()
    {
      new Move("e5", "d7"),
      new Move("e5", "f7"),
      new Move("e5", "d3"),
      new Move("e5", "c4"),
      new Move("e5", "c6"),
      new Move("e5", "g4"),
      new Move("e5", "g6")
    };

    var a1BlackKnightMoves = new List<Move>()
    {
      new Move("a1", "c2")
    };

    yield return new object[] { boardString, Color.White, "e5", e5WhiteKnightMoves };
    yield return new object[] { boardString, Color.White, "a1", a1BlackKnightMoves };
  }

  [Theory, MemberData(nameof(TestPossibleBishopMovesData))]
  public void TestPossibleBishopMoves(string boardString,
                                      Color playerColor,
                                      string bishopTileString,
                                      List<Move> expected)
  {
    var board = new Board(playerColor, boardString);

    Tile bishopTile = board.GetTile(bishopTileString);
    List<Move> actual = bishopTile.Piece!.GetMoves(board, bishopTile.Index);

    Assert.Equal(expected.Count, actual.Count);

    actual = actual.OrderBy(m => m.Dst).ToList();
    expected = expected.OrderBy(m => m.Dst).ToList();
    for (int i = 0; i < actual.Count; i++)
    {
      Assert.Equal(expected[i].Src, actual[i].Src);
      Assert.Equal(expected[i].Dst, actual[i].Dst);
    }
  }

  public static IEnumerable<object[]> TestPossibleBishopMovesData()
  {
    var boardString = """
    _______b
    ________
    _____B__
    ___b____
    ________
    _____B__
    ________
    ___B____
    """
    .Replace("\n", String.Empty);

    var f3WhiteBishopMoves = new List<Move>()
    {
      new Move("f3", "g2"), // right backward diagonal
      new Move("f3", "h1"),
      new Move("f3", "e2"), // left backward diagonal
      new Move("f3", "g4"), // right forward diagonal
      new Move("f3", "h5"),
      new Move("f3", "e4"), // left forward diagonal
      new Move("f3", "d5")  // capture
    };

    var h8BlackBishopMoves = new List<Move>()
    {
      new Move("h8", "g7"),
      new Move("h8", "f6")  // capture
    };

    yield return new object[] { boardString, Color.White, "f3", f3WhiteBishopMoves };
    yield return new object[] { boardString, Color.White, "h8", h8BlackBishopMoves };
  }

  [Theory, MemberData(nameof(TestPossibleRookMovesData))]
  public void TestPossibleRookMoves(string boardString,
                                    Color playerColor,
                                    string rookTileString,
                                    List<Move> expected)
  {
    var board = new Board(playerColor, boardString);

    Tile rookTile = board.GetTile(rookTileString);
    List<Move> actual = rookTile.Piece!.GetMoves(board, rookTile.Index);

    Assert.Equal(expected.Count, actual.Count);

    actual = actual.OrderBy(m => m.Dst).ToList();
    expected = expected.OrderBy(m => m.Dst).ToList();
    for (int i = 0; i < actual.Count; i++)
    {
      Assert.Equal(expected[i].Src, actual[i].Src);
      Assert.Equal(expected[i].Dst, actual[i].Dst);
    }
  }

  public static IEnumerable<object[]> TestPossibleRookMovesData()
  {
    var boardString = """
    ________
    __R_____
    _____R__
    ________
    __r__R__
    ________
    __r_____
    ________
    """
    .Replace("\n", String.Empty);

    var c4BlackRookMoves = new List<Move>()
    {
      new Move("c4", "c3"),
      new Move("c4", "c5"),
      new Move("c4", "c6"),
      new Move("c4", "c7"),
      new Move("c4", "d4"),
      new Move("c4", "e4"),
      new Move("c4", "f4"),
      new Move("c4", "b4"),
      new Move("c4", "a4")
    };

    var f4WhiteRookMoves = new List<Move>()
    {
      new Move("f4", "f3"),
      new Move("f4", "f2"),
      new Move("f4", "f1"),
      new Move("f4", "f5"),
      new Move("f4", "g4"),
      new Move("f4", "h4"),
      new Move("f4", "e4"),
      new Move("f4", "d4"),
      new Move("f4", "c4")
    };

    yield return new object[] { boardString, Color.White, "c4", c4BlackRookMoves };
    yield return new object[] { boardString, Color.White, "f4", f4WhiteRookMoves };
  }

  [Theory, MemberData(nameof(TestPossibleQueenMovesData))]
  public void TestPossibleQueenMoves(string boardString,
                                    Color playerColor,
                                    string queenTileString,
                                    List<Move> expected)
  {
    var board = new Board(playerColor, boardString);

    Tile queenTile = board.GetTile(queenTileString);
    List<Move> actual = queenTile.Piece!.GetMoves(board, queenTile.Index);

    Assert.Equal(expected.Count, actual.Count);

    actual = actual.OrderBy(m => m.Dst).ToList();
    expected = expected.OrderBy(m => m.Dst).ToList();
    for (int i = 0; i < actual.Count; i++)
    {
      Assert.Equal(expected[i].Src, actual[i].Src);
      Assert.Equal(expected[i].Dst, actual[i].Dst);
    }
  }

  public static IEnumerable<object[]> TestPossibleQueenMovesData()
  {
    var boardString = """
    ________
    ________
    ________
    ________
    ___Q____
    _q_qqq__
    _Q__qq__
    ________
    """
    .Replace("\n", String.Empty);

    var b2WhiteQueenMoves = new List<Move>()
    {
      new Move("b2", "a1"),
      new Move("b2", "b1"),
      new Move("b2", "c1"),
      new Move("b2", "c2"),
      new Move("b2", "d2"),
      new Move("b2", "e2"),
      new Move("b2", "c3"),
      new Move("b2", "b3"),
      new Move("b2", "a3"),
      new Move("b2", "a2")
    };

    var e2BlackQueenMoves = new List<Move>()
    {
      new Move("e2", "d1"),
      new Move("e2", "e1"),
      new Move("e2", "f1"),
      new Move("e2", "d2"),
      new Move("e2", "c2"),
      new Move("e2", "b2")
    };

    yield return new object[] { boardString, Color.White, "b2", b2WhiteQueenMoves };
    yield return new object[] { boardString, Color.White, "e2", e2BlackQueenMoves };
  }

  [Theory, MemberData(nameof(TestPossibleKingMovesData))]
  public void TestPossibleKingMoves(string boardString,
                                    Color playerColor,
                                    string kingTileString,
                                    List<Move> expected)
  {
    var board = new Board(playerColor, boardString);

    Tile kingTile = board.GetTile(kingTileString);
    List<Move> actual = kingTile.Piece!.GetMoves(board, kingTile.Index);

    Assert.Equal(expected.Count, actual.Count);

    actual = actual.OrderBy(m => m.Dst).ToList();
    expected = expected.OrderBy(m => m.Dst).ToList();
    for (int i = 0; i < actual.Count; i++)
    {
      Assert.Equal(expected[i].Src, actual[i].Src);
      Assert.Equal(expected[i].Dst, actual[i].Dst);
    }
  }

  public static IEnumerable<object[]> TestPossibleKingMovesData()
  {
    var boardString = """
    r___k__R
    ________
    ________
    ________
    __P_____
    __Kp____
    ________
    r___K__R
    """
    .Replace("\n", String.Empty);

    var e8BlackKingMoves = new List<Move>
    {
      new Move("e8", "c8"), // castling move
      new Move("e8", "d8"),
      new Move("e8", "d7"),
      new Move("e8", "e7"),
      new Move("e8", "f7"),
      new Move("e8", "f8")
    };

    var c3WhiteKingMoves = new List<Move>()
    {
      new Move("c3", "b3"),
      new Move("c3", "b2"),
      new Move("c3", "c2"),
      new Move("c3", "d2"),
      new Move("c3", "d3"),
      new Move("c3", "d4"),
      new Move("c3", "b4")
    };

    var e1WhiteKingMoves = new List<Move>()
    {
      new Move("e1", "g1"), // castling move
      new Move("e1", "f1"),
      new Move("e1", "f2"),
      new Move("e1", "e2"),
      new Move("e1", "d2"),
      new Move("e1", "d1")
    };

    yield return new object[] { boardString, Color.White, "e8", e8BlackKingMoves };
    yield return new object[] { boardString, Color.White, "c3", c3WhiteKingMoves };
    yield return new object[] { boardString, Color.White, "e1", e1WhiteKingMoves };
  }

  [Theory, MemberData(nameof(TestChecksData))]
  public void TestChecks(string boardString,
                         Color playerColor,
                         Color playerTurn,
                         bool expected)
  {
    var board = new Board(playerColor, boardString);

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

    yield return new object[] { whiteInCheckBoardStr, Color.White, Color.White, true };
    yield return new object[] { blackInCheckBoardStr, Color.White, Color.White, false };
    yield return new object[] { blackInCheckBoardStr, Color.White, Color.Black, true };
  }

  [Theory, MemberData(nameof(TestCheckmateData))]
  public void TestCheckmate(string boardString,
                            Color playerColor,
                            bool expected)
  {
    var board = new Board(playerColor, boardString);

    bool actual = board.IsCheckmate();

    Assert.Equal(expected, actual);
  }

  public static IEnumerable<object[]> TestCheckmateData()
  {
    var whiteInCheckmateBoardStr = """
    ________
    ________
    ________
    ________
    ______rr
    ________
    ________
    _______K
    """
    .Replace("\n", String.Empty);

    var whiteInCheckBoardStr = """
    ________
    ________
    ________
    ________
    _______r
    ________
    ________
    _______K
    """
    .Replace("\n", String.Empty);

    yield return new object[] { whiteInCheckmateBoardStr, Color.White, true };
    yield return new object[] { whiteInCheckBoardStr, Color.White, false };
  }
}
