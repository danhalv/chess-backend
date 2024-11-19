
namespace ChessApi.Models;

public class Pawn : IPiece
{
  public Color Color { get; }

  public Pawn(Color color)
  {
    Color = color;
  }
}
