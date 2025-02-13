namespace ChessLib;

public class King : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }
  public char CharRepresentation { get; }

  public King(Color color)
  {
    Color = color;
    HasMoved = false;
    CharRepresentation = (color == Color.White) ? 'K' : 'k';
  }

  List<Move> IPiece.GetMoves(Board board, int pieceTilePos)
  {
    return RegularMoves(board, pieceTilePos)
           .Concat(CastlingMoves(board, pieceTilePos)).ToList();
  }

  private List<Move> RegularMoves(Board board, int pieceTilePos)
  {
    var surroundingTiles = new List<List<int>>
    {
      Tile.HorizontalTiles(Direction.Left, pieceTilePos, this.Color),
      Tile.HorizontalTiles(Direction.Right, pieceTilePos, this.Color),
      Tile.VerticalTiles(Direction.Forward, pieceTilePos, this.Color),
      Tile.VerticalTiles(Direction.Backward, pieceTilePos, this.Color),
      Tile.DiagonalTiles(Direction.DiagonalRight, Direction.Forward, pieceTilePos, this.Color),
      Tile.DiagonalTiles(Direction.DiagonalLeft, Direction.Forward, pieceTilePos, this.Color),
      Tile.DiagonalTiles(Direction.DiagonalRight, Direction.Backward, pieceTilePos, this.Color),
      Tile.DiagonalTiles(Direction.DiagonalLeft, Direction.Backward, pieceTilePos, this.Color)
    }.Aggregate(new List<int>(),
                (current, next) =>
    {
      if (next.Any())
        current.Add(next.First());

      return current;
    });

    List<Move> legalMoves(List<int> surroundingTileIndices)
    {
      var moves = new List<Move>();

      foreach (int tileIndex in surroundingTileIndices)
      {
        IPiece piece = board.GetTile(tileIndex).Piece as IPiece;

        if (piece == null || piece.Color != this.Color)
          moves.Add(new Move(pieceTilePos, tileIndex));
      }

      return moves;
    }

    return legalMoves(surroundingTiles);
  }

  private List<Move> CastlingMoves(Board board, int pieceTilePos)
  {
    if (this.HasMoved)
      return new List<Move>();

    var horizontalTiles = new List<List<int>>
    {
      Tile.HorizontalTiles(Direction.Left, pieceTilePos, this.Color),
      Tile.HorizontalTiles(Direction.Right, pieceTilePos, this.Color)
    };

    List<Move> legalCastlingMoves(List<int> horizontalTileIndices)
    {
      var moves = new List<Move>();

      foreach (int tileIndex in horizontalTileIndices)
      {
        Rook rook = board.GetTile(tileIndex).Piece as Rook;

        if (rook != null && !rook.HasMoved && rook.Color == this.Color)
        {
          if (tileIndex == Tile.StringToIndex("a1"))
            moves.Add(new CastlingMove(pieceTilePos, Tile.StringToIndex("a1")));
          else if (tileIndex == Tile.StringToIndex("h1"))
            moves.Add(new CastlingMove(pieceTilePos, Tile.StringToIndex("h1")));
          else if (tileIndex == Tile.StringToIndex("a8"))
            moves.Add(new CastlingMove(pieceTilePos, Tile.StringToIndex("a8")));
          else if (tileIndex == Tile.StringToIndex("h8"))
            moves.Add(new CastlingMove(pieceTilePos, Tile.StringToIndex("h8")));
        }
      }

      return moves;
    }

    return horizontalTiles.Aggregate(new List<Move>(),
                                     (current, next) =>
    {
      current.AddRange(legalCastlingMoves(next));
      return current;
    });
  }
}
