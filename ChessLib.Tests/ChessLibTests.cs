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

  [Theory, MemberData(nameof(TestPossiblePawnMovesData))]
  public void TestPossiblePawnMoves(string boardString,
                                    string pawnTileStr,
                                    List<Move> expected)
  {
    var board = new Board(Color.White, boardString);
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

    yield return new object[] { boardString, "b2", b2WhitePawnMoves };
    yield return new object[] { boardString, "g7", g7BlackPawnMoves };
    yield return new object[] { boardString, "f4", f4BlackPawnMoves };
    yield return new object[] { boardString, "b5", b5WhitePawnMoves };
  }

  [Theory, MemberData(nameof(TestPossibleRookMovesData))]
  public void TestPossibleRookMoves(string boardString,
                                    string rookTileString,
                                    List<Move> expected)
  {
    var board = new Board(Color.White, boardString);

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

    yield return new object[] { boardString, "c4", c4BlackRookMoves };
    yield return new object[] { boardString, "f4", f4WhiteRookMoves };
  }

  [Theory, MemberData(nameof(TestPossibleQueenMovesData))]
  public void TestPossibleQueenMoves(string boardString,
                                    string queenTileString,
                                    List<Move> expected)
  {
    var board = new Board(Color.White, boardString);

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

    yield return new object[] { boardString, "b2", b2WhiteQueenMoves };
    yield return new object[] { boardString, "e2", e2BlackQueenMoves };
  }

  [Theory, MemberData(nameof(TestPossibleKingMovesData))]
  public void TestPossibleKingMoves(string boardString,
                                    string kingTileString,
                                    List<Move> expected)
  {
    var board = new Board(Color.White, boardString);

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
      new Move("e8", "a8"), // castling move
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
      new Move("e1", "h1"), // castling move
      new Move("e1", "f1"),
      new Move("e1", "f2"),
      new Move("e1", "e2"),
      new Move("e1", "d2"),
      new Move("e1", "d1")
    };

    yield return new object[] { boardString, "e8", e8BlackKingMoves };
    yield return new object[] { boardString, "c3", c3WhiteKingMoves };
    yield return new object[] { boardString, "e1", e1WhiteKingMoves };
  }

  [Theory, MemberData(nameof(TestCastlingLegalMovesData))]
  public void TestCastlingLegalMoves(string boardString,
                                     Color playerTurn,
                                     string kingTile,
                                     Move castlingMove)
  {
    var board = new Board(playerTurn, boardString);

    var legalMoves = board.LegalMoves(kingTile);

    Assert.Contains(castlingMove, legalMoves);
  }

  public static IEnumerable<object[]> TestCastlingLegalMovesData()
  {
    var boardString = """
    r___k__r
    ________
    ________
    ________
    ________
    ________
    ________
    R___K__R
    """
    .Replace("\n", String.Empty);

    // white kingside castling
    yield return new object[] { boardString, Color.White, "e1", new CastlingMove("e1", "h1") };

    // white queenside castling
    yield return new object[] { boardString, Color.White, "e1", new CastlingMove("e1", "a1") };

    // black kingside castling
    yield return new object[] { boardString, Color.Black, "e8", new CastlingMove("e8", "h8") };

    // black queenside castling
    yield return new object[] { boardString, Color.Black, "e8", new CastlingMove("e8", "a8") };
  }

  [Theory, MemberData(nameof(TestCastlingMovesData))]
  public void TestCastlingMoves(string boardString,
                                string oldKingTile,
                                string newKingTile,
                                string oldRookTile,
                                string newRookTile,
                                Move castlingMove)
  {
    var board = new Board(Color.White, boardString);

    var kingBeforeMove = board.GetTile(oldKingTile).Piece;
    var rookBeforeMove = board.GetTile(oldRookTile).Piece;

    board.MakeMove(castlingMove);

    var kingAfterMove = board.GetTile(newKingTile).Piece;
    var rookAfterMove = board.GetTile(newRookTile).Piece;
    Assert.True(Object.ReferenceEquals(kingBeforeMove, kingAfterMove));
    Assert.True(Object.ReferenceEquals(rookBeforeMove, rookAfterMove));
  }

  public static IEnumerable<object[]> TestCastlingMovesData()
  {
    var boardString = """
    r___k__r
    ________
    ________
    ________
    ________
    ________
    ________
    R___K__R
    """
    .Replace("\n", String.Empty);

    // white kingside castling
    yield return new object[] { boardString, "e1", "g1", "h1", "f1", new CastlingMove("e1", "h1") };

    // white queenside castling
    yield return new object[] { boardString, "e1", "c1", "a1", "d1", new CastlingMove("e1", "a1") };

    // black kingside castling
    yield return new object[] { boardString, "e8", "g8", "h8", "f8", new CastlingMove("e8", "h8") };

    // black queenside castling
    yield return new object[] { boardString, "e8", "c8", "a8", "d8", new CastlingMove("e8", "a8") };
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

    bool actual = board.IsCheckmate();

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

  [Theory, MemberData(nameof(TestPromotionMovesData))]
  public void TestPromotionMoves(string boardString,
                                      Color playerTurn,
                                      string pawnTile,
                                      Move promotionMove)
  {
    var board = new Board(playerTurn, boardString);

    var pawn = board.GetTile(pawnTile).Piece;
    var pawnMoves = pawn.GetMoves(board, Tile.StringToIndex(pawnTile));

    Assert.Contains(promotionMove, pawnMoves);

    board.MakeMove(promotionMove);

    Assert.IsType<Queen>(board.GetTile(promotionMove.Dst).Piece);
  }

  public static IEnumerable<object[]> TestPromotionMovesData()
  {
    var boardString = """
    ________
    P_______
    ________
    P_______
    ________
    ________
    p_______
    ________
    """
    .Replace("\n", String.Empty);

    // white promotion
    yield return new object[] { boardString, Color.White, "a7", new PromotionMove("a7", "a8") };

    // black promotion
    yield return new object[] { boardString, Color.Black, "a2", new PromotionMove("a2", "a1") };
  }
}
