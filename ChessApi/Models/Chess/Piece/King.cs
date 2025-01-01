namespace ChessApi.Models.Chess;

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
    var moves = new List<Move>();

    void addTilesUntilOpponentOccupied(List<int> tiles)
    {
      bool isKingDefaultPosition = (this.Color == Color.White)
                                   ? (pieceTilePos == Tile.StringToIndex("e1"))
                                   : (pieceTilePos == Tile.StringToIndex("e8"));
      bool hasKingMoved = board.GetTile(pieceTilePos).Piece!.HasMoved;

      if (tiles.Count > 0)
      {
        if (!board.IsTileOccupied(tiles[0]))
        {
          moves.Add(new Move(pieceTilePos, tiles[0]));
        }
        else
        {
          IPiece tilePiece = board.GetTile(tiles[0]).Piece!;

          if (tilePiece.Color != this.Color)
          {
            moves.Add(new Move(pieceTilePos, tiles[0]));
          }
        }
      }

      var rookDefaultTiles = new List<int>
      {
        Tile.StringToIndex("a1"),
        Tile.StringToIndex("h1"),
        Tile.StringToIndex("a8"),
        Tile.StringToIndex("h8")
      };

      foreach (var tile in tiles.Where(i => rookDefaultTiles.Contains(i)))
      {
        var tilePiece = board.GetTile(tile).Piece;

        // Castling moves
        if (tilePiece != null
            && tilePiece.GetType() == typeof(Rook)
            && tilePiece.Color == this.Color
            && !tilePiece.HasMoved
            && isKingDefaultPosition
            && !hasKingMoved)
        {
          if (this.Color == Color.White)
          {
            if (tile == Tile.StringToIndex("a1"))
              moves.Add(new Move(pieceTilePos, Tile.StringToIndex("c1")));
            else if (tile == Tile.StringToIndex("h1"))
              moves.Add(new Move(pieceTilePos, Tile.StringToIndex("g1")));
          }
          else
          {
            if (tile == Tile.StringToIndex("a8"))
              moves.Add(new Move(pieceTilePos, Tile.StringToIndex("c8")));
            else if (tile == Tile.StringToIndex("h8"))
              moves.Add(new Move(pieceTilePos, Tile.StringToIndex("g8")));
          }
        }
      }
    }

    addTilesUntilOpponentOccupied(
        Tile.VerticalTiles(Direction.Forward, pieceTilePos, this.Color));
    addTilesUntilOpponentOccupied(
        Tile.VerticalTiles(Direction.Backward, pieceTilePos, this.Color));
    addTilesUntilOpponentOccupied(
        Tile.HorizontalTiles(Direction.Left, pieceTilePos, this.Color));
    addTilesUntilOpponentOccupied(
        Tile.HorizontalTiles(Direction.Right, pieceTilePos, this.Color));
    addTilesUntilOpponentOccupied(
        Tile.DiagonalTiles(Direction.DiagonalRight,
                           Direction.Forward,
                           pieceTilePos,
                           this.Color));
    addTilesUntilOpponentOccupied(
        Tile.DiagonalTiles(Direction.DiagonalLeft,
                           Direction.Forward,
                           pieceTilePos,
                           this.Color));
    addTilesUntilOpponentOccupied(
        Tile.DiagonalTiles(Direction.DiagonalRight,
                           Direction.Backward,
                           pieceTilePos,
                           this.Color));
    addTilesUntilOpponentOccupied(
        Tile.DiagonalTiles(Direction.DiagonalLeft,
                           Direction.Backward,
                           pieceTilePos,
                           this.Color));

    return moves;
  }
}
