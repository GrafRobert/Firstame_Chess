using ChessWinForms.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessWinForms
{
    public partial class NetworkGameForm : Form
    {
        private Board board;
        private Panel boardPanel;
        private TcpClient client;
        private TcpListener listener;
        private NetworkStream stream;
        private bool isHost;
        private string ipAddress;
        private PieceColor myColor;
        private PieceColor currentTurn = PieceColor.White;
        private int selectedRow = -1;
        private int selectedCol = -1;

        public NetworkGameForm(bool isHost, string ipAddress)
        {
            InitializeComponent();
            this.isHost = isHost;
            this.ipAddress = ipAddress;

            board = new Board();
            board.InitializeStandardSetup();

            boardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Gray
            };
            boardPanel.Paint += BoardPanel_Paint;
            boardPanel.MouseClick += BoardPanel_MouseClick;
            Controls.Add(boardPanel);

            if (isHost)
                StartHosting();
            else
                ConnectToHost();
        }

        private async void StartHosting()
        {
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Text = "Aștept jucătorul să se conecteze...";

            client = await listener.AcceptTcpClientAsync();
            stream = client.GetStream();
            Text = "Conectat! Jocul începe.";
            myColor = PieceColor.White;
            ListenForMoves();
        }

        private async void ConnectToHost()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(IPAddress.Parse(ipAddress), 5000);
                stream = client.GetStream();
                Text = "Conectat la gazdă! Jocul începe.";
                myColor = PieceColor.Black;
                ListenForMoves();
            }
            catch
            {
                MessageBox.Show("Nu s-a putut conecta la gazdă.");
                Close();
            }
        }

        private async void ListenForMoves()
        {
            byte[] buffer = new byte[256];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] parts = msg.Split(':');
                Position from = ParsePosition(parts[0]);
                Position to = ParsePosition(parts[1]);

                board.MovePiece(from, to);
                currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                selectedRow = -1;
                selectedCol = -1;
                boardPanel.Invalidate();
            }
        }

        private Position ParsePosition(string s)
        {
            string[] parts = s.Split(',');
            return new Position(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        private void SendMove(Position from, Position to)
        {
            string msg = $"{from.Row},{from.Column}:{to.Row},{to.Column}";
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream.Write(data, 0, data.Length);
        }

        private bool IsMyTurn()
        {
            return currentTurn == myColor;
        }

        private int GetCellSize()
        {
            int size = Math.Min(boardPanel.Width, boardPanel.Height);
            return size / 8;
        }

        private void BoardPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
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

                var moves = board.GetPossibleMoves(new Position(selectedRow, selectedCol));
                foreach (var m in moves)
                {
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
                case PieceType.Pawn: suffix = "p"; break;
                case PieceType.Rook: suffix = "r"; break;
                case PieceType.Knight: suffix = "n"; break;
                case PieceType.Bishop: suffix = "b"; break;
                case PieceType.Queen: suffix = "q"; break;
                case PieceType.King: suffix = "k"; break;
                default: suffix = "?"; break;
            }

            string imageName = $"{prefix}{suffix}.png";
            string imagePath = System.IO.Path.Combine(Application.StartupPath, "Resources", imageName);

            if (System.IO.File.Exists(imagePath))
            {
                using (Image img = Image.FromFile(imagePath))
                {
                    g.DrawImage(img, rect);
                }
            }
            //else
            //{
            //     Font f = new Font("Arial", Math.Max(8, rect.Height / 3));
            //     Brush br = new SolidBrush(Color.Black);
            //    string fallbackText = prefix.ToUpper() + suffix;
            //    SizeF sz = g.MeasureString(fallbackText, f);
            //    PointF pt = new PointF(rect.X + (rect.Width - sz.Width) / 2f, rect.Y + (rect.Height - sz.Height) / 2f);
            //    g.DrawString(fallbackText, f, br, pt);
            //}
        }

        private void BoardPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (!IsMyTurn()) return;

            int cell = GetCellSize();
            int col = e.X / cell;
            int row = e.Y / cell;
            Position pos = new Position(row, col);
            if (!pos.IsValid()) return;

            Piece clickedPiece = board.GetPiece(pos);

            if (selectedRow < 0 || selectedCol < 0)
            {
                if (clickedPiece != null && clickedPiece.Color == myColor)
                {
                    selectedRow = pos.Row;
                    selectedCol = pos.Column;
                    boardPanel.Invalidate();
                }
                return;
            }

            Position from = new Position(selectedRow, selectedCol);
            List<Position> moves = board.GetPossibleMoves(from);
            bool canMove = moves.Exists(m => m.Row == pos.Row && m.Column == pos.Column);

            if (canMove)
            {
                board.MovePiece(from, pos);
                SendMove(from, pos);
                currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                selectedRow = -1;
                selectedCol = -1;
                boardPanel.Invalidate();
            }
            else
            {
                if (clickedPiece != null && clickedPiece.Color == myColor)
                {
                    selectedRow = pos.Row;
                    selectedCol = pos.Column;
                    boardPanel.Invalidate();
                }
            }
        }
        private void StartMenuForm_Load(object sender, EventArgs e) { }
        private void NetworkGameForm_Load(object sender, EventArgs e) { }
        private void NetworkSetupForm_Load(object sender, EventArgs e) { }
    }
}