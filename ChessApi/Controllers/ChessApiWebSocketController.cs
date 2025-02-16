using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using ChessApi.Models;
using ChessApi.Models.WebSocketMessages;
using ChessLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChessApi.Controllers;

public class ChessApiWebSocketController : ControllerBase
{
  private ChessDbContext? Db =>
    HttpContext.RequestServices.GetService<ChessDbContext>();

  private WebSocket? Ws { get; set; } = null;

  private byte[] MessageBuffer { get; set; } = new byte[1024 * 8];

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

    Ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

    await AddPlayerToGameAsync(id);
    await WebSocketHandler(id);
  }

  private async Task WebSocketHandler(long gameId)
  {
    if (Db is null)
    {
      await Ws!.CloseOutputAsync(WebSocketCloseStatus.InternalServerError,
                                "Could not get ChessDbContext service",
                                CancellationToken.None);
      return;
    }

    while (Ws!.State == WebSocketState.Open)
    {
      var clientMessage = await ReceiveClientMessageAsync();

      IWebSocketRequest? WsRequest =
        await DeserializeWebSocketRequest(clientMessage);

      if (WsRequest is null)
        continue;

      var chessgame = await Db!.ChessGames
                               .Include("Moves")
                               .FirstOrDefaultAsync(c => c.Id == gameId);

      if (chessgame is null)
      {
        await Ws!.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData,
                                  "Invalid chess game id",
                                  CancellationToken.None);
        return;
      }

      var board = new Board();

      foreach (var move in chessgame!.Moves)
      {
        switch (move)
        {
          case CastlingMove castlingMove:
            board.MakeMove((CastlingMove)castlingMove);
            break;
          case EnpassantCapture enpassantCapture:
            board.MakeMove((EnpassantCapture)enpassantCapture);
            break;
          case PawnDoubleMove pawnDoubleMove:
            board.MakeMove((PawnDoubleMove)pawnDoubleMove);
            break;
          case PromotionMove promotionMove:
            board.MakeMove((PromotionMove)promotionMove);
            break;
          default:
            board.MakeMove((Move)move);
            break;
        }
      }

      switch (WsRequest.RequestType)
      {
        case WebSocketRequestType.MakeMove:
          {
            if (_players.TryGetValue(gameId, out (WebSocket whitePlayer, WebSocket blackPlayer) players))
            {
              bool isPlayerTurn =
                (Ws! == players.whitePlayer && board.Turn == Color.White)
                || (Ws! == players.blackPlayer && board.Turn == Color.Black);

              if (!isPlayerTurn)
                break;
            }

            var sentMove = ((MakeMoveRequest)WsRequest).Move;

            var makeMove = () =>
            {
              var move = board.LegalMoves().FirstOrDefault(move =>
              {
                if (move.Src == sentMove.Src && move.Dst == sentMove.Dst)
                  return true;
                return false;
              });

              if (move is null)
                return;

              switch (move)
              {
                case CastlingMove castlingMove:
                  board.MakeMove((CastlingMove)castlingMove);
                  break;
                case EnpassantCapture enpassantCapture:
                  board.MakeMove((EnpassantCapture)enpassantCapture);
                  break;
                case PawnDoubleMove pawnDoubleMove:
                  board.MakeMove((PawnDoubleMove)pawnDoubleMove);
                  break;
                case PromotionMove promotionMove:
                  board.MakeMove((PromotionMove)promotionMove);
                  break;
                default:
                  board.MakeMove((Move)move);
                  break;
              }

              chessgame!.Moves.Add(move);
            };

            makeMove();
            chessgame!.Turn = board.Turn;
            await Db!.SaveChangesAsync();

            var jsonBoardStr = JsonSerializer.Serialize(board);
            MessageBuffer = System.Text.Encoding.UTF8.GetBytes(jsonBoardStr);

            await BroadcastMessage(gameId, jsonBoardStr.Length);

            break;
          }
        case WebSocketRequestType.GetMoves:
          {
            var moves = board.LegalMoves(((GetMovesRequest)WsRequest).Tile);
            var movesJsonStr = JsonSerializer.Serialize(moves);
            MessageBuffer = System.Text.Encoding.UTF8.GetBytes(movesJsonStr);

            await SendMessage(Ws!, movesJsonStr.Length);

            break;
          }
        case WebSocketRequestType.GetBoard:
          {
            var jsonBoardStr = JsonSerializer.Serialize(board);
            MessageBuffer = System.Text.Encoding.UTF8.GetBytes(jsonBoardStr);

            await SendMessage(Ws!, jsonBoardStr.Length);

            break;
          }
        default:
          break;
      }
    }
  }

  private async Task<string> ReceiveClientMessageAsync()
  {
    var chunks = new List<string>();

    WebSocketReceiveResult result;
    do
    {
      result = await Ws!.ReceiveAsync(new ArraySegment<byte>(MessageBuffer), CancellationToken.None);
      chunks.Add(System.Text.Encoding.UTF8.GetString(MessageBuffer, 0, result.Count));
    }
    while (!result.EndOfMessage && !result.CloseStatus.HasValue);

    if (result.CloseStatus.HasValue)
    {
      await Ws!.CloseOutputAsync(result.CloseStatus.Value,
                                result.CloseStatusDescription,
                                CancellationToken.None);
    }

    return string.Concat(chunks);
  }

  private async Task<IWebSocketRequest?> DeserializeWebSocketRequest(
      string clientRequestStr)
  {
    try
    {
      var wsBaseRequest =
        JsonSerializer.Deserialize<BaseWebSocketRequest>(clientRequestStr);

      return wsBaseRequest!.RequestType switch
      {
        WebSocketRequestType.MakeMove =>
          JsonSerializer.Deserialize<MakeMoveRequest>(clientRequestStr),
        WebSocketRequestType.GetMoves =>
          JsonSerializer.Deserialize<GetMovesRequest>(clientRequestStr),
        WebSocketRequestType.GetBoard =>
          JsonSerializer.Deserialize<GetBoardRequest>(clientRequestStr),
        _ => throw new ArgumentException(
               "Invalid enum value for WebSocketRequestType",
               nameof(wsBaseRequest.RequestType)),
      };
    }
    catch (Exception e)
    {
      MessageBuffer = System.Text.Encoding.UTF8.GetBytes(e.Message);

      await Ws!.SendAsync(new ArraySegment<byte>(MessageBuffer, 0, e.Message.Length),
                         WebSocketMessageType.Text,
                         true,
                         CancellationToken.None);

      return null;
    }
  }

  private async Task SendMessage(WebSocket ws, int msgLen)
  {
    await ws.SendAsync(new ArraySegment<byte>(MessageBuffer, 0, msgLen),
                       WebSocketMessageType.Text,
                       true,
                       CancellationToken.None);
  }

  private async Task BroadcastMessage(long gameId, int msgLen)
  {
    if (_players.TryGetValue(gameId, out (WebSocket whitePlayer, WebSocket blackPlayer) players))
    {
      if (players.whitePlayer != null)
        await SendMessage(players.whitePlayer, msgLen);
      if (players.blackPlayer != null)
        await SendMessage(players.blackPlayer, msgLen);
    }
  }

  private async Task<ChessGame?> RetrieveChessGameAsync(long gameId)
  {
    var chessgame = await Db!.ChessGames
                             .Include("Moves")
                             .FirstOrDefaultAsync(c => c.Id == gameId);

    if (chessgame is null)
    {
      await Ws!.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData,
                                "Invalid chess game id",
                                CancellationToken.None);
      return null;
    }

    return chessgame;
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

  private async Task<bool> AddPlayerToGameAsync(long gameId)
  {
    if (await IsGameFullAsync(gameId))
      return false;

    if (_players.TryGetValue(gameId, out (WebSocket whitePlayer, WebSocket blackPlayer) players))
    {
      if (_players.TryUpdate(gameId,
                             (players.whitePlayer, Ws!),
                             (players.whitePlayer, null)))
        return true;
    }
    else
    {
      if (_players.TryAdd(gameId, (Ws!, null)))
        return true;
    }

    return false;
  }
}
