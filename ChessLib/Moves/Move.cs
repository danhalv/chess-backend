using System.Text.Json.Serialization;

namespace ChessLib;

public class Move
{
  [JsonInclude]
  public int Src { get; set; }
  [JsonInclude]
  public int Dst { get; set; }

  [JsonConstructor]
  public Move(int src, int dst)
  {
    Src = src;
    Dst = dst;
  }

  public Move(string src, string dst)
  {
    Src = Tile.StringToIndex(src);
    Dst = Tile.StringToIndex(dst);
  }

  public override bool Equals(object obj)
  {
    if (obj == null || GetType() != obj.GetType())
      return false;

    var other = (Move)obj;
    return (Src == other.Src) && (Dst == other.Dst);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Src, Dst);
  }
}
