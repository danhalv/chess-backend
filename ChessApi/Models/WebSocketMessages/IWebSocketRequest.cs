using System.Text.Json.Serialization;

namespace ChessApi.Models.WebSocketMessages;

[JsonPolymorphic(
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(BaseWebSocketRequest))]
public interface IWebSocketRequest
{
  WebSocketRequestType RequestType { get; set; }
}
