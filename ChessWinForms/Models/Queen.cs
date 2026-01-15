using System;
using System.Collections.Generic;


namespace ChessWinForms.Models
{
    public class Queen : Piece
    {

        public Queen(PieceColor color, Position pos) : base(color, pos)
        {
            Type = PieceType.Queen;
        }

        public override List<Position> GetPossibleMoves(Board board, bool ignoreCheck = false)
        {
            var moves = new List<Position>();

            // Toate cele 8 direcții
            int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int r = Position.Row + dr[i];
                int c = Position.Column + dc[i];

                while (new Position(r, c).IsValid())
                {
                    Position target = new Position(r, c);
                    Piece p = board.GetPiece(target);

                    if (p == null)
                    {
                        moves.Add(target);
                    }
                    else
                    {
                        if (p.Color != this.Color)
                        {
                            moves.Add(target);
                        }
                        // STOP la orice piesă
                        break;
                    }
                    r += dr[i];
                    c += dc[i];
                }
            }
            return moves;
        }
    }
}