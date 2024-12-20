using ChessApi.Models.Chess;

namespace ChessApi.Models;

public class ChessMove : Move
{
  public long Id { get; set; }

  public ChessMove(int src, int dst)
    : base(src, dst)
  { }

  public ChessMove(string srcStr, string dstStr)
    : base(srcStr, dstStr)
  { }
}
