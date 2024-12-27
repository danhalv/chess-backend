namespace ChessApi.Models.Chess;

public interface IPiece
{
  Color Color { get; }
  bool HasMoved { get; set; }
  char CharRepresentation { get; }

  List<Move> GetMoves(Board board, int pieceTilePos);
}
