
using System.Collections.Generic;


namespace ChessWinForms.Models
{
    public class Rook : Piece
    {
        public Rook(PieceColor color, Position pos) : base(color, pos)
        {
            Type = PieceType.Rook;
        }

        public override List<Position> GetPossibleMoves(Board board, bool ignoreCheck = false)
        {
            var moves = new List<Position>();

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for(int dir= 0; dir<4;dir++)
            {
                int r = Position.Row + dr[dir];
                int c = Position.Column + dc[dir];
                
                while(new Position(r,c).IsValid())
                {
                    Position target = new Position(r,c);
                    Piece p = board.GetPiece(target);

                    if (p == null)
                    {
                        moves.Add(target);
                    }
                    else 
                        if(p.Color!=this.Color)
                    {
                        moves.Add(target);
                        break;
                    }
                    r += dr[dir];
                    c += dc[dir];
                }

            }

            return moves;


        }
    }
}
