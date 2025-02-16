namespace ChessLib;

public class Pawn : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }
  public bool IsEnpassantable { get; set; }
  public char CharRepresentation { get; }

  public Pawn(Color color, bool hasMoved = false, bool isEnpassantable = false)
  {
    Color = color;
    HasMoved = hasMoved;
    IsEnpassantable = isEnpassantable;
    CharRepresentation = (color == Color.White) ? 'P' : 'p';
  }

  List<Move> IPiece.GetMoves(Board board, int pieceTilePos)
  {
    return ForwardMoves(board, pieceTilePos)
           .Concat(RegularCaptureMoves(board, pieceTilePos))
           .Concat(EnpassantCaptureMoves(board, pieceTilePos))
           .ToList();
  }

  private List<Move> ForwardMoves(Board board, int pieceTilePos)
  {
    var moves = new List<Move>();

    var forwardTiles = Tile.VerticalTiles(Direction.Forward, pieceTilePos, this.Color);

    // one step forward
    if (forwardTiles.Count >= 1)
    {
      var oneTileForward = forwardTiles[0];

      if (board.IsTileOccupied(oneTileForward))
        return moves;

      // possible promotion on last row
      if ((this.Color == Color.White && Tile.Row(oneTileForward) == 7)
          || (this.Color == Color.Black && Tile.Row(oneTileForward) == 0))
      {
        moves.Add(new PromotionMove(pieceTilePos, oneTileForward));
        return moves;
      }
      else
      {
        moves.Add(new Move(pieceTilePos, oneTileForward));
      }

      // two steps forward
      if (forwardTiles.Count >= 2)
      {
        var twoTilesForward = forwardTiles[1];

        if (!board.IsTileOccupied(twoTilesForward) && !this.HasMoved)
          moves.Add(new PawnDoubleMove(pieceTilePos, twoTilesForward));
      }
    }

    return moves;
  }

  private List<Move> RegularCaptureMoves(Board board, int pieceTilePos)
  {
    var forwardDiagonals = new List<List<int>>
    {
      Tile.DiagonalTiles(Direction.DiagonalRight,
                         Direction.Forward,
                         pieceTilePos,
                         this.Color),
      Tile.DiagonalTiles(Direction.DiagonalLeft,
                         Direction.Forward,
                         pieceTilePos,
                         this.Color)
    }.Aggregate(new List<int>(),
                (current, next) =>
    {
      if (next.Any())
        current.Add(next.First());

      return current;
    });

    return forwardDiagonals.Aggregate(new List<Move>(),
                                      (current, next) =>
    {
      IPiece piece = board.GetTile(next).Piece as IPiece;

      if (piece != null && piece.Color != this.Color)
      {
        // possible promotion on last row
        if ((this.Color == Color.White && Tile.Row(next) == 7)
            || (this.Color == Color.Black && Tile.Row(next) == 0))
        {
          current.Add(new PromotionMove(pieceTilePos, next));
        }
        else
        {
          current.Add(new Move(pieceTilePos, next));
        }
      }

      return current;
    });
  }

  private List<Move> EnpassantCaptureMoves(Board board, int pieceTilePos)
  {
    var horizontalTiles = new List<List<int>>
    {
      Tile.HorizontalTiles(Direction.Left, pieceTilePos, this.Color),
      Tile.HorizontalTiles(Direction.Right, pieceTilePos, this.Color)
    }.Aggregate(new List<int>(),
                (current, next) =>
    {
      if (next.Any())
        current.Add(next.First());

      return current;
    });

    return horizontalTiles.Aggregate(new List<Move>(),
                                     (current, next) =>
    {
      Pawn pawn = board.GetTile(next).Piece as Pawn;

      if (pawn != null && pawn.Color != this.Color && pawn.IsEnpassantable)
      {
        if (Color.White == this.Color)
          current.Add(new EnpassantCapture(pieceTilePos, next + 8));
        else
          current.Add(new EnpassantCapture(pieceTilePos, next - 8));
      }

      return current;
    });
  }
}
