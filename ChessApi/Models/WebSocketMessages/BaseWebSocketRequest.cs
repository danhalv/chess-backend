using System.Text.Json.Serialization;

namespace ChessApi.Models.WebSocketMessages;

public class BaseWebSocketRequest : IWebSocketRequest
{
  [JsonRequired]
  public WebSocketRequestType RequestType { get; set; }

  public BaseWebSocketRequest(WebSocketRequestType requestType)
  {
    RequestType = requestType;
  }
}
