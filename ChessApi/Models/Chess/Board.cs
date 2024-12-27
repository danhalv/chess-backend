using System.Diagnostics;

namespace ChessApi.Models.Chess;

public class Board
{
  // Represents board. Index 0 starts at 'a1' tile.
  public Tile[] Tiles { get; private set; } = new Tile[64];
  public Color Turn { get; private set; } = Color.White;

  public Board()
  {
    // fill second row with white pawns
    for (int i = 8; i < 16; i++)
      Tiles[i] = new Tile(i, new Pawn(Color.White));

    // fill 7th row with black pawns
    for (int i = 48; i < 56; i++)
      Tiles[i] = new Tile(i, new Pawn(Color.Black));

    // fill remaining empty tiles
    for (int i = 0; i < 64; i++)
    {
      if (Tiles[i] == null)
        Tiles[i] = new Tile(i);
    }
  }

  public Board(Color playerColor, string boardString)
  {
    Debug.Assert(boardString.Length == 64,
                 "Board string should be 64 characters.");

    for (int i = 0; i < 64; i++)
    {
      int tileIndex = (Tile.Row(63 - i) * 8) + Tile.Col(i);

      switch (boardString[i])
      {
        case '_':
          Tiles[tileIndex] = new Tile(tileIndex);
          break;
        case 'b':
          Tiles[tileIndex] = new Tile(tileIndex, new Bishop(Color.Black));
          break;
        case 'B':
          Tiles[tileIndex] = new Tile(tileIndex, new Bishop(Color.White));
          break;
        case 'k':
          Tiles[tileIndex] = new Tile(tileIndex, new Knight(Color.Black));
          break;
        case 'K':
          Tiles[tileIndex] = new Tile(tileIndex, new Knight(Color.White));
          break;
        case 'p':
          Tiles[tileIndex] = new Tile(tileIndex, new Pawn(Color.Black));
          break;
        case 'P':
          Tiles[tileIndex] = new Tile(tileIndex, new Pawn(Color.White));
          break;
        default:
          Debug.Assert(false, "Unsupported character.");
          break;
      }
    }
  }

  public Tile GetTile(int tileIndex)
  {
    return Tiles[tileIndex];
  }

  public Tile GetTile(string tileString)
  {
    return Tiles[Tile.StringToIndex(tileString)];
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
    Tiles[move.Dst].Piece = Tiles[move.Src].Piece;
    Tiles[move.Src].Piece = null;

    Turn = (Turn == Color.White) ? Color.Black : Color.White;
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

      if (IsTileOccupied(tileIndex))
        boardString += Tiles[tileIndex].Piece!.CharRepresentation;
      else
        boardString += "_";
    }

    return boardString;
  }
}
