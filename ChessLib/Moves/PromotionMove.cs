namespace ChessLib;

public class PromotionMove : Move
{
  public PromotionMove(int src, int dst)
    : base(src, dst)
  { }

  public PromotionMove(string src, string dst)
    : base(src, dst)
  { }
}
