using System;
using System.Collections.Generic;
using ChessWinForms.Models;

namespace ChessWinForms
{
    // Definim un delegate pentru eveniment (în loc de Action)
    public delegate void GameStateChangedHandler();

    public class GameManager
    {
        public Board Board { get; private set; }
        public PieceColor CurrentTurn { get; private set; }
        public bool IsGameOver { get; private set; }
        public string GameOverMessage { get; private set; }

        public event GameStateChangedHandler OnGameStateChanged;

        public GameManager()
        {
            ResetGame();
        }

        public void ResetGame()
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

            // 1. Validare: Există piesă și e rândul ei?
            if (piece == null || piece.Color != CurrentTurn) return false;

            // 2. Validare: Este o mutare posibilă? (Folosim foreach, fara LINQ)
            List<Position> moves = Board.GetPossibleMoves(from);
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

            // 3. Validare Rege (să nu intre în șah)
            if (piece.Type == PieceType.King)
            {
                PieceColor opponent = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                if (Board.IsSquareAttacked(to, opponent))
                {
                    return false;
                }
            }

            ExecuteMoveInternal(from, to);
            return true;
        }

        public void ApplyRemoteMove(Position from, Position to)
        {
            ExecuteMoveInternal(from, to);
        }

        private void ExecuteMoveInternal(Position from, Position to)
        {
            Board.MovePiece(from, to);

            // Schimbare tură
            if (CurrentTurn == PieceColor.White)
                CurrentTurn = PieceColor.Black;
            else
                CurrentTurn = PieceColor.White;

            UpdateGameStatus();

            // Declanșare eveniment (fără ?.Invoke)
            if (OnGameStateChanged != null)
            {
                OnGameStateChanged();
            }
        }

        private void UpdateGameStatus()
        {
            PieceColor opponent = CurrentTurn;
            PieceColor mover = (CurrentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;

            Position tempPos;
            if (!Board.TryFindKingPosition(opponent, out tempPos))
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