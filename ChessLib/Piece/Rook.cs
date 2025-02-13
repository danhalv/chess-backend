namespace ChessLib;

public class Rook : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }
  public char CharRepresentation { get; }

  public Rook(Color color)
  {
    Color = color;
    HasMoved = false;
    CharRepresentation = (color == Color.White) ? 'R' : 'r';
  }

  List<Move> IPiece.GetMoves(Board board, int pieceTilePos)
  {
    var horizontalsAndVerticals = new List<List<int>>
    {
      Tile.HorizontalTiles(Direction.Left, pieceTilePos, this.Color),
      Tile.HorizontalTiles(Direction.Right, pieceTilePos, this.Color),
      Tile.VerticalTiles(Direction.Forward, pieceTilePos, this.Color),
      Tile.VerticalTiles(Direction.Backward, pieceTilePos, this.Color)
    };

    // add vertical or horizontal moves until a tile is occupied
    // include occupied tile if it's opponent's piece
    List<Move> legalMoves(List<int> horizontalOrVertical)
    {
      var moves = new List<Move>();

      foreach (int tileIndex in horizontalOrVertical)
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

    return horizontalsAndVerticals.Aggregate(new List<Move>(),
                                             (current, next) =>
    {
      current.AddRange(legalMoves(next));
      return current;
    });
  }
}
