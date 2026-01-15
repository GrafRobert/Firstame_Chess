using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChessWinForms.Models;

namespace ChessWinForms
{
    public partial class Form1 : Form
    {
        private GameManager gameManager;
        private int selectedRow = -1;
        private int selectedCol = -1;


        private Label lblTurn;

        public Form1()
        {
            InitializeComponent();


            lblTurn = new Label();
            lblTurn.AutoSize = true;
            lblTurn.Location = new Point(10, 10);
            lblTurn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTurn.BackColor = Color.White;
            lblTurn.Text = "Tura: Alb";
            this.Controls.Add(lblTurn);
            lblTurn.BringToFront();


            SetDoubleBuffered(boardPanel);

            gameManager = new GameManager();

            gameManager.OnGameStateChanged += OnGameStateChanged;

            this.boardPanel.Paint += new PaintEventHandler(BoardPanel_Paint);
            this.boardPanel.MouseClick += new MouseEventHandler(BoardPanel_MouseClick);
            this.boardPanel.Resize += new EventHandler(BoardPanel_Resize);
        }

        private void OnGameStateChanged()
        {

            string numeTura = "";
            if (gameManager.CurrentTurn == PieceColor.White)
                numeTura = "Alb";
            else
                numeTura = "Negru";

            lblTurn.Text = "Tura: " + numeTura;



            boardPanel.Invalidate();

            if (gameManager.IsGameOver)
            {
                this.BeginInvoke(new MethodInvoker(ShowGameOverAndClose));
            }
        }

        private void ShowGameOverAndClose()
        {
            GameOverForm gameOverForm = new GameOverForm(gameManager.GameOverMessage);
            gameOverForm.ShowDialog();
            this.Close();
        }

        private void BoardPanel_Resize(object sender, EventArgs e)
        {
            this.boardPanel.Invalidate();
        }

        public static void SetDoubleBuffered(Control c)
        {
            System.Reflection.PropertyInfo aProp =
                typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (aProp != null)
                aProp.SetValue(c, true, null);
        }

        private void Form1_Load(object sender, EventArgs e) { }

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

                    Brush b = new SolidBrush(dark ? Color.SaddleBrown : Color.BurlyWood);
                    g.FillRectangle(b, rect);
                    b.Dispose();

                    Piece p = gameManager.Board.GetPiece(new Position(r, c));
                    if (p != null)
                        DrawPiece(g, p, rect);
                }
            }

            if (selectedRow >= 0 && selectedCol >= 0)
            {
                Rectangle selRect = new Rectangle(selectedCol * cell, selectedRow * cell, cell, cell);
                Pen pen = new Pen(Color.Red, 3);
                g.DrawRectangle(pen, selRect);
                pen.Dispose();

                List<Position> moves = gameManager.Board.GetPossibleMoves(new Position(selectedRow, selectedCol));
                foreach (Position m in moves)
                {
                    Rectangle center = new Rectangle(m.Column * cell + cell / 4, m.Row * cell + cell / 4, cell / 2, cell / 2);
                    Brush br = new SolidBrush(Color.FromArgb(160, Color.Green));
                    g.FillEllipse(br, center);
                    br.Dispose();
                }
            }
        }

        private void DrawPiece(Graphics g, Piece piece, Rectangle rect)
        {
            string prefix = (piece.Color == PieceColor.White) ? "w" : "b";
            string suffix = "";

            switch (piece.Type)
            {
                case PieceType.Pawn: suffix = "p"; break;
                case PieceType.Rook: suffix = "r"; break;
                case PieceType.Knight: suffix = "n"; break;
                case PieceType.Bishop: suffix = "b"; break;
                case PieceType.Queen: suffix = "q"; break;
                case PieceType.King: suffix = "k"; break;
                default: suffix = "?"; break;
            }

            string imageName = prefix + suffix + ".png";
            string imagePath = System.IO.Path.Combine(Application.StartupPath, "Resources", imageName);

            if (System.IO.File.Exists(imagePath))
            {
                Image img = Image.FromFile(imagePath);
                g.DrawImage(img, rect);
                img.Dispose();
            }
            //else
            //{
            //    // Fallback text
            //    Font f = new Font("Arial", Math.Max(8, rect.Height / 3));
            //    Brush br = new SolidBrush(Color.Black);
            //    string fallbackText = prefix.ToUpper() + suffix;
            //    SizeF sz = g.MeasureString(fallbackText, f);
            //    PointF pt = new PointF(rect.X + (rect.Width - sz.Width) / 2f, rect.Y + (rect.Height - sz.Height) / 2f);
            //    g.DrawString(fallbackText, f, br, pt);
            //    f.Dispose();
            //    br.Dispose();
            //}
        }

        private Position PixelToPosition(Point p)
        {
            int cell = GetCellSize();
            return new Position(p.Y / cell, p.X / cell);
        }

        private void BoardPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameManager.IsGameOver) return;
            Position pos = PixelToPosition(e.Location);
            if (!pos.IsValid()) return;

            Piece clickedPiece = gameManager.Board.GetPiece(pos);

            if (selectedRow < 0 || selectedCol < 0)
            {
                if (clickedPiece != null && clickedPiece.Color == gameManager.CurrentTurn)
                {
                    selectedRow = pos.Row;
                    selectedCol = pos.Column;
                    boardPanel.Invalidate();
                }
                return;
            }

            if (selectedRow == pos.Row && selectedCol == pos.Column)
            {
                selectedRow = -1;
                selectedCol = -1;
                boardPanel.Invalidate();
                return;
            }

            Position from = new Position(selectedRow, selectedCol);

            if (gameManager.TryMove(from, pos))
            {
                selectedRow = -1;
                selectedCol = -1;
            }
            else
            {
                if (clickedPiece != null && clickedPiece.Color == gameManager.CurrentTurn)
                {
                    selectedRow = pos.Row;
                    selectedCol = pos.Column;
                    boardPanel.Invalidate();
                }
            }
        }

   


    }
}