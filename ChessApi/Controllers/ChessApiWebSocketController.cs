using System.Collections.Concurrent;
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

  static readonly ConcurrentDictionary<long, (WebSocket whitePlayer, WebSocket blackPlayer)> _players =
      new ConcurrentDictionary<long, (WebSocket whitePlayer, WebSocket blackPlayer)>();

  [Route("/ws/chessgames/{id}")]
  public async Task Get(long id)
  {
    if (!HttpContext.WebSockets.IsWebSocketRequest)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    if (await IsGameFullAsync(id))
    {
      HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
      return;
    }

    using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

    await AddPlayerToGameAsync(id, ws);
    await WebSocketHandler(id, ws);
  }

  private async Task WebSocketHandler(long gameId, WebSocket ws)
  {
    if (Db is null)
    {
      await ws.CloseOutputAsync(WebSocketCloseStatus.InternalServerError,
                                "Could not get ChessDbContext service",
                                CancellationToken.None);
      return;
    }

    var chessgame = await Db!.ChessGames
                             .Include("ChessMoves")
                             .FirstOrDefaultAsync(c => c.Id == gameId);

    if (chessgame is null)
    {
      await ws.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData,
                                "Invalid chess game id",
                                CancellationToken.None);
      return;
    }

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

      var sendResponse = async (WebSocket ws, int msgLen) =>
        await ws.SendAsync(new ArraySegment<byte>(buffer, 0, msgLen),
                           result.MessageType,
                           result.EndOfMessage,
                           CancellationToken.None);

      var broadcastResponse = async (int msgLen) =>
      {
        if (_players.TryGetValue(gameId, out (WebSocket whitePlayer, WebSocket blackPlayer) players))
        {
          await sendResponse(players.whitePlayer, msgLen);
          await sendResponse(players.blackPlayer, msgLen);
        }
      };

      switch (wsRequest.RequestType)
      {
        case WebSocketRequestType.MakeMove:
          {
            if (_players.TryGetValue(gameId, out (WebSocket whitePlayer, WebSocket blackPlayer) players))
            {
              bool isPlayerTurn =
                (ws == players.whitePlayer && board.Turn == Color.White)
                || (ws == players.blackPlayer && board.Turn == Color.Black);

              if (!isPlayerTurn)
                break;
            }

            var sentMove = ((MakeMoveRequest)wsRequest).Move;
            board.MakeMove(sentMove);
            chessgame.ChessMoves.Add(new ChessMove(sentMove.Src, sentMove.Dst));
            chessgame.Turn = board.Turn;
            await Db.SaveChangesAsync();

            var jsonBoardStr = JsonSerializer.Serialize(board);
            buffer = System.Text.Encoding.UTF8.GetBytes(jsonBoardStr);

            await broadcastResponse(jsonBoardStr.Length);

            break;
          }
        case WebSocketRequestType.GetMoves:
          {
            var moves = board.LegalMoves(((GetMovesRequest)wsRequest).Tile);
            var movesJsonStr = JsonSerializer.Serialize(moves);
            buffer = System.Text.Encoding.UTF8.GetBytes(movesJsonStr);

            await sendResponse(ws, movesJsonStr.Length);

            break;
          }
        case WebSocketRequestType.GetBoard:
          {
            var jsonBoardStr = JsonSerializer.Serialize(board);
            buffer = System.Text.Encoding.UTF8.GetBytes(jsonBoardStr);

            await sendResponse(ws, jsonBoardStr.Length);

            break;
          }
        default:
          break;
      }

      result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer),
                                     CancellationToken.None);
    }

    await ws.CloseOutputAsync(result.CloseStatus.Value,
                              result.CloseStatusDescription,
                              CancellationToken.None);
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
        WebSocketRequestType.GetBoard =>
          JsonSerializer.Deserialize<GetBoardRequest>(wsRequestJsonStr),
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

  private async Task<bool> IsGameFullAsync(long gameId)
  {
    if (_players.TryGetValue(gameId, out (WebSocket whitePlayer, WebSocket blackPlayer) players))
    {
      if (players.whitePlayer != null && players.blackPlayer != null)
        return true;
    }

    return false;
  }

  private async Task<bool> AddPlayerToGameAsync(long gameId, WebSocket ws)
  {
    if (await IsGameFullAsync(gameId))
      return false;

    if (_players.TryGetValue(gameId, out (WebSocket whitePlayer, WebSocket blackPlayer) players))
    {
      if (_players.TryUpdate(gameId,
                             (players.whitePlayer, ws),
                             (players.whitePlayer, null)))
        return true;
    }
    else
    {
      if (_players.TryAdd(gameId, (ws, null)))
        return true;
    }

    return false;
  }
}
