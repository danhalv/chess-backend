using System.Net.WebSockets;
using System.Text.Json;
using ChessApi.Models;
using ChessApi.Models.Chess;
using ChessApi.Models.WebSocketMessages;
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

    var buffer = new byte[1024 * 8];

    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer),
                                       CancellationToken.None);

    while (!result.CloseStatus.HasValue)
    {
      var wsRequest =
        await DeserializeWebSocketRequest(buffer, result.Count, ws);

      if (wsRequest is null)
      {
        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer),
                                       CancellationToken.None);
        continue;
      }

      var board = new Board();

      foreach (var m in chessgame!.ChessMoves)
      {
        board.MakeMove(m);
      }

      var sendResponse = async (int msgLen) =>
        await ws.SendAsync(new ArraySegment<byte>(buffer, 0, msgLen),
                           result.MessageType,
                           result.EndOfMessage,
                           CancellationToken.None);

      switch (wsRequest.RequestType)
      {
        case WebSocketRequestType.MakeMove:
          {
            board.MakeMove(((MakeMoveRequest)wsRequest).Move);
            var jsonBoardStr = JsonSerializer.Serialize(board);
            buffer = System.Text.Encoding.UTF8.GetBytes(jsonBoardStr);

            await sendResponse(jsonBoardStr.Length);

            break;
          }
        case WebSocketRequestType.GetMoves:
          {
            var moves = board.LegalMoves(((GetMovesRequest)wsRequest).Tile);
            var movesJsonStr = JsonSerializer.Serialize(moves);
            buffer = System.Text.Encoding.UTF8.GetBytes(movesJsonStr);

            await sendResponse(movesJsonStr.Length);

            break;
          }
        default:
          break;
      }

      result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer),
                                     CancellationToken.None);
    }

    await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
  }

  private async Task<IWebSocketRequest?> DeserializeWebSocketRequest(
      Byte[] buffer,
      int numBufferBytes,
      WebSocket ws)
  {
    try
    {
      var wsRequestJsonStr =
        System.Text.Encoding.UTF8.GetString(buffer, 0, numBufferBytes);

      var wsBaseRequest =
        JsonSerializer.Deserialize<BaseWebSocketRequest>(wsRequestJsonStr);

      return wsBaseRequest!.RequestType switch
      {
        WebSocketRequestType.MakeMove =>
          JsonSerializer.Deserialize<MakeMoveRequest>(wsRequestJsonStr),
        WebSocketRequestType.GetMoves =>
          JsonSerializer.Deserialize<GetMovesRequest>(wsRequestJsonStr),
        _ => throw new ArgumentException(
               "Invalid enum value for WebSocketRequestType",
               nameof(wsBaseRequest.RequestType)),
      };
    }
    catch (Exception e)
    {
      buffer = System.Text.Encoding.UTF8.GetBytes(e.Message);

      await ws.SendAsync(new ArraySegment<byte>(buffer, 0, e.Message.Length),
                         WebSocketMessageType.Text,
                         true,
                         CancellationToken.None);

      return null;
    }
  }
}
