
using System.Collections.Generic;


namespace ChessWinForms.Models
{
    public class Pawn : Piece
    {
        public Pawn(PieceColor color, Position pos) : base(color, pos)
        {
            Type = PieceType.Pawn;
        }

        public override List<Position> GetPossibleMoves(Board board, bool ignoreCheck = false)
        {
            var moves = new List<Position>();
            int dir;
            int startRow;
            if (Color == PieceColor.White)
            {
                dir = -1;
                startRow = 6;
            }
            else
            {
                dir = 1;
                startRow = 1;
            }

            var one = new Position(Position.Row + dir, Position.Column);

            if(one.IsValid() && board.GetPiece(one) == null)
                moves.Add(one);

            var two = new Position(Position.Row + 2 * dir, Position.Column);

            if (Position.Row == startRow && two.IsValid() && board.GetPiece(two)==null)
                moves.Add(two);

            Position captureLeft = new Position(Position.Row + dir, Position.Column - 1);
            Position captureRight = new Position(Position.Row + dir, Position.Column + 1);

            if ((captureLeft.IsValid()))
            {
                Piece p = board.GetPiece(captureLeft);

                if(p!=null && p.Color != this.Color)
                    moves.Add(captureLeft);
            }


            if ((captureRight.IsValid()))
            {
                Piece p = board.GetPiece(captureRight);

                if (p != null && p.Color != this.Color)
                    moves.Add(captureRight);
            }


            return moves;

        }
    }
}
