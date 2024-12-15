namespace ChessApi.Models.Chess;

public class Pawn : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }
  public bool IsEnpassantable { get; set; }

  public Pawn(Color color, bool hasMoved = false, bool isEnpassantable = false)
  {
    Color = color;
    HasMoved = hasMoved;
    IsEnpassantable = isEnpassantable;
  }

  List<Move> IPiece.GetMoves(Board board, int pieceTilePos)
  {
    List<Move> diagonalPawnMoves(List<int> diagonalTiles)
    {
      var diagonalMoves = new List<Move>();

      if (diagonalTiles.Any())
      {
        if (board.IsTileOccupied(diagonalTiles[0]))
        {
          bool isEnemy = (this.Color !=
                          board.GetTile(diagonalTiles[0]).Piece!.Color);
          if (isEnemy)
            diagonalMoves.Add(new Move(pieceTilePos, diagonalTiles[0]));
        }
      }

      return diagonalMoves;
    }

    var moves = new List<Move>();

    // diagonal captures
    moves.AddRange(diagonalPawnMoves(Tile.DiagonalTiles(Direction.DiagonalRight,
                                                        Direction.Forward,
                                                        pieceTilePos,
                                                        this.Color)));
    moves.AddRange(diagonalPawnMoves(Tile.DiagonalTiles(Direction.DiagonalLeft,
                                                        Direction.Forward,
                                                        pieceTilePos,
                                                        this.Color)));

    var forwardTiles = Tile.VerticalTiles(Direction.Forward,
                                          pieceTilePos,
                                          this.Color);

    // forward moves
    for (int i = 0; i < 2; i++)
    {
      if (forwardTiles.Count < i || board.IsTileOccupied(forwardTiles[i]))
        break;

      if (i < 1 || !HasMoved)
        moves.Add(new Move(pieceTilePos, forwardTiles[i]));
    }

    // en passant capture moves
    var rightTiles = Tile.HorizontalTiles(Direction.Right,
                                          pieceTilePos,
                                          this.Color);

    if (rightTiles.Any())
    {
      var piece = board.GetTile(rightTiles[0]).Piece;

      if (piece != null
          && piece.GetType() == typeof(Pawn)
          && ((Pawn)piece).IsEnpassantable)
      {
        if (this.Color == Color.White)
        {
          var dstTile = Tile.CalcIndex(pieceTilePos, 1, 1);
          if (Tile.IsInRange(dstTile))
            moves.Add(new Move(pieceTilePos, dstTile));
        }
        else
        {
          var dstTile = Tile.CalcIndex(pieceTilePos, -1, -1);
          if (Tile.IsInRange(dstTile))
            moves.Add(new Move(pieceTilePos, dstTile));
        }
      }
    }

    var leftTiles = Tile.HorizontalTiles(Direction.Left,
                                         pieceTilePos,
                                         this.Color);

    if (leftTiles.Any())
    {
      var piece = board.GetTile(leftTiles[0]).Piece;

      if (piece != null
          && piece.GetType() == typeof(Pawn)
          && ((Pawn)piece).IsEnpassantable)
      {
        if (this.Color == Color.White)
        {
          var dstTile = Tile.CalcIndex(pieceTilePos, 1, -1);
          if (Tile.IsInRange(dstTile))
            moves.Add(new Move(pieceTilePos, dstTile));
        }
        else
        {
          var dstTile = Tile.CalcIndex(pieceTilePos, -1, 1);
          if (Tile.IsInRange(dstTile))
            moves.Add(new Move(pieceTilePos, dstTile));
        }
      }
    }

    return moves;
  }
}
