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
    var moves = new List<Move>();

    void addTilesUntilOpponentOccupied(List<int> tiles)
    {
      foreach (int tileIndex in tiles)
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

    addTilesUntilOpponentOccupied(
        Tile.VerticalTiles(Direction.Forward, pieceTilePos, this.Color));
    addTilesUntilOpponentOccupied(
        Tile.VerticalTiles(Direction.Backward, pieceTilePos, this.Color));
    addTilesUntilOpponentOccupied(
        Tile.HorizontalTiles(Direction.Left, pieceTilePos, this.Color));
    addTilesUntilOpponentOccupied(
        Tile.HorizontalTiles(Direction.Right, pieceTilePos, this.Color));

    return moves;
  }
}
