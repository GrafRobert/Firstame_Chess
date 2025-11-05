using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWinForms.Models
{
    public class King : Piece
    {
        public King(PieceColor color, Position pos) : base(color, pos)
        {
            Type = PieceType.King;
        }

        public override List<Position> GetPossibleMoves(Board board, bool ignoreCheck = false)
        {
            var moves = new List<Position>();
            int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };

            PieceColor enemyColor = this.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;

            for (int i = 0; i < 8; i++)
            {
                int r = Position.Row + dr[i];
                int c = Position.Column + dc[i];
                Position target = new Position(r, c);
                if (!target.IsValid()) continue;

                Piece captured = board.GetPiece(target);
                if (captured == null || captured.Color != this.Color)
                {
                    Console.WriteLine($"Checking square {target.Row}, {target.Column} → attacked: {board.IsSquareAttacked(target, enemyColor)}");

                    if (!board.IsSquareAttacked(target, enemyColor))
                    {
                        moves.Add(target);
                    }
                }
            }

            return moves;
        }




    }
}
