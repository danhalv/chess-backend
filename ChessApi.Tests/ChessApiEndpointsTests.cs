using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using ChessApi;
using ChessApi.Models.Chess;
using ChessApi.Models.WebSocketMessages;
using Microsoft.AspNetCore.TestHost;

namespace ChessApi.Tests;

public class ChessApiEndpointsTests
{
  [Fact]
  public async void TestWebSocketMakeMoveRequest()
  {
    var boardStringAfterMove = """
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

    var expected = JsonSerializer.Serialize(new Board(Color.White,
                                                      boardStringAfterMove));

    var buffer = new byte[1024 * 8];

    var server = new TestServer(ChessApi.Program.CreateWebHostBuilder(new string[0]));
    var httpClient = server.CreateClient();
    var response = await httpClient.PostAsync("/chessgames", new ByteArrayContent(buffer));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var wsClient = server.CreateWebSocketClient();
    var ws = await wsClient.ConnectAsync(new Uri("/ws/chessgames/1"), CancellationToken.None);

    var makeMoveReq = new MakeMoveRequest(WebSocketRequestType.MakeMove,
                                          new Move("h2", "h4"));
    var jsonMoveStr = JsonSerializer.Serialize(makeMoveReq);
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

  [Fact]
  public async void TestWebSocketGetMovesRequest()
  {
    var a7BlackPawnMoves = new List<Move>()
    {
      new Move("a7", "a6"),
      new Move("a7", "a5")
    };
    var expected = JsonSerializer.Serialize(a7BlackPawnMoves);

    var buffer = new byte[1024 * 8];

    var server = new TestServer(ChessApi.Program.CreateWebHostBuilder(new string[0]));
    var httpClient = server.CreateClient();
    var response = await httpClient.PostAsync("/chessgames", new ByteArrayContent(buffer));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var wsClient = server.CreateWebSocketClient();
    var ws = await wsClient.ConnectAsync(new Uri("/ws/chessgames/1"), CancellationToken.None);

    var getMovesReq = new GetMovesRequest(WebSocketRequestType.GetMoves, "a7");
    var reqJsonStr = JsonSerializer.Serialize(getMovesReq);
    buffer = System.Text.Encoding.UTF8.GetBytes(reqJsonStr);

    await ws.SendAsync(new ArraySegment<byte>(buffer, 0, reqJsonStr.Length),
                       WebSocketMessageType.Text, true, CancellationToken.None);

    var movesJsonStr = "";

    while (true)
    {
      var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer),
                                         CancellationToken.None);

      movesJsonStr +=
        System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);

      if (result.EndOfMessage)
        break;
    }

    Assert.Equal(movesJsonStr, expected);
  }
}
