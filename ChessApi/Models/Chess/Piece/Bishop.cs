namespace ChessApi.Models.Chess;

public class Bishop : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }
  public char CharRepresentation { get; }

  public Bishop(Color color)
  {
    Color = color;
    HasMoved = false;
    CharRepresentation = (color == Color.White) ? 'P' : 'p';
  }

  List<Move> IPiece.GetMoves(Board board, int pieceTilePos)
  {
    var moves = new List<Move>();

    void addDiagonalTilesUntilOccupied(List<int> diagonalTiles)
    {
      foreach (int tileIndex in diagonalTiles)
      {
        if (!board.IsTileOccupied(tileIndex))
        {
          moves.Add(new Move(pieceTilePos, tileIndex));
        }
        else
        {
          IPiece tilePiece = board.GetTile(tileIndex).Piece!;

          if (tilePiece.Color != this.Color)
            moves.Add(new Move(pieceTilePos, tileIndex));

          break;
        }
      }
    }

    addDiagonalTilesUntilOccupied(Tile.DiagonalTiles(Direction.DiagonalRight,
                                                     Direction.Forward,
                                                     pieceTilePos,
                                                     this.Color));
    addDiagonalTilesUntilOccupied(Tile.DiagonalTiles(Direction.DiagonalRight,
                                                     Direction.Backward,
                                                     pieceTilePos,
                                                     this.Color));
    addDiagonalTilesUntilOccupied(Tile.DiagonalTiles(Direction.DiagonalLeft,
                                                     Direction.Forward,
                                                     pieceTilePos,
                                                     this.Color));
    addDiagonalTilesUntilOccupied(Tile.DiagonalTiles(Direction.DiagonalLeft,
                                                     Direction.Backward,
                                                     pieceTilePos,
                                                     this.Color));

    return moves;
  }
}
