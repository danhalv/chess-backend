using System.Diagnostics;

namespace ChessApi.Models;

public class Board
{
  // Represents board. Index 0 starts at 'a1' tile.
  private Tile[] _Tiles;

  public Board()
  {
    _Tiles = new Tile[64];

    // fill second row with white pawns
    for (int i = 8; i < 16; i++)
      _Tiles[i] = new Tile(i, new Pawn(Color.White));

    // fill 7th row with black pawns
    for (int i = 48; i < 56; i++)
      _Tiles[i] = new Tile(i, new Pawn(Color.Black));

    // fill remaining empty tiles
    for (int i = 0; i < 64; i++)
    {
      if (_Tiles[i] == null)
        _Tiles[i] = new Tile(i);
    }
  }

  public Board(Color playerColor, string boardString)
  {
    Debug.Assert(boardString.Length == 64,
                 "Board string should be 64 characters.");

    _Tiles = new Tile[64];

    for (int i = 0; i < 64; i++)
    {
      int tileIndex = (Tile.Row(63 - i) * 8) + Tile.Col(i);

      switch (boardString[i])
      {
        case '_':
          _Tiles[tileIndex] = new Tile(tileIndex);
          break;
        case 'p':
          _Tiles[tileIndex] = new Tile(tileIndex, new Pawn(Color.Black));
          break;
        case 'P':
          _Tiles[tileIndex] = new Tile(tileIndex, new Pawn(Color.White));
          break;
        default:
          Debug.Assert(false, "Unsupported character.");
          break;
      }
    }
  }

  public Tile GetTile(int tileIndex)
  {
    return _Tiles[tileIndex];
  }

  public Tile GetTile(string tileString)
  {
    return _Tiles[Tile.StringToIndex(tileString)];
  }

  public bool IsTileOccupied(int tileIndex)
  {
    var tile = GetTile(tileIndex);

    if (tile.Piece != null)
      return true;
    return false;
  }
}
