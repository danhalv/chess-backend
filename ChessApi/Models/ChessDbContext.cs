using ChessApi.Models.Chess;
using Microsoft.EntityFrameworkCore;

namespace ChessApi.Models;

public class ChessDbContext : DbContext
{
  public ChessDbContext(DbContextOptions<ChessDbContext> options)
    : base(options)
  { }

  public DbSet<ChessGame> ChessGames { get; set; }
}
