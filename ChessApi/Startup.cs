using ChessApi.Controllers;
using ChessApi.Models;
using ChessApi.Models.Chess;
using Microsoft.EntityFrameworkCore;

namespace ChessApi;

public class Startup
{
  public IConfiguration Configuration { get; }

  private readonly IWebHostEnvironment _env;

  public Startup(IConfiguration configuration,
                 IWebHostEnvironment env)
  {
    Configuration = configuration;
    _env = env;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers();

    if (_env.IsStaging())
    {
      var conn = Configuration.GetConnectionString("DefaultConnection");
      services.AddDbContext<ChessDbContext>(options =>
        options.UseNpgsql(conn));
    }
    else
    {
      services.AddDbContext<ChessDbContext>(options =>
        options.UseInMemoryDatabase("ChessDB"));
    }
  }

  public void Configure(IApplicationBuilder app)
  {
    var webSocketOptions = new WebSocketOptions
    {
      KeepAliveInterval = TimeSpan.FromMinutes(2)
    };
    app.UseWebSockets(webSocketOptions);

    app.UseRouting();

    app.UseEndpoints(e =>
    {
      e.MapControllers();

      e.MapGet("/", () => "Hello World!");

      e.MapGet("/chessgames", async (ChessDbContext db) =>
          await db.ChessGames
                  .Include("ChessMoves")
                  .ToListAsync());

      e.MapGet("/chessgames/{id}", async (int id, ChessDbContext db) =>
          await db.ChessGames
                  .Include("ChessMoves")
                  .FirstOrDefaultAsync(c => c.Id == id)
            is ChessGame game
              ? Results.Ok(game)
              : Results.NotFound());

      e.MapGet("/chessgames/{id}/board", async (int id, ChessDbContext db) =>
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

      e.MapPost("/chessgames", async (ChessDbContext db) =>
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
    });
  }
}
