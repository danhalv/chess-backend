using System.Text.Json.Serialization;

namespace ChessApi.Models.WebSocketMessages;

public class GetMovesRequest : BaseWebSocketRequest
{
  [JsonRequired]
  public string Tile { get; set; }

  public GetMovesRequest(WebSocketRequestType requestType, string tile)
    : base(requestType)
  {
    Tile = tile;
  }
}
