namespace ChessLib;

public class PawnDoubleMove : Move
{
  public PawnDoubleMove(int src, int dst)
    : base(src, dst)
  { }

  public PawnDoubleMove(string src, string dst)
    : base(src, dst)
  { }
}
