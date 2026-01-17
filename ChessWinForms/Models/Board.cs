
using System.Collections.Generic;


namespace ChessWinForms.Models
{
    public class Board
    {

        

        public Piece[,] Cells { get; } = new Piece[8, 8];

        public Piece GetPiece(Position p) => p.IsValid() ? Cells[p.Row, p.Column] : null;

        public void SetPiece(Position p, Piece piece)
        {
            if (!p.IsValid()) return;
            Cells[p.Row, p.Column] = piece;
            if (piece != null)
                piece.Position = p;
        }

        public void MovePiece(Position from, Position to)
        {
            Piece piece = GetPiece(from);
            if(piece == null) return;
            SetPiece(to, piece);
            SetPiece(from, null);
            piece.Position = to;
        }

        public void InitializeStandardSetup()
        {
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    Cells[r, c] = null;

            // Initializare rook de tipul piece (polimorfism<3)
            
            Piece blackRookA = new Rook(PieceColor.Black, new Position(0, 0));
            Piece blackRookH = new Rook(PieceColor.Black, new Position(0, 7));
            SetPiece(blackRookA.Position, blackRookA);
            SetPiece(blackRookH.Position, blackRookH);

            Piece whiteRookA = new Rook(PieceColor.White, new Position(7, 0));
            Piece whiteRookH = new Rook(PieceColor.White, new Position(7, 7));
            SetPiece(whiteRookA.Position, whiteRookA);
            SetPiece(whiteRookH.Position, whiteRookH);
        

        // Piesele negre de sus
        
            SetPiece(new Position(0, 1), new Knight(PieceColor.Black, new Position(0, 1)));
            SetPiece(new Position(0, 2), new Bishop(PieceColor.Black, new Position(0, 2)));
            SetPiece(new Position(0, 3), new Queen(PieceColor.Black, new Position(0, 3)));
            SetPiece(new Position(0, 4), new King(PieceColor.Black, new Position(0, 4)));
            SetPiece(new Position(0, 5), new Bishop(PieceColor.Black, new Position(0, 5)));
            SetPiece(new Position(0, 6), new Knight(PieceColor.Black, new Position(0, 6)));
           

            //Pionii albi/negrii
            for (int c = 0; c < 8; c++)
                SetPiece(new Position(1, c), new Pawn(PieceColor.Black, new Position(1, c)));

            
            for (int c = 0; c < 8; c++)
                SetPiece(new Position(6, c), new Pawn(PieceColor.White, new Position(6, c)));


            // Piesele albe de jos
            SetPiece(new Position(7, 1), new Knight(PieceColor.White, new Position(7, 1)));
            SetPiece(new Position(7, 2), new Bishop(PieceColor.White, new Position(7, 2)));
            SetPiece(new Position(7, 3), new Queen(PieceColor.White, new Position(7, 3)));
            SetPiece(new Position(7, 4), new King(PieceColor.White, new Position(7, 4)));
            SetPiece(new Position(7, 5), new Bishop(PieceColor.White, new Position(7, 5)));
            SetPiece(new Position(7, 6), new Knight(PieceColor.White, new Position(7, 6)));
            
        }



        public List<Position> GetPossibleMoves(Position pos)
        {
            Piece piece = GetPiece(pos);
            if (piece == null) return new List<Position>();
            return piece.GetPossibleMoves(this);
        }

       
        public Position TryFindKingPosition(PieceColor color)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece p = Cells[r, c];
                    if (p != null && p.Type == PieceType.King && p.Color == color)
                    {
                        return  new Position(r, c);
                        
                    }
                }
            }

            return null;
        }



       
        public bool IsInCheck(PieceColor color)
        {
            Position kingPos = TryFindKingPosition(color);
            if (kingPos == null) return false;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece attacker = Cells[r, c];
                    if (attacker == null || attacker.Color == color) continue;
                    var moves = attacker.GetPossibleMoves(this);
                    for (int i = 0; i < moves.Count; i++)
                    {
                        var m = moves[i];
                        if (m.Row == kingPos.Row && m.Column == kingPos.Column)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool HasAnyLegalMove(PieceColor color)
        {
            
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = Cells[r, c];
                    if (piece == null || piece.Color != color) continue;

                  
                    var moves = piece.GetPossibleMoves(this);

                    foreach (var target in moves)
                    {
                       
                        Position start = piece.Position;
                        Piece captured = GetPiece(target);

                        
                        SetPiece(target, piece);
                        SetPiece(start, null);
                      
                        bool kingSafe = !IsInCheck(color);

                        SetPiece(start, piece);
                        SetPiece(target, captured);

                        if (kingSafe) return true;
                    }
                }
            }

            return false;
        }

        public bool IsCheckmate(PieceColor color)
        {
            return IsInCheck(color) && !HasAnyLegalMove(color);
            
        }

        public bool IsSquareAttacked(Position pos, PieceColor Color)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var attacker = Cells[r, c];
                    if (attacker == null || attacker.Color != Color) continue;

                    
                    if (attacker.Type == PieceType.King) continue;

                    var moves = attacker.GetPossibleMoves(this);

                    foreach (var m in moves)
                    {
                        if (m.Row == pos.Row && m.Column == pos.Column)
                            return true;
                    }
                }
            }   
            return false;
        }




    }
}
