using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            piece.HasMoved = true;
            piece.Position = to;
        }

        public void InitializeStandardSetup()
        {
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    Cells[r, c] = null;

            // Black major pieces (top)
            SetPiece(new Position(0, 0), new Rook(PieceColor.Black, new Position(0, 0)));
            SetPiece(new Position(0, 1), new Knight(PieceColor.Black, new Position(0, 1)));
            SetPiece(new Position(0, 2), new Bishop(PieceColor.Black, new Position(0, 2)));
            SetPiece(new Position(0, 3), new Queen(PieceColor.Black, new Position(0, 3)));
            SetPiece(new Position(0, 4), new King(PieceColor.Black, new Position(0, 4)));
            SetPiece(new Position(0, 5), new Bishop(PieceColor.Black, new Position(0, 5)));
            SetPiece(new Position(0, 6), new Knight(PieceColor.Black, new Position(0, 6)));
            SetPiece(new Position(0, 7), new Rook(PieceColor.Black, new Position(0, 7)));

            // Black pawns
            for (int c = 0; c < 8; c++)
                SetPiece(new Position(1, c), new Pawn(PieceColor.Black, new Position(1, c)));

            // White pawns
            for (int c = 0; c < 8; c++)
                SetPiece(new Position(6, c), new Pawn(PieceColor.White, new Position(6, c)));

            // White major pieces (bottom)
            SetPiece(new Position(7, 0), new Rook(PieceColor.White, new Position(7, 0)));
            SetPiece(new Position(7, 1), new Knight(PieceColor.White, new Position(7, 1)));
            SetPiece(new Position(7, 2), new Bishop(PieceColor.White, new Position(7, 2)));
            SetPiece(new Position(7, 3), new Queen(PieceColor.White, new Position(7, 3)));
            SetPiece(new Position(7, 4), new King(PieceColor.White, new Position(7, 4)));
            SetPiece(new Position(7, 5), new Bishop(PieceColor.White, new Position(7, 5)));
            SetPiece(new Position(7, 6), new Knight(PieceColor.White, new Position(7, 6)));
            SetPiece(new Position(7, 7), new Rook(PieceColor.White, new Position(7, 7)));
        }



        public List<Position> GetPossibleMoves(Position pos)
        {
            Piece piece = GetPiece(pos);
            if (piece == null) return new List<Position>();
            return piece.GetPossibleMoves(this);
        }

        // Găsește poziția regelui pentru o culoare; returnează null dacă nu există
        // Caută poziția regelui; returnează true și pune în out poziția dacă a fost găsit
        public bool TryFindKingPosition(PieceColor color, out Position kingPos)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var p = Cells[r, c];
                    if (p != null && p.Type == PieceType.King && p.Color == color)
                    {
                        kingPos = new Position(r, c);
                        return true;
                    }
                }
            }

            kingPos = new Position(-1, -1); // valoare sentinel
            return false;
        }


        // Returnează true dacă regele de culoarea `color` este atacat acum
        public bool IsInCheck(PieceColor color)
        {
            Position kingPos;
            if (!TryFindKingPosition(color, out kingPos)) return false;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var attacker = Cells[r, c];
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


        // Verifică dacă jucătorul are vreo mutare (pseudo-legală) — folosit pentru mat/stalemate detection simplu
        public bool HasAnyPseudoMove(PieceColor color)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var p = Cells[r, c];
                    if (p == null || p.Color != color) continue;
                    var moves = p.GetPossibleMoves(this);
                    if (moves != null && moves.Count > 0) return true;
                }
            }
            return false;
        }

        // Detectare checkmate (bazată pe ideea simplă: regele e în check și nu există vreo mutare pseudo-legală)
        public bool IsCheckmate(PieceColor color)
        {
            return IsInCheck(color) && !HasAnyPseudoMove(color);

            
        }

        // Detectare stalemate (regele nu e în check, dar nu sunt mutări pseudo-legale)
        public bool IsStalemate(PieceColor color)
        {
            if (IsInCheck(color)) return false;
            return !HasAnyPseudoMove(color);
        }

        public List<Piece> GetAllPieces(PieceColor color)
        {
            var pieces = new List<Piece>();
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece p = Cells[r, c];
                    if (p != null && p.Color == color)
                        pieces.Add(p);
                }
            }
            return pieces;
        }



        public bool IsSquareAttacked(Position pos, PieceColor byColor)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var attacker = Cells[r, c];
                    if (attacker == null || attacker.Color != byColor) continue;

                    // 🔒 Ignorăm regii pentru a evita recursia infinită
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
