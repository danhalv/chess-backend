namespace ChessLib;

public class Knight : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }
  public char CharRepresentation { get; }

  public Knight(Color color)
  {
    Color = color;
    HasMoved = false;
    CharRepresentation = (color == Color.White) ? 'N' : 'n';
  }

  List<Move> IPiece.GetMoves(Board board, int pieceTilePos)
  {
    var possibleTiles = new List<int>()
    {
      Tile.CalcIndex(pieceTilePos, 1, 2),
      Tile.CalcIndex(pieceTilePos, 1, -2),
      Tile.CalcIndex(pieceTilePos, 2, 1),
      Tile.CalcIndex(pieceTilePos, 2, -1),
      Tile.CalcIndex(pieceTilePos, -1, 2),
      Tile.CalcIndex(pieceTilePos, -1, -2),
      Tile.CalcIndex(pieceTilePos, -2, 1),
      Tile.CalcIndex(pieceTilePos, -2, -1)
    };

    return possibleTiles.Aggregate(new List<Move>(),
                                   (current, next) =>
    {
      if (Tile.IsInRange(next))
      {
        var piece = board.GetTile(next).Piece;

        if (piece == null || piece.Color != this.Color)
          current.Add(new Move(pieceTilePos, next));
      }

      return current;
    });
  }
}
