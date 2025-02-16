namespace ChessLib;

public class EnpassantCapture : Move
{
  public EnpassantCapture(int src, int dst)
    : base(src, dst)
  { }

  public EnpassantCapture(string src, string dst)
    : base(src, dst)
  { }
}
