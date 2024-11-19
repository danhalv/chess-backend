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
}
