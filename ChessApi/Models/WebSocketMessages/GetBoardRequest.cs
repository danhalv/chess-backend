using System.Text.Json.Serialization;

namespace ChessApi.Models.WebSocketMessages;

public class GetBoardRequest : BaseWebSocketRequest
{
  public GetBoardRequest(WebSocketRequestType requestType)
    : base(requestType)
  { }
}
