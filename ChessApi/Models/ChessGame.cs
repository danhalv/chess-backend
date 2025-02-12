using ChessLib;

namespace ChessApi.Models;

public class ChessGame
{
  public long Id { get; set; }
  public Color Turn { get; set; }
  public List<Move> Moves { get; set; } = null!;
}
