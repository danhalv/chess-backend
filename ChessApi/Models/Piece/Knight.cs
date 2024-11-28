namespace ChessApi.Models;

public class Knight : IPiece
{
  public Color Color { get; }
  public bool HasMoved { get; set; }

  public Knight(Color color)
  {
    Color = color;
    HasMoved = false;
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

    var moves = new List<Move>();

    foreach (int tileIndex in possibleTiles)
    {
      if (!Tile.IsInRange(tileIndex))
        continue;

      IPiece? tilePiece = board.GetTile(tileIndex).Piece;

      if (tilePiece == null
          || this.Color != tilePiece.Color)
      {
        moves.Add(new Move(pieceTilePos, tileIndex));
      }
    }

    return moves;
  }
}
