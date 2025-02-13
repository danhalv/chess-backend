namespace ChessLib;

public class Queen : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }
  public char CharRepresentation { get; }

  public Queen(Color color)
  {
    Color = color;
    HasMoved = false;
    CharRepresentation = (color == Color.White) ? 'Q' : 'q';
  }

  List<Move> IPiece.GetMoves(Board board, int pieceTilePos)
  {
    // possible tile consists of all diagonal, horizontal, and vertical tiles
    var possibleTiles = new List<List<int>>()
    {
      Tile.HorizontalTiles(Direction.Left, pieceTilePos, this.Color),
      Tile.HorizontalTiles(Direction.Right, pieceTilePos, this.Color),
      Tile.VerticalTiles(Direction.Forward, pieceTilePos, this.Color),
      Tile.VerticalTiles(Direction.Backward, pieceTilePos, this.Color),
      Tile.DiagonalTiles(Direction.DiagonalRight, Direction.Forward, pieceTilePos, this.Color),
      Tile.DiagonalTiles(Direction.DiagonalLeft, Direction.Forward, pieceTilePos, this.Color),
      Tile.DiagonalTiles(Direction.DiagonalRight, Direction.Backward, pieceTilePos, this.Color),
      Tile.DiagonalTiles(Direction.DiagonalLeft, Direction.Backward, pieceTilePos, this.Color)
    };

    // add moves in a direction until a tile is occupied
    // include occupied tile if it's opponent's piece
    List<Move> legalMoves(List<int> tileIndices)
    {
      var moves = new List<Move>();

      foreach (int tileIndex in tileIndices)
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

    return possibleTiles.Aggregate(new List<Move>(),
                                   (current, next) =>
    {
      current.AddRange(legalMoves(next));
      return current;
    });
  }
}
