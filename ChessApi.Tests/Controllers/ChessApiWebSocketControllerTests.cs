using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using ChessApi.Models;
using ChessApi.Models.WebSocketMessages;
using ChessLib;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace ChessApi.Tests.Controllers;

public class ChessApiWebSocketControllerTests : ChessApiControllerTests
{
  private readonly WebSocketClient _wsClient;

  public ChessApiWebSocketControllerTests()
    : base()
  {
    _wsClient = _server.CreateWebSocketClient();
  }

  [Fact]
  public async Task MakeMoveRequest_Broadcasts_Board_After_Move()
  {
    // Arrange: Create new game
    var createResponse = await _httpClient.PostAsync("/api/chessgames/", null);

    // Assert: Game is created
    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    var createResponseContentString =
      await createResponse.Content.ReadAsStringAsync();
    var gameId = ExtractGameId(createResponseContentString);

    // Arrange: Connect websockets to created games
    var wsWhitePlayer = await ConnectToGameAsync(gameId);
    Thread.Sleep(50);
    var wsBlackPlayer = await ConnectToGameAsync(gameId);
    Thread.Sleep(50);

    // Assert: WebSocket connection is open
    Assert.Equal(WebSocketState.Open, wsWhitePlayer.State);
    Assert.Equal(WebSocketState.Open, wsBlackPlayer.State);

    // Arrange: Prepare buffers, listen for responses, and send request
    var whitePlayerBuffer = new byte[1024 * 8];
    var blackPlayerBuffer = new byte[1024 * 8];
    var whitePlayerReceiveTask =
      Task.Run(() => ReceiveServerMessageAsync(whitePlayerBuffer, wsWhitePlayer));
    var blackPlayerReceiveTask =
      Task.Run(() => ReceiveServerMessageAsync(blackPlayerBuffer, wsBlackPlayer));
    var makeMoveRequest = new MakeMoveRequest(WebSocketRequestType.MakeMove,
                                              new Move("h2", "h4"));
    var makeMoveRequestString = JsonSerializer.Serialize(makeMoveRequest);
    whitePlayerBuffer =
      System.Text.Encoding.UTF8.GetBytes(makeMoveRequestString);

    // Act: Send MakeMoveRequest and receive responses
    await wsWhitePlayer.SendAsync(
      new ArraySegment<byte>(whitePlayerBuffer, 0, makeMoveRequestString.Length),
      WebSocketMessageType.Text, true, CancellationToken.None);
    Thread.Sleep(10);
    var whitePlayerReceivedMessage = await whitePlayerReceiveTask;
    var blackPlayerReceivedMessage = await blackPlayerReceiveTask;

    // Assert: Board after move is broadcast to both players
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
    var boardAfterMove = new Board(Color.Black, boardStringAfterMove);
    var movedPiece = boardAfterMove.GetTile("h4").Piece;
    Assert.NotNull(movedPiece);
    movedPiece.HasMoved = true;
    var boardJsonAfterMove = JsonSerializer.Serialize(boardAfterMove);
    Assert.Equal(boardJsonAfterMove, whitePlayerReceivedMessage);
    Assert.Equal(boardJsonAfterMove, blackPlayerReceivedMessage);
  }

  [Fact]
  public async Task GetMovesRequest_Returns_Legal_Moves()
  {
    // Arrange: Create game and connect players
    var gameId = await CreateGameAsync();
    var (wsWhitePlayer, wsBlackPlayer) = await ConnectPlayersToGameAsync(gameId);

    // Arrange: Prepare buffers for responses
    var whitePlayerBuffer = new byte[1024 * 8];
    var whitePlayerReceiveTask =
      Task.Run(() => ReceiveServerMessageAsync(whitePlayerBuffer, wsWhitePlayer));
    var getMovesRequest = new GetMovesRequest(WebSocketRequestType.GetMoves, "b1");
    var getMovesRequestString = JsonSerializer.Serialize(getMovesRequest);
    whitePlayerBuffer =
      System.Text.Encoding.UTF8.GetBytes(getMovesRequestString);

    // Act: Send GetMovesRequest and receive responses
    await wsWhitePlayer.SendAsync(
      new ArraySegment<byte>(whitePlayerBuffer, 0, getMovesRequestString.Length),
      WebSocketMessageType.Text, true, CancellationToken.None);
    Thread.Sleep(10);
    var whitePlayerReceivedMessage = await whitePlayerReceiveTask;

    // Assert: Board after move is broadcast to both players
    var expectedMoves = new List<Move>()
    {
      new Move("b1", "c3"),
      new Move("b1", "a3")
    };
    var expectedMovesJson = JsonSerializer.Serialize(expectedMoves);
    Assert.Equal(expectedMovesJson, whitePlayerReceivedMessage);
  }

  [Fact]
  public async Task Rejects_Connection_When_Game_Is_Full()
  {
    // Arrange: Create game and connect players
    var gameId = await CreateGameAsync();
    var (wsWhitePlayer, wsBlackPlayer) = await ConnectPlayersToGameAsync(gameId);

    // Act & Assert: 3rd player is rejected when attempting to connect
    Assert.ThrowsAsync<InvalidOperationException>(async () =>
      await ConnectToGameAsync(gameId));
  }

  private async Task<long> CreateGameAsync()
  {
    // Act: Create new game
    var createResponse = await _httpClient.PostAsync("/api/chessgames/", null);

    // Assert: Game is created
    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    var createResponseContentString =
      await createResponse.Content.ReadAsStringAsync();

    return ExtractGameId(createResponseContentString);
  }

  private async Task<(WebSocket, WebSocket)> ConnectPlayersToGameAsync(long gameId)
  {
    // Act: Connect websockets (players) to created game
    var wsWhitePlayer = await ConnectToGameAsync(gameId);
    Thread.Sleep(10);
    var wsBlackPlayer = await ConnectToGameAsync(gameId);
    Thread.Sleep(10);

    // Assert: WebSocket connections are open
    Assert.Equal(WebSocketState.Open, wsWhitePlayer.State);
    Assert.Equal(WebSocketState.Open, wsBlackPlayer.State);

    return (wsWhitePlayer, wsBlackPlayer);
  }

  private async Task<WebSocket> ConnectToGameAsync(long gameId)
  {
    return await _wsClient.ConnectAsync(new Uri($"/ws/chessgames/{gameId}"),
                                        CancellationToken.None);
  }

  private static async Task<string> ReceiveServerMessageAsync(byte[] buffer, WebSocket ws)
  {
    var chunks = new List<string>();

    WebSocketReceiveResult result;
    do
    {
      result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
      chunks.Add(System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count));
    }
    while (!result.EndOfMessage);

    return string.Concat(chunks);
  }
}
