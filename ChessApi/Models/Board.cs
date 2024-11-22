using System;
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
      var tileRow = playerColor == Color.White ? (63 - i) / 8 : i / 8;
      var tileCol = playerColor == Color.White ? i % 8 : (63 - i) % 8;
      int tileIndex = (tileRow * 8) + tileCol;

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
    Debug.Assert(tileString.Length == 2,
                 "Unknown tile string format. Example: 'a1'.");

    int tileRow = tileString[0] switch
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

    int tileCol = tileString[1] switch
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
    return _Tiles[tileIndex];
  }
}
