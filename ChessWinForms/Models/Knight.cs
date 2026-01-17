using System.Collections.Generic;

namespace ChessWinForms.Models
{
    public class Knight : Piece
    {
        public Knight(PieceColor color, Position pos) : base(color, pos)
        {
            Type = PieceType.Knight;
        }

        public override List<Position> GetPossibleMoves(Board board, bool ignoreCheck = false)
        {
            var moves = new List<Position>();
            int[] dr = { -2, -2, -1, -1, 1, 1, 2, 2 };
            int[] dc = { -1, 1, -2, 2, -2, 2, -1, 1 };

            for (int i = 0; i < 8; i++)
            {
                int r = Position.Row + dr[i];
                int c = Position.Column + dc[i];
                Position target = new Position(r, c);
                if (!target.IsValid()) continue;
                Piece  piece = board.GetPiece(target);
                if (piece == null || piece.Color != this.Color)
                    moves.Add(target);
            }

            return moves;
        }
    }
}