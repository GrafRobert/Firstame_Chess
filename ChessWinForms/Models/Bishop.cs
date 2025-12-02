
using System.Collections.Generic;


namespace ChessWinForms.Models
{
    public class Bishop : Piece
    {

        public Bishop(PieceColor color, Position pos): base(color,pos)
        {
            Type = PieceType.Bishop;
        }

        public override List<Position> GetPossibleMoves(Board board, bool ignoreCheck = false)
        {
            var moves = new List<Position>();

            int[] dr = { -1, -1, 1, 1 };
            int[] dc = { -1, 1, -1, 1 };

            for (int i = 0; i<4;i++)
            {
                int r = Position.Row + dr[i];
                int c = Position.Column + dc[i];

                while(new Position(r,c).IsValid())
                {
                    Position target = new Position(r, c);
                    Piece piece = board.GetPiece(target);

                    if(piece==null)
                        {
                        moves.Add(target);
                    }
                    else
                        if(piece.Color != this.Color)
                    {
                        moves.Add(target);
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
