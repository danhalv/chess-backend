using System.Text.Json.Serialization;

namespace ChessApi.Models.Chess;

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
}
