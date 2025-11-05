using System;
using System.Windows.Forms;

namespace ChessWinForms
{
    public partial class GameOverForm : Form
    {
        public GameOverForm(string message)
        {
            InitializeComponent();

            Label lbl = new Label
            {
                Text = message,
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 16),
                Location = new System.Drawing.Point(50, 20)
            };

            Button btnQuit = new Button
            {
                Text = "Quit",
                Location = new System.Drawing.Point(80, 60),
                Width = 80
            };
            btnQuit.Click += (s, e) => Application.Exit();

            Controls.Add(lbl);
            Controls.Add(btnQuit);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(250, 150);
        }

        private void GameOverForm_Load(object sender, EventArgs e)
        {

        }
    }
}