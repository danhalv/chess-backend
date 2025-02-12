using System.Text.Json.Serialization;
using ChessLib;

namespace ChessApi.Models.WebSocketMessages;

public class MakeMoveRequest : BaseWebSocketRequest
{
  [JsonRequired]
  public Move Move { get; set; }

  public MakeMoveRequest(WebSocketRequestType requestType, Move move)
    : base(requestType)
  {
    Move = move;
  }
}
