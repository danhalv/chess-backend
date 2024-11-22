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
    yield return new object[] { boardString, Color.White, "c1", new Pawn(Color.White) };
    yield return new object[] { boardString, Color.Black, "a1", new Pawn(Color.White) };
    yield return new object[] { boardString, Color.Black, "h8", new Pawn(Color.Black) };
    yield return new object[] { boardString, Color.Black, "e5", null! };
  }
}
