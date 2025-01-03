using System.Diagnostics;

namespace ChessApi.Models.Chess;

public class Board
{
  // Represents board. Index 0 starts at 'a1' tile.
  public Tile[] Tiles { get; private set; } = new Tile[64];
  public Color Turn { get; private set; } = Color.White;

  public Board()
  {
    Tiles[0] = new Tile(0, new Rook(Color.White));
    Tiles[1] = new Tile(1, new Knight(Color.White));
    Tiles[2] = new Tile(2, new Bishop(Color.White));
    Tiles[3] = new Tile(3, new Queen(Color.White));
    Tiles[4] = new Tile(4, new King(Color.White));
    Tiles[5] = new Tile(5, new Bishop(Color.White));
    Tiles[6] = new Tile(6, new Knight(Color.White));
    Tiles[7] = new Tile(7, new Rook(Color.White));

    // fill second row with white pawns
    for (int i = 8; i < 16; i++)
      Tiles[i] = new Tile(i, new Pawn(Color.White));

    // fill 7th row with black pawns
    for (int i = 48; i < 56; i++)
      Tiles[i] = new Tile(i, new Pawn(Color.Black));

    Tiles[56] = new Tile(56, new Rook(Color.Black));
    Tiles[57] = new Tile(57, new Knight(Color.Black));
    Tiles[58] = new Tile(58, new Bishop(Color.Black));
    Tiles[59] = new Tile(59, new Queen(Color.Black));
    Tiles[60] = new Tile(60, new King(Color.Black));
    Tiles[61] = new Tile(61, new Bishop(Color.Black));
    Tiles[62] = new Tile(62, new Knight(Color.Black));
    Tiles[63] = new Tile(63, new Rook(Color.Black));

    // fill remaining empty tiles
    for (int i = 0; i < 64; i++)
    {
      if (Tiles[i] == null)
        Tiles[i] = new Tile(i);
    }
  }

  public Board(Color playerTurn, string boardString)
  {
    Debug.Assert(boardString.Length == 64,
                 "Board string should be 64 characters.");

    Turn = playerTurn;

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
          Tiles[tileIndex] = new Tile(tileIndex, new King(Color.Black));
          break;
        case 'K':
          Tiles[tileIndex] = new Tile(tileIndex, new King(Color.White));
          break;
        case 'n':
          Tiles[tileIndex] = new Tile(tileIndex, new Knight(Color.Black));
          break;
        case 'N':
          Tiles[tileIndex] = new Tile(tileIndex, new Knight(Color.White));
          break;
        case 'p':
          Tiles[tileIndex] = new Tile(tileIndex, new Pawn(Color.Black));
          break;
        case 'P':
          Tiles[tileIndex] = new Tile(tileIndex, new Pawn(Color.White));
          break;
        case 'r':
          Tiles[tileIndex] = new Tile(tileIndex, new Rook(Color.Black));
          break;
        case 'R':
          Tiles[tileIndex] = new Tile(tileIndex, new Rook(Color.White));
          break;
        case 'q':
          Tiles[tileIndex] = new Tile(tileIndex, new Queen(Color.Black));
          break;
        case 'Q':
          Tiles[tileIndex] = new Tile(tileIndex, new Queen(Color.White));
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

  public bool IsCheck()
  {
    return IsCheck(Turn);
  }

  public bool IsCheck(Color playerColor)
  {
    var playerKingTile = Tiles.First(t => t.Piece != null
                                          && t.Piece.GetType() == typeof(King)
                                          && t.Piece.Color == playerColor);

    var opponentColor = (playerColor == Color.White) ? Color.Black : Color.White;

    return PlayerMoves(opponentColor).Any(move =>
    {
      return move.Dst == playerKingTile.Index;
    });
  }

  public bool IsCheckmate()
  {
    if (!IsCheck())
      return false;

    // true, if all possible moves result in check
    return PlayerMoves(Turn).All(move =>
    {
      var savedData = (SrcPiece: GetTile(move.Src).Piece,
                       DstPiece: GetTile(move.Dst).Piece,
                       PlayerTurn: Turn);

      MakeMove(move);

      bool isCheckAfterMove = IsCheck(savedData.PlayerTurn);

      // undo move
      Tiles[move.Src].Piece = savedData.SrcPiece;
      Tiles[move.Dst].Piece = savedData.DstPiece;
      Turn = savedData.PlayerTurn;

      return isCheckAfterMove;
    });
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

  private List<Move> PlayerMoves(Color playerColor)
  {
    var playerOccupiedTiles = Tiles.Where(t => t.Piece != null
                                               && t.Piece.Color == playerColor);

    return playerOccupiedTiles.Aggregate(new List<Move>(),
                                         (moves, tile) =>
                                         {
                                           moves.AddRange(tile.Piece!.GetMoves(this, tile.Index));
                                           return moves;
                                         });
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
