using ChessApi.Models;
using ChessLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChessApi.Controllers;

[Route("api/chessgames/")]
[ApiController]
public class ChessApiHttpController : ControllerBase
{
  private readonly ChessDbContext _db;

  public ChessApiHttpController(ChessDbContext db)
  {
    _db = db;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<ChessGame>>> GetChessGames()
  {
    return await _db.ChessGames
                    .Include("Moves")
                    .ToListAsync();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ChessGame>> GetChessGame(long id)
  {
    var chessgame = await _db.ChessGames
                             .Include("Moves")
                             .FirstOrDefaultAsync(c => c.Id == id);

    if (chessgame is null)
      return NotFound();

    return Ok(chessgame);
  }

  [HttpPost]
  public async Task<ActionResult<ChessGame>> CreateChessGame()
  {
    var chessgame = new ChessGame
    {
      Id = 0,
      Turn = Color.White,
      Moves = new List<Move>()
    };

    _db.ChessGames.Add(chessgame);
    await _db.SaveChangesAsync();

    return Created($"/api/chessgames/{chessgame.Id}", chessgame);
  }
}
