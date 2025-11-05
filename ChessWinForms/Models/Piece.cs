using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWinForms.Models
{
    public abstract class Piece
    {
        public PieceColor Color { get; set; }
        public PieceType Type { get; set; }

        public Position Position { get; set; }

        public bool HasMoved { get; set; }

        protected Piece(PieceColor color, Position pos)
        {
            Color = color;
            Position = pos;
        }

        public abstract List<Position> GetPossibleMoves(Board board, bool ignoreChech = false);

    }
}
