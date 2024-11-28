
namespace ChessApi.Models;

public interface IPiece
{
  Color Color { get; }
  bool HasMoved { get; set; }

  List<Move> GetMoves(Board board, int pieceTilePos);
}
