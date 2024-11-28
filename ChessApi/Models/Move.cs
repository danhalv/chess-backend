namespace ChessApi.Models;

public class Move
{
  public int From { get; }
  public int To { get; }

  public Move(int fromIndex, int toIndex)
  {
    From = fromIndex;
    To = toIndex;
  }

  public Move(string fromString, string toString)
  {
    From = Tile.StringToIndex(fromString);
    To = Tile.StringToIndex(toString);
  }
}
