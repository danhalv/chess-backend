
namespace ChessApi.Models;

public class Tile
{
  public int Index { get; }
  public Color Color { get; }
  public IPiece? Piece { get; set; }

  public Tile(int index, IPiece? piece = null)
  {
    Index = index;
    Piece = piece;

    var isEvenColumn = (index % 2) == 0;
    var isEvenRow = ((index / 8) % 2) == 0;

    if (isEvenRow)
      Color = isEvenColumn ? Color.Black : Color.White;
    else
      Color = isEvenColumn ? Color.White : Color.Black;
  }
}
