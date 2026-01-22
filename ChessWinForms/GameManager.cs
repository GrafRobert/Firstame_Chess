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
            if (piece == null || piece.Color != CurrentTurn) return false;

            var  moves = Board.GetPossibleMoves(from);
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


            Piece capturedPiece = Board.GetPiece(to);

            Board.SetPiece(to, piece);
            Board.SetPiece(from, null);

           
            bool kingInCheck = Board.IsInCheck(CurrentTurn);

            
            Board.SetPiece(from, piece);
            Board.SetPiece(to, capturedPiece);

           
            if (kingInCheck)
            {
               
                return false;
            }
          
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

            if (Board.IsCheckmate(opponent))
            {
                IsGameOver = true;
                GameOverMessage = opponent.ToString() + " is checkmated. " + mover.ToString() + " wins!";
            }
         
        }
    }
}