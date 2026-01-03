
using System.Collections.Generic;


namespace ChessWinForms.Models
{
    public abstract class Piece
    {
        public PieceColor Color { get; set; }
        public PieceType Type { get; set; }

        public Position Position { get; set; }

        protected Piece(PieceColor color, Position pos)
        {
            Color = color;
            Position = pos;
        }

        public abstract List<Position> GetPossibleMoves(Board board, bool ignoreChech = false);

    }
}
