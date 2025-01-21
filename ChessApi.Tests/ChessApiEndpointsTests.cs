using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using ChessApi;
using ChessApi.Models;
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
    ________
    PPPPPPP_
    RNBQKBNR
    """
    .Replace("\n", String.Empty);

    var expectedBoardStr = JsonSerializer.Serialize(new Board(Color.Black,
                                                    boardStringAfterMove));
    var expectedChessGameStr = "{\"id\":1,\"turn\":0,\"chessMoves\":[{\"id\":1,\"src\":15,\"dst\":31}]}";

    var buffer = new byte[1024 * 8];

    var server = new TestServer(ChessApi.Program.CreateWebHostBuilder(new string[0]));
    var httpClient = server.CreateClient();
    var response = await httpClient.PostAsync("/api/chessgames", new ByteArrayContent(buffer));

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

    Assert.Equal(expectedBoardStr, jsonBoardStr);

    var getResponse = await httpClient.GetAsync("/api/chessgames/1");
    var chessgameJsonStr = await getResponse.Content.ReadAsStringAsync();

    Assert.Equal(expectedChessGameStr, chessgameJsonStr);
  }

  [Fact]
  public async void TestWebSocketGetMovesRequest()
  {
    var b1WhiteKnightMoves = new List<Move>()
    {
      new Move("b1", "c3"),
      new Move("b1", "a3")
    };
    var expected = JsonSerializer.Serialize(b1WhiteKnightMoves);

    var buffer = new byte[1024 * 8];

    var server = new TestServer(ChessApi.Program.CreateWebHostBuilder(new string[0]));
    var httpClient = server.CreateClient();
    var response = await httpClient.PostAsync("/api/chessgames", new ByteArrayContent(buffer));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var wsClient = server.CreateWebSocketClient();
    var ws = await wsClient.ConnectAsync(new Uri("/ws/chessgames/2"), CancellationToken.None);

    var getMovesReq = new GetMovesRequest(WebSocketRequestType.GetMoves, "b1");
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

  [Fact]
  public async void TestWebSocketConnectFullGame()
  {
    var buffer = new byte[1024 * 8];

    var server = new TestServer(ChessApi.Program.CreateWebHostBuilder(new string[0]));
    var httpClient = server.CreateClient();
    var response = await httpClient.PostAsync("/api/chessgames", new ByteArrayContent(buffer));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var wsClient = server.CreateWebSocketClient();

    var wsWhitePlayer = await wsClient.ConnectAsync(new Uri("/ws/chessgames/3"), CancellationToken.None);
    Thread.Sleep(50);

    var wsBlackPlayer = await wsClient.ConnectAsync(new Uri("/ws/chessgames/3"), CancellationToken.None);
    Thread.Sleep(50);

    // Try to connect third player but game is full
    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
      await wsClient.ConnectAsync(new Uri("/ws/chessgames/3"), CancellationToken.None));

    Assert.Equal(WebSocketState.Open, wsWhitePlayer.State);
    Assert.Equal(WebSocketState.Open, wsBlackPlayer.State);
  }

  [Fact]
  public async void TestWebSocketMakeMoveRequestBroadcast()
  {
    var boardStringAfterMove = """
    rnbqkbnr
    pppppppp
    ________
    ________
    ________
    P_______
    _PPPPPPP
    RNBQKBNR
    """
    .Replace("\n", String.Empty);

    var expected = JsonSerializer.Serialize(new Board(Color.Black,
                                                      boardStringAfterMove));

    var buffer = new byte[1024 * 8];
    var whitePlayerBuffer = new byte[1024 * 8];
    var blackPlayerBuffer = new byte[1024 * 8];
    var whitePlayerReceivedBoardStr = "";
    var blackPlayerReceivedBoardStr = "";

    var server = new TestServer(ChessApi.Program.CreateWebHostBuilder(new string[0]));
    var httpClient = server.CreateClient();
    var response = await httpClient.PostAsync("/api/chessgames", new ByteArrayContent(buffer));

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var wsClient = server.CreateWebSocketClient();

    var wsWhitePlayer = await wsClient.ConnectAsync(new Uri("/ws/chessgames/4"), CancellationToken.None);
    var whitePlayerThread = new Thread(async () =>
    {
      while (true)
      {
        var result = await wsWhitePlayer.ReceiveAsync(new ArraySegment<byte>(whitePlayerBuffer), CancellationToken.None);

        whitePlayerReceivedBoardStr += System.Text.Encoding.UTF8.GetString(whitePlayerBuffer, 0, result.Count);

        if (result.EndOfMessage)
          break;
      }
    });
    whitePlayerThread.Start();
    Thread.Sleep(50);

    var wsBlackPlayer = await wsClient.ConnectAsync(new Uri("/ws/chessgames/4"), CancellationToken.None);
    var blackPlayerThread = new Thread(async () =>
    {
      while (true)
      {
        var result = await wsBlackPlayer.ReceiveAsync(new ArraySegment<byte>(blackPlayerBuffer), CancellationToken.None);

        blackPlayerReceivedBoardStr += System.Text.Encoding.UTF8.GetString(blackPlayerBuffer, 0, result.Count);

        if (result.EndOfMessage)
          break;
      }
    });
    blackPlayerThread.Start();
    Thread.Sleep(50);

    var makeMoveReq = new MakeMoveRequest(WebSocketRequestType.MakeMove,
                                          new Move("a2", "a3"));
    var jsonMoveStr = JsonSerializer.Serialize(makeMoveReq);
    buffer = System.Text.Encoding.UTF8.GetBytes(jsonMoveStr);

    await wsWhitePlayer.SendAsync(new ArraySegment<byte>(buffer, 0, jsonMoveStr.Length),
                                  WebSocketMessageType.Text, true, CancellationToken.None);

    Thread.Sleep(50);

    whitePlayerThread.Join();
    blackPlayerThread.Join();

    Assert.Equal(expected, whitePlayerReceivedBoardStr);
    Assert.Equal(expected, blackPlayerReceivedBoardStr);
  }
}
