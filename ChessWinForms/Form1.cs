using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChessWinForms.Models;

namespace ChessWinForms
{
    public partial class Form1 : Form
    {
        private Board board;
        private int selectedRow = -1;
        private int selectedCol = -1;
        private PieceColor currentTurn = PieceColor.White;
        private bool gameOver = false;

        public Form1()
        {
            InitializeComponent();
            SetDoubleBuffered(boardPanel);
            // apel la codul generat de Designer
            board = new Board();
            board.InitializeStandardSetup();
            UpdateGameStatus(); // verificare inițială (va afișa GameOver doar dacă e mat/pat/regină capturată)

            // Înregistrăm evenimentele; boardPanel este definit în Designer
            this.boardPanel.Paint += BoardPanel_Paint;
            this.boardPanel.MouseClick += BoardPanel_MouseClick;
            this.boardPanel.Resize += (s, e) => this.boardPanel.Invalidate();
           
        }

        public static void SetDoubleBuffered(Control c)
        {
            System.Reflection.PropertyInfo aProp =
                typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            // opțional: initializări la load
        }

        private int GetCellSize()
        {
            int size = Math.Min(boardPanel.Width, boardPanel.Height);
            if (size <= 0) return 1;
            return size / 8;
        }

        private void BoardPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Gray);
            int cell = GetCellSize();

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Rectangle rect = new Rectangle(c * cell, r * cell, cell, cell);
                    bool dark = ((r + c) % 2) == 1;
                    using (Brush b = new SolidBrush(dark ? Color.SaddleBrown : Color.BurlyWood))
                        g.FillRectangle(b, rect);

                    Piece p = board.GetPiece(new Position(r, c));
                    if (p != null)
                        DrawPiece(g, p, rect);
                }
            }

            if (selectedRow >= 0 && selectedCol >= 0)
            {
                Rectangle selRect = new Rectangle(selectedCol * cell, selectedRow * cell, cell, cell);
                using (Pen pen = new Pen(Color.Red, 3))
                    g.DrawRectangle(pen, selRect);

                List<Position> moves = board.GetPossibleMoves(new Position(selectedRow, selectedCol));
                for (int i = 0; i < moves.Count; i++)
                {
                    Position m = moves[i];
                    Rectangle center = new Rectangle(m.Column * cell + cell / 4, m.Row * cell + cell / 4, cell / 2, cell / 2);
                    using (Brush br = new SolidBrush(Color.FromArgb(160, Color.Green)))
                        g.FillEllipse(br, center);
                }
            }
        }

        private void DrawPiece(Graphics g, Piece piece, Rectangle rect)
        {
            string prefix = piece.Color == PieceColor.White ? "w" : "b";
            string suffix = "";

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    suffix = "p";
                    break;
                case PieceType.Rook:
                    suffix = "r";
                    break;
                case PieceType.Knight:
                    suffix = "n";
                    break;
                case PieceType.Bishop:
                    suffix = "b";
                    break;
                case PieceType.Queen:
                    suffix = "q";
                    break;
                case PieceType.King:
                    suffix = "k";
                    break;
                default:
                    suffix = "?";
                    break;
            }

            string imageName = prefix + suffix + ".png";
            string imagePath = System.IO.Path.Combine(Application.StartupPath, "Resources", imageName);

            if (System.IO.File.Exists(imagePath))
            {
                using (Image img = Image.FromFile(imagePath))
                {
                    g.DrawImage(img, rect);
                }
            }
            else
            {
                // fallback: desen text dacă imaginea lipsește
                using (Font f = new Font("Arial", Math.Max(8, rect.Height / 3)))
                using (Brush br = new SolidBrush(Color.Black))
                {
                    string fallbackText = prefix.ToUpper() + suffix;
                    SizeF sz = g.MeasureString(fallbackText, f);
                    PointF pt = new PointF(rect.X + (rect.Width - sz.Width) / 2f, rect.Y + (rect.Height - sz.Height) / 2f);
                    g.DrawString(fallbackText, f, br, pt);
                }
            }
        }



        private void UpdateGameStatus()
        {
            PieceColor opponent = currentTurn;
            PieceColor mover = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // Regele adversarului trebuie să existe
            if (!board.TryFindKingPosition(opponent, out Position oppKingPos))
            {
                ShowGameOver($"{opponent} king captured. {mover} wins!");
                gameOver = true;
                return;
            }

            // Afișăm GameOver doar pentru checkmate sau stalemate
            if (board.IsCheckmate(opponent))
            {
                ShowGameOver($"{opponent} is checkmated. {mover} wins!");
                gameOver = true;
            }
            else if (board.IsStalemate(opponent))
            {
                ShowGameOver("Stalemate. Draw.");
                gameOver = true;
            }
        }

        private Position PixelToPosition(Point p)
        {
            int cell = GetCellSize();
            int col = p.X / cell;
            int row = p.Y / cell;
            return new Position(row, col);
        }

        private void BoardPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameOver) return;
            Position pos = PixelToPosition(e.Location);
            if (!pos.IsValid()) return;

            Piece clickedPiece = board.GetPiece(pos);

            // Selectare piesă
            if (selectedRow < 0 || selectedCol < 0)
            {
                if (clickedPiece != null && clickedPiece.Color == currentTurn)
                {
                    selectedRow = pos.Row;
                    selectedCol = pos.Column;
                    boardPanel.Invalidate();
                }
                return;
            }

            int fromRow = selectedRow;
            int fromCol = selectedCol;

            // Deselecție
            if (fromRow == pos.Row && fromCol == pos.Column)
            {
                selectedRow = -1;
                selectedCol = -1;
                boardPanel.Invalidate();
                return;
            }

            List<Position> moves = board.GetPossibleMoves(new Position(fromRow, fromCol));
            bool canMove = false;
            for (int i = 0; i < moves.Count; i++)
            {
                if (moves[i].Row == pos.Row && moves[i].Column == pos.Column)
                {
                    canMove = true;
                    break;
                }
            }

            if (canMove)
            {
                var movingPiece = board.GetPiece(new Position(fromRow, fromCol));
                // Validare mutare rege: nu permite mutarea pe pătrat controlat de adversar
                if (movingPiece != null && movingPiece.Type == PieceType.King)
                {
                    PieceColor opponent = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    if (board.IsSquareAttacked(pos, opponent))
                    {
                        MessageBox.Show("Regele nu poate muta pe un pătrat controlat de adversar!", "Mutare ilegală");
                        selectedRow = -1;
                        selectedCol = -1;
                        boardPanel.Invalidate();
                        return;
                    }
                }

                // Aplicăm mutarea
                board.MovePiece(new Position(fromRow, fromCol), pos);
                selectedRow = -1;
                selectedCol = -1;

                // Schimbăm tura
                currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

                // Verificăm starea jocului numai după mutare
                UpdateGameStatus();
                boardPanel.Invalidate();
            }
            else
            {
                // Dacă s-a dat click pe o piesă proprie, selectăm acea piesă
                if (clickedPiece != null && clickedPiece.Color == currentTurn)
                {
                    selectedRow = pos.Row;
                    selectedCol = pos.Column;
                    boardPanel.Invalidate();
                }
            }
        }

        private void ShowGameOver(string message)
        {
            var gameOverForm = new GameOverForm(message);
            gameOverForm.ShowDialog();

            gameOver = true;
            selectedRow = -1;
            selectedCol = -1;
            boardPanel.Invalidate();
        }
    }
}