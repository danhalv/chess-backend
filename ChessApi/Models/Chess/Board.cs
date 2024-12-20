using System.Diagnostics;

namespace ChessApi.Models.Chess;

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
        case 'b':
          _Tiles[tileIndex] = new Tile(tileIndex, new Bishop(Color.Black));
          break;
        case 'B':
          _Tiles[tileIndex] = new Tile(tileIndex, new Bishop(Color.White));
          break;
        case 'k':
          _Tiles[tileIndex] = new Tile(tileIndex, new Knight(Color.Black));
          break;
        case 'K':
          _Tiles[tileIndex] = new Tile(tileIndex, new Knight(Color.White));
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

  public void MakeMove(Move move)
  {
    _Tiles[move.Dst].Piece = _Tiles[move.Src].Piece;
    _Tiles[move.Src].Piece = null;
  }

  public string ToString(Color playerColor = Color.White)
  {
    var boardString = "";

    for (int i = 0; i < 64; i++)
    {
      int tileIndex = (playerColor == Color.White) ?
        (Tile.Row(63 - i) * 8) + Tile.Col(i) :
        i;

      // newline on rows
      if (i > 7 && i % 8 == 0)
        boardString += "\n";

      switch (_Tiles[tileIndex].Piece)
      {
        case Bishop b:
          boardString += (b.Color == Color.White) ? "B" : "b";
          break;
        case Knight k:
          boardString += (k.Color == Color.White) ? "K" : "k";
          break;
        case Pawn p:
          boardString += (p.Color == Color.White) ? "P" : "p";
          break;
        default:
          boardString += "_";
          break;
      }
    }

    return boardString;
  }
}
