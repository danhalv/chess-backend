namespace ChessApi.Models.Chess;

public class Move
{
  public int Src { get; set; }
  public int Dst { get; set; }

  public Move(int src, int dst)
  {
    Src = src;
    Dst = dst;
  }

  public Move(string srcStr, string dstStr)
  {
    Src = Tile.StringToIndex(srcStr);
    Dst = Tile.StringToIndex(dstStr);
  }
}
