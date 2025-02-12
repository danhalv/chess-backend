namespace ChessLib;

public class CastlingMove : Move
{
  public CastlingMove(int src, int dst)
    : base(src, dst)
  { }

  public CastlingMove(string src, string dst)
    : base(src, dst)
  { }
}
