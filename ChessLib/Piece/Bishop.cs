namespace ChessLib;

public class Bishop : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }
  public char CharRepresentation { get; }

  public Bishop(Color color)
  {
    Color = color;
    HasMoved = false;
    CharRepresentation = (color == Color.White) ? 'B' : 'b';
  }

  List<Move> IPiece.GetMoves(Board board, int pieceTilePos)
  {
    var diagonals = new List<List<int>>
    {
      Tile.DiagonalTiles(Direction.DiagonalRight,
                         Direction.Forward,
                         pieceTilePos,
                         this.Color),
      Tile.DiagonalTiles(Direction.DiagonalRight,
                         Direction.Backward,
                         pieceTilePos,
                         this.Color),
      Tile.DiagonalTiles(Direction.DiagonalLeft,
                         Direction.Forward,
                         pieceTilePos,
                         this.Color),
      Tile.DiagonalTiles(Direction.DiagonalLeft,
                         Direction.Backward,
                         pieceTilePos,
                         this.Color)
    };

    // add moves in diagonal until a tile is occupied
    // include occupied tile if it's opponent's piece
    List<Move> legalMoves(List<int> diagonal)
    {
      var moves = new List<Move>();

      foreach (int tileIndex in diagonal)
      {
        IPiece piece = board.GetTile(tileIndex).Piece as IPiece;

        if (piece == null)
        {
          moves.Add(new Move(pieceTilePos, tileIndex));
        }
        else
        {
          if (piece.Color != this.Color)
            moves.Add(new Move(pieceTilePos, tileIndex));

          break;
        }
      }

      return moves;
    }

    return diagonals.Aggregate(new List<Move>(),
                               (current, next) =>
    {
      current.AddRange(legalMoves(next));
      return current;
    });
  }
}
