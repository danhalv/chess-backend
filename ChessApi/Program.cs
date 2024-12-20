using ChessApi.Models;
using ChessApi.Models.Chess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChessDbContext>(options =>
  options.UseNpgsql(conn));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/chessgames", async (ChessDbContext db) =>
    await db.ChessGames
            .Include("ChessMoves")
            .ToListAsync());

app.MapGet("/chessgames/{id}", async (int id, ChessDbContext db) =>
    await db.ChessGames
            .Include("ChessMoves")
            .FirstOrDefaultAsync(c => c.Id == id)
      is ChessGame game
        ? Results.Ok(game)
        : Results.NotFound());

app.MapGet("/chessgames/{id}/board", async (int id, ChessDbContext db) =>
{
  var game = await db.ChessGames
                     .Include("ChessMoves")
                     .FirstOrDefaultAsync(c => c.Id == id);

  if (game is null)
    return Results.NotFound();

  var board = new Board();

  foreach (var move in game.ChessMoves)
    board.MakeMove(move);

  return Results.Text(board.ToString());
});

app.MapPost("/chessgames", async (ChessDbContext db) =>
{
  var game = new ChessGame
  {
    Id = 0,
    Turn = Color.Black,
    ChessMoves = new List<ChessMove> { new ChessMove("a2", "a3") }
  };

  db.ChessGames.Add(game);
  await db.SaveChangesAsync();

  return Results.Created($"/chessgames/{game.Id}", game);
});

app.Run();
