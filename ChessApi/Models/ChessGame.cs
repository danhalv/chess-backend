using ChessApi.Models.Chess;

namespace ChessApi.Models;

public class ChessGame
{
  public long Id { get; set; }
  public Color Turn { get; set; }
  public List<ChessMove> ChessMoves { get; set; } = null!;
}
