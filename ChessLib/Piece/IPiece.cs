namespace ChessLib;

public interface IPiece
{
  Color Color { get; }
  bool HasMoved { get; set; }
  bool IsEnpassantable { get; set; }
  char CharRepresentation { get; }

  List<Move> GetMoves(Board board, int pieceTilePos);
}
