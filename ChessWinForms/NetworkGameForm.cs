using ChessWinForms.Models;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChessWinForms
{
    public partial class NetworkGameForm : Form
    {
        private GameManager gameManager;
        private Panel boardPanel;

        // ELEMENT NOU
        private Label lblTurn;

        private TcpClient client;
        private TcpListener listener;
        private NetworkStream stream;

        private bool isHost;
        private string ipAddress;
        private PieceColor myColor;

        private int selectedRow = -1;
        private int selectedCol = -1;

        private delegate void UpdateBoardDelegate(Position from, Position to);

        public NetworkGameForm(bool isHost, string ipAddress)
        {
            InitializeComponent();
            this.isHost = isHost;
            this.ipAddress = ipAddress;

            gameManager = new GameManager();
            gameManager.OnGameStateChanged += OnGameStateChanged;

            // --- CONFIGURARE LABEL TURĂ ---
            lblTurn = new Label();
            lblTurn.Dock = DockStyle.Top; // Se lipește de marginea de sus
            lblTurn.Height = 30;          // Înălțime fixă
            lblTurn.TextAlign = ContentAlignment.MiddleCenter; // Centrat
            lblTurn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTurn.Text = "Așteptare conexiune...";
            Controls.Add(lblTurn); // Adăugăm ÎNAINTE de boardPanel pentru ordinea dock-ului
            // -----------------------------

            boardPanel = new Panel();
            boardPanel.Dock = DockStyle.Fill; // Ocupă restul spațiului
            boardPanel.BackColor = Color.Gray;

            boardPanel.Paint += new PaintEventHandler(BoardPanel_Paint);
            boardPanel.MouseClick += new MouseEventHandler(BoardPanel_MouseClick);

            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (aProp != null)
                aProp.SetValue(boardPanel, true, null);

            Controls.Add(boardPanel);
            // Asigurăm ordinea corectă (Label sus, Tabla jos)
            boardPanel.BringToFront();
            lblTurn.SendToBack(); // De fapt, la DockStyle, ordinea adăugării contează. 
                                  // Label Dock Top + Panel Dock Fill = OK.

            if (isHost)
                StartHosting();
            else
                ConnectToHost();
        }

        private void OnGameStateChanged()
        {
            // --- ACTUALIZARE LABEL ---
            string numeTura = "";
            if (gameManager.CurrentTurn == PieceColor.White)
                numeTura = "Alb";
            else
                numeTura = "Negru";

            // Putem adăuga un indiciu vizual dacă e tura MEA


            lblTurn.Text = "Tura: " + numeTura;

            if (gameManager.CurrentTurn == PieceColor.White)
                lblTurn.ForeColor = Color.Black;
            else
                lblTurn.ForeColor = Color.Red;
            // ------------------------

            boardPanel.Invalidate();

            if (gameManager.IsGameOver)
            {
                this.BeginInvoke(new MethodInvoker(ShowGameOverAndClose));
            }
        }

        private void ShowGameOverAndClose()
        {
            if (!this.IsDisposed && !this.Disposing)
            {
                GameOverForm gameOverForm = new GameOverForm(gameManager.GameOverMessage);
                gameOverForm.ShowDialog();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void OnConnectionLost()
        {
            if (this.IsDisposed || this.Disposing) return;
            MessageBox.Show("Conexiunea cu celălalt jucător a fost pierdută.", "Deconectare");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private async void StartHosting()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, 5000);
                listener.Start();

                // Actualizăm label-ul de stare
                lblTurn.Text = "Host - Aștept jucătorul...";

                client = await listener.AcceptTcpClientAsync();
                stream = client.GetStream();

                // Setăm textul inițial când începe jocul
                myColor = PieceColor.White;
                lblTurn.Text = "Jocul a început! Tura: Alb (Tura Ta)";

                ListenForMoves();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la pornire Host: " + ex.Message);
                Close();
            }
        }

        private async void ConnectToHost()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(IPAddress.Parse(ipAddress), 5000);
                stream = client.GetStream();

                myColor = PieceColor.Black;
                lblTurn.Text = "Conectat! Tura: Alb (Adversar)";

                ListenForMoves();
            }
            catch
            {
                MessageBox.Show("Nu s-a putut conecta la Host.");
                Close();
            }
        }

        private async void ListenForMoves()
        {
            byte[] buffer = new byte[256];
            while (true)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        this.BeginInvoke(new MethodInvoker(OnConnectionLost));
                        break;
                    }

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] parts = msg.Split(':');

                    if (parts.Length == 2)
                    {
                        Position from = ParsePosition(parts[0]);
                        Position to = ParsePosition(parts[1]);

                        this.Invoke(new UpdateBoardDelegate(ApplyRemoteMoveSafe), new object[] { from, to });
                    }
                }
                catch (Exception)
                {
                    this.BeginInvoke(new MethodInvoker(OnConnectionLost));
                    break;
                }
            }
        }

        private void ApplyRemoteMoveSafe(Position from, Position to)
        {
            gameManager.RemoteMove(from, to);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (stream != null) stream.Close();
            if (client != null) client.Close();
            if (listener != null) listener.Stop();
        }

        private Position ParsePosition(string s)
        {
            try
            {
                string[] parts = s.Split(',');
                return new Position(int.Parse(parts[0]), int.Parse(parts[1]));
            }
            catch
            {
                return new Position(-1, -1);
            }
        }

        private void SendMove(Position from, Position to)
        {
            try
            {
                if (stream == null || !stream.CanWrite) return;
                string msg = from.Row + "," + from.Column + ":" + to.Row + "," + to.Column;
                byte[] data = Encoding.UTF8.GetBytes(msg);
                stream.Write(data, 0, data.Length);
            }
            catch
            {
            }
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
        }

        private void BoardPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameManager.CurrentTurn != myColor || gameManager.IsGameOver) return;

            int cell = GetCellSize();
            Position pos = new Position(e.Y / cell, e.X / cell);
            if (!pos.IsValid()) return;

            Piece clickedPiece = gameManager.Board.GetPiece(pos);

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

            if (selectedRow == pos.Row && selectedCol == pos.Column)
            {
                selectedRow = -1; selectedCol = -1;
                boardPanel.Invalidate();
                return;
            }

            Position from = new Position(selectedRow, selectedCol);

            if (gameManager.TryMove(from, pos))
            {
                SendMove(from, pos);
                selectedRow = -1; selectedCol = -1;
            }
            else
            {
                if (clickedPiece != null && clickedPiece.Color == myColor)
                {
                    selectedRow = pos.Row; selectedCol = pos.Column;
                    boardPanel.Invalidate();
                }
            }
        }
        private void NetworkGameForm_Load(object sender, EventArgs e) { }
    }
}