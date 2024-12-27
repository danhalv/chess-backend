using System.Net.WebSockets;
using System.Text.Json;
using ChessApi.Models;
using ChessApi.Models.Chess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChessApi.Controllers;

public class ChessApiWebSocketController : ControllerBase
{
  private ChessDbContext? Db =>
    HttpContext.RequestServices.GetService<ChessDbContext>();

  [Route("/ws/chessgames/{id}")]
  public async Task Get(long id)
  {
    if (!HttpContext.WebSockets.IsWebSocketRequest)
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

    using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
    await WebSocketHandler(id, ws);
  }

  private async Task WebSocketHandler(long gameId, WebSocket ws)
  {
    if (Db is null)
      await ws.CloseAsync(WebSocketCloseStatus.InternalServerError,
                          "Could not get ChessDbContext service",
                          CancellationToken.None);

    var chessgame = await Db!.ChessGames
                             .Include("ChessMoves")
                             .FirstOrDefaultAsync(c => c.Id == gameId);

    if (chessgame is null)
      await ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                          "Invalid chess game id",
                          CancellationToken.None);

    var buffer = new byte[1024 * 4];

    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!result.CloseStatus.HasValue)
    {
      var move = await DeserializeBufferMove(buffer, ws);

      if (move is null)
      {
        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        continue;
      }

      var board = new Board();

      foreach (var m in chessgame!.ChessMoves)
      {
        board.MakeMove(m);
      }
      board.MakeMove(move);

      var jsonBoardStr = JsonSerializer.Serialize(board);
      buffer = System.Text.Encoding.UTF8.GetBytes(jsonBoardStr);

      await ws.SendAsync(new ArraySegment<byte>(buffer, 0, jsonBoardStr.Length),
                         result.MessageType, result.EndOfMessage, CancellationToken.None);
      result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
  }

  private async Task<Move?> DeserializeBufferMove(Byte[] buffer, WebSocket ws)
  {
    try
    {
      var jsonMoveStr = System.Text.Encoding.UTF8.GetString(buffer);
      buffer = System.Text.Encoding.UTF8.GetBytes(jsonMoveStr);

      await ws.SendAsync(new ArraySegment<byte>(buffer, 0, jsonMoveStr.Length),
                         WebSocketMessageType.Text, true, CancellationToken.None);
      var move = JsonSerializer.Deserialize<Move>(jsonMoveStr);

      return move;
    }
    catch (Exception e)
    {
      buffer = System.Text.Encoding.UTF8.GetBytes(e.Message);

      await ws.SendAsync(new ArraySegment<byte>(buffer, 0, e.Message.Length),
                         WebSocketMessageType.Text, true, CancellationToken.None);

      return null;
    }
  }
}
