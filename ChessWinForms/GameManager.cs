using System;
using System.Collections.Generic;
using ChessWinForms.Models;

namespace ChessWinForms
{
    

    public class GameManager
    {
        public Board Board { get; private set; }
        public PieceColor CurrentTurn { get; private set; }
        public bool IsGameOver { get; private set; }
        public string GameOverMessage { get; private set; }

        public event Action OnGameStateChanged;

        public GameManager()
        {
            Board = new Board();
            Board.InitializeStandardSetup();
            CurrentTurn = PieceColor.White;
            IsGameOver = false;
            GameOverMessage = "";
        }


        public bool TryMove(Position from, Position to)
        {
            if (IsGameOver) return false;

            Piece piece = Board.GetPiece(from);

            // 1. Verificări de bază (dacă e piesa ta)
            if (piece == null || piece.Color != CurrentTurn) return false;

            // 2. Verificăm dacă mutarea este fizic posibilă pentru acea piesă
            var moves = Board.GetPossibleMoves(from);
            bool isPossible = false;
            foreach (Position m in moves)
            {
                if (m.Row == to.Row && m.Column == to.Column)
                {
                    isPossible = true;
                    break;
                }
            }
            if (!isPossible) return false;

            // --- MODIFICARE AICI: SIMULARE MUTARE PENTRU A VERIFICA ȘAHUL ---

            // A. Salvăm starea de pe pătratul țintă (poate fi o piesă inamică sau null)
            Piece capturedPiece = Board.GetPiece(to);

            // B. Executăm mutarea temporar pe tablă
            // Folosim SetPiece pentru a muta piesa fără a declanșa schimbarea turei
            Board.SetPiece(to, piece);
            Board.SetPiece(from, null);

            // C. Verificăm: După această mutare, Regele meu este în șah?
            bool kingInCheck = Board.IsInCheck(CurrentTurn);

            // D. Dăm "Undo" la mutare (revenim la starea inițială)
            Board.SetPiece(from, piece); // Punem piesa noastră înapoi
            Board.SetPiece(to, capturedPiece); // Punem piesa capturată (dacă exista) înapoi

            // E. Decizia
            if (kingInCheck)
            {
                // Dacă mutarea ne lasă (sau ne bagă) în șah, este ilegală!
                return false;
            }
            // -------------------------------------------------------------

            // Dacă am trecut de verificare, executăm mutarea reală
            ExecuteMove(from, to);
            return true;
        }

        public void RemoteMove(Position from, Position to)
        {
            ExecuteMove(from, to);
        }

        private void ExecuteMove(Position from, Position to)
        {
            Board.MovePiece(from, to);

          
            if (CurrentTurn == PieceColor.White)
                CurrentTurn = PieceColor.Black;
            else
                CurrentTurn = PieceColor.White;

            UpdateGameStatus();

            
            if (OnGameStateChanged != null)
            {
                OnGameStateChanged();
            }
        }

        private void UpdateGameStatus()
        {
            PieceColor opponent = CurrentTurn;
            PieceColor mover = (CurrentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;

            Position tempPos = Board.TryFindKingPosition(opponent);
            if (tempPos == null)
            {
                IsGameOver = true;
                GameOverMessage = opponent.ToString() + " king captured. " + mover.ToString() + " wins!";
            }
            else if (Board.IsCheckmate(opponent))
            {
                IsGameOver = true;
                GameOverMessage = opponent.ToString() + " is checkmated. " + mover.ToString() + " wins!";
            }
            else if (Board.IsStalemate(opponent))
            {
                IsGameOver = true;
                GameOverMessage = "Stalemate. Draw.";
            }
        }
    }
}