using System.Diagnostics;

namespace ChessApi.Models;

public class Tile
{
  public int Index { get; }
  public Color Color { get; }
  public IPiece? Piece { get; set; }

  public Tile(int index, IPiece? piece = null)
  {
    Index = index;
    Piece = piece;

    var isEvenColumn = (index % 2) == 0;
    var isEvenRow = ((index / 8) % 2) == 0;

    if (isEvenRow)
      Color = isEvenColumn ? Color.Black : Color.White;
    else
      Color = isEvenColumn ? Color.White : Color.Black;
  }

  public static int Row(int tileIndex)
  {
    return tileIndex / 8;
  }

  public static int Col(int tileIndex)
  {
    return tileIndex % 8;
  }

  public static bool IsInRange(int tileIndex)
  {
    if (tileIndex >= 0 && tileIndex < 64)
      return true;
    return false;
  }

  public static int CalcIndex(int tileIndex, int rowDiff, int colDiff)
  {
    int newTileRow = Tile.Row(tileIndex) + rowDiff;
    int newTileCol = Tile.Col(tileIndex) + colDiff;

    if (newTileRow < 0 || newTileRow > 7)
      return -1;

    if (newTileCol < 0 || newTileCol > 7)
      return -1;

    return tileIndex + (rowDiff * 8) + colDiff;
  }

  public static List<int> DiagonalTiles(Direction diagonal,
                                        Direction forwardOrBackward,
                                        int tileIndex,
                                        Color playerColor)
  {
    List<int> accumulateDiagonals(int tileIndex, int rowDiff, int colDiff)
    {
      var diagonalTiles = new List<int>();

      for (int diagonalTile = Tile.CalcIndex(tileIndex, rowDiff, colDiff);
           Tile.IsInRange(diagonalTile);
           diagonalTile += Tile.CalcIndex(diagonalTile, rowDiff, colDiff))
      {
        diagonalTiles.Add(diagonalTile);
      }

      return diagonalTiles;
    }

    if ((playerColor == Color.White && forwardOrBackward == Direction.Forward)
        || (playerColor == Color.Black && forwardOrBackward == Direction.Backward))
    {
      if (diagonal == Direction.DiagonalRight)
        return accumulateDiagonals(tileIndex, 1, 1);
      else
        return accumulateDiagonals(tileIndex, 1, -1);
    }

    if ((playerColor == Color.White && forwardOrBackward == Direction.Backward)
        || (playerColor == Color.Black && forwardOrBackward == Direction.Forward))
    {
      if (diagonal == Direction.DiagonalRight)
        return accumulateDiagonals(tileIndex, -1, 1);
      else
        return accumulateDiagonals(tileIndex, -1, -1);
    }

    return new List<int>();
  }

  public static List<int> HorizontalTiles(Direction direction,
                                          int tileIndex,
                                          Color playerColor)
  {
    var horizontalTiles = new List<int>();

    bool isSameTileRow(int tileIndex1, int tileIndex2)
    {
      return (tileIndex1 / 8) == (tileIndex2 / 8);
    }

    if ((playerColor == Color.White && direction == Direction.Right)
        || (playerColor == Color.Black && direction == Direction.Left))
    {
      for (int sideTile = Tile.CalcIndex(tileIndex, 0, 1);
           Tile.IsInRange(sideTile) && isSameTileRow(tileIndex, sideTile);
           sideTile = Tile.CalcIndex(sideTile, 0, 1))
        horizontalTiles.Add(sideTile);
    }

    if ((playerColor == Color.White && direction == Direction.Left)
        || (playerColor == Color.Black && direction == Direction.Right))
    {
      for (int sideTile = Tile.CalcIndex(tileIndex, 0, -1);
           Tile.IsInRange(sideTile) && isSameTileRow(tileIndex, sideTile);
           sideTile = Tile.CalcIndex(sideTile, 0, -1))
        horizontalTiles.Add(sideTile);
    }

    return horizontalTiles;
  }

  public static List<int> VerticalTiles(Direction direction,
                                        int tileIndex,
                                        Color playerColor)
  {
    var verticalTiles = new List<int>();

    if ((playerColor == Color.White && direction == Direction.Forward)
        || (playerColor == Color.Black && direction == Direction.Backward))
    {
      for (int verticalTile = Tile.CalcIndex(tileIndex, 1, 0);
           Tile.IsInRange(verticalTile);
           verticalTile = Tile.CalcIndex(verticalTile, 1, 0))
        verticalTiles.Add(verticalTile);
    }

    if ((playerColor == Color.White && direction == Direction.Backward)
        || (playerColor == Color.Black && direction == Direction.Forward))
    {
      for (int verticalTile = Tile.CalcIndex(tileIndex, -1, 0);
           Tile.IsInRange(verticalTile);
           verticalTile = Tile.CalcIndex(verticalTile, -1, 0))
        verticalTiles.Add(verticalTile);
    }

    return verticalTiles;
  }

  public static int StringToIndex(string tileString)
  {
    Debug.Assert(tileString.Length == 2,
                 "Unknown tile string format. Example: 'a1'.");

    int tileCol = tileString[0] switch
    {
      'a' => 0,
      'b' => 1,
      'c' => 2,
      'd' => 3,
      'e' => 4,
      'f' => 5,
      'g' => 6,
      'h' => 7,
      _ => throw new ArgumentOutOfRangeException(Char.ToString(tileString[0])),
    };

    int tileRow = tileString[1] switch
    {
      '1' => 0,
      '2' => 1,
      '3' => 2,
      '4' => 3,
      '5' => 4,
      '6' => 5,
      '7' => 6,
      '8' => 7,
      _ => throw new ArgumentOutOfRangeException(Char.ToString(tileString[1])),
    };

    int tileIndex = (tileRow * 8) + tileCol;
    return tileIndex;
  }
}
