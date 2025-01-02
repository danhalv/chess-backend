using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using ChessApi;
using ChessApi.Models.Chess;
using Microsoft.AspNetCore.TestHost;

namespace ChessApi.Tests;

public class ChessApiEndpointsTests
{
  [Fact]
  public async void TestWebSocketMove()
  {
    var boardString = """
    rnbqkbnr
    pppppppp
    ________
    ________
    _______P
    P_______
    _PPPPPP_
    RNBQKBNR
    """
    .Replace("\n", String.Empty);

    var expected = JsonSerializer.Serialize(new Board(Color.White, boardString));

    var buffer = new byte[1024 * 8];

    var server = new TestServer(ChessApi.Program.CreateWebHostBuilder(new string[0]));
    var httpClient = server.CreateClient();
    var response = await httpClient.PostAsync("/chessgames", new ByteArrayContent(buffer));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var wsClient = server.CreateWebSocketClient();
    var ws = await wsClient.ConnectAsync(new Uri("/ws/chessgames/1"), CancellationToken.None);

    var jsonMoveStr = JsonSerializer.Serialize(new Move("h2", "h4"));
    buffer = System.Text.Encoding.UTF8.GetBytes(jsonMoveStr);

    await ws.SendAsync(new ArraySegment<byte>(buffer, 0, jsonMoveStr.Length),
                       WebSocketMessageType.Text, true, CancellationToken.None);

    var jsonBoardStr = "";

    while (true)
    {
      var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

      jsonBoardStr +=
        System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);

      if (result.EndOfMessage)
        break;
    }

    Assert.Equal(jsonBoardStr, expected);
  }
}
