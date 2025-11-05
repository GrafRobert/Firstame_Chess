using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWinForms.Models
{
    public class Queen : Piece
    {

        public Queen(PieceColor color, Position pos) : base(color, pos)
        {
            Type = PieceType.Queen;
        }

        public override List<Position> GetPossibleMoves(Board board,  bool ignoreCheck = false)
        {
            var moves = new List<Position>();

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            int[] dr1 = { -1, -1, 1, 1 };
            int[] dc1 = { -1, 1, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                // 🔁 Mutări de turn (linie și coloană)
                int r = Position.Row + dr[i];
                int c = Position.Column + dc[i];

                while (new Position(r, c).IsValid())
                {
                    Position target = new Position(r, c);
                    Piece piece = board.GetPiece(target);

                    if (piece == null)
                    {
                        moves.Add(target);
                        Console.WriteLine($"Queen move: {target.Row}, {target.Column}"); // debug
                    }
                    else if (piece.Color != this.Color)
                    {
                        moves.Add(target);
                        Console.WriteLine($"Queen move: {target.Row}, {target.Column}"); // debug
                        break;
                    }
                    else break;

                    r += dr[i];
                    c += dc[i];
                }

                // 🔁 Mutări diagonale
                int r1 = Position.Row + dr1[i];
                int c1 = Position.Column + dc1[i];

                while (new Position(r1, c1).IsValid())
                {
                    Position target1 = new Position(r1, c1);
                    Piece piece1 = board.GetPiece(target1);

                    if (piece1 == null)
                    {
                        moves.Add(target1);
                        Console.WriteLine($"Queen move: {target1.Row}, {target1.Column}"); // debug
                    }
                    else if (piece1.Color != this.Color)
                    {
                        moves.Add(target1);
                        Console.WriteLine($"Queen move: {target1.Row}, {target1.Column}"); // debug
                        break;
                    }
                    else break;

                    r1 += dr1[i];
                    c1 += dc1[i];
                }
            }
            return moves;
        }
    }
}
