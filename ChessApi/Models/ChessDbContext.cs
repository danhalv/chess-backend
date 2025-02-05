using ChessApi.Models.Chess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChessApi.Models;

public class ChessDbContext : DbContext
{
  public ChessDbContext(DbContextOptions<ChessDbContext> options)
    : base(options)
  { }

  public DbSet<ChessGame> ChessGames { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Configure the ChessGame -> Move relationship (one-to-many)
    modelBuilder.Entity<ChessGame>()
      .HasMany(cg => cg.Moves)              // Chessgame has many moves
      .WithOne()                            // Moves has one chessgame
      .HasForeignKey("ChessGameId")         // Foreign key in move
      .OnDelete(DeleteBehavior.Cascade);    // Cascade delete for related moves for chessgames

    // Define the shadow property "Id" for Moves
    modelBuilder.Entity<Move>()
      .Property<long>("Id") // Explicitly set the type for the shadow property "Id"
      .IsRequired();        // Make it required (as it's a primary key)

    // Define Move as an entity with a primary key
    modelBuilder.Entity<Move>()
      .HasKey("Id");

    // Use TPH inheritance strategy at the entity level for Move and its subclasses
    modelBuilder.Entity<Move>()
      .HasDiscriminator<string>("MoveType")       // Discriminator for TPH
      .HasValue<Move>("Move")                     // Base type
      .HasValue<CastlingMove>("CastlingMove")     // Subclass
      .HasValue<PromotionMove>("PromotionMove");  // Subclass
  }
}
