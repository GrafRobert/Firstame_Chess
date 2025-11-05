using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace ChessWinForms
{
    public class NetworkSetupForm : Form
    {
        private Button btnHostGame;
        private Button btnJoinGame;
        private Button btnConnect;
        private Button btnBack;
        private TextBox txtIpAddress;
        private Label lblTitle;
        private Label lblStatus;

        public NetworkSetupForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Configurare joc în rețea";
            this.ClientSize = new Size(420, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "Alege modul de joc în rețea",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                Location = new Point(60, 20)
            };
            Controls.Add(lblTitle);

            btnHostGame = new Button
            {
                Text = "Host joc",
                Location = new Point(120, 70),
                Size = new Size(180, 40)
            };
            btnHostGame.Click += BtnHostGame_Click;
            Controls.Add(btnHostGame);

            btnJoinGame = new Button
            {
                Text = "Conectează-te la joc",
                Location = new Point(120, 120),
                Size = new Size(180, 40)
            };
            btnJoinGame.Click += BtnJoinGame_Click;
            Controls.Add(btnJoinGame);

            txtIpAddress = new TextBox
            {
                Text = "IP gazdă",
                ForeColor = Color.Gray,
                Location = new Point(120, 170),
                Size = new Size(180, 26),
                Visible = false
            };
            txtIpAddress.Enter += (s, e) =>
            {
                if (txtIpAddress.Text == "IP gazdă")
                {
                    txtIpAddress.Text = "";
                    txtIpAddress.ForeColor = Color.Black;
                }
            };
            txtIpAddress.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtIpAddress.Text))
                {
                    txtIpAddress.Text = "IP gazdă";
                    txtIpAddress.ForeColor = Color.Gray;
                }
            };
            Controls.Add(txtIpAddress);

            btnConnect = new Button
            {
                Text = "Connect",
                Location = new Point(310, 168),
                Size = new Size(70, 28),
                Visible = false
            };
            btnConnect.Click += BtnConnect_Click;
            Controls.Add(btnConnect);

            lblStatus = new Label
            {
                Text = "",
                AutoSize = true,
                ForeColor = Color.Blue,
                Location = new Point(20, 240)
            };
            Controls.Add(lblStatus);

            btnBack = new Button
            {
                Text = "Înapoi",
                Location = new Point(120, 270),
                Size = new Size(180, 28)
            };
            btnBack.Click += BtnBack_Click;
            Controls.Add(btnBack);
        }

        private void BtnHostGame_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Pornești ca Host. Așteaptă conexiuni...";
            // Deschide NetworkGameForm în modul host
            try
            {
                using (var gameForm = new NetworkGameForm(isHost: true, ipAddress: null))
                {
                    this.Hide();
                    gameForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la pornirea host-ului: " + ex.Message, "Eroare");
            }
            finally
            {
                lblStatus.Text = "";
                this.Show();
            }
        }

        private void BtnJoinGame_Click(object sender, EventArgs e)
        {
            // Arată câmpul IP și butonul Connect pentru input explicit
            txtIpAddress.Visible = true;
            btnConnect.Visible = true;
            txtIpAddress.Focus();
            // Dacă textbox are placeholder, selectează tot textul pentru a facilita scrierea
            if (txtIpAddress.Text == "IP gazdă")
            {
                txtIpAddress.SelectAll();
            }
            lblStatus.Text = "Introdu IP-ul gazdei și apasă Connect.";
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            // Validări
            if (!txtIpAddress.Visible)
            {
                MessageBox.Show("Apasă mai întâi 'Conectează-te la joc' pentru a introduce IP-ul.", "Conectare");
                return;
            }

            string ipText = txtIpAddress.Text?.Trim();
            if (string.IsNullOrWhiteSpace(ipText) || ipText == "IP gazdă")
            {
                MessageBox.Show("Introdu IP-ul gazdei!", "Conectare");
                txtIpAddress.Focus();
                return;
            }

            if (!IPAddress.TryParse(ipText, out var parsedIp))
            {
                MessageBox.Show("Format IP invalid. Folosește 127.0.0.1 pentru test local.", "Conectare");
                txtIpAddress.Focus();
                return;
            }

            lblStatus.Text = $"Încerc conectarea la {ipText} ...";
            try
            {
                using (var gameForm = new NetworkGameForm(isHost: false, ipAddress: ipText))
                {
                    this.Hide();
                    gameForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la conectare: " + ex.Message, "Eroare");
            }
            finally
            {
                lblStatus.Text = "";
                this.Show();
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NetworkSetupForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "NetworkSetupForm";
            this.Load += new System.EventHandler(this.NetworkSetupForm_Load);
            this.ResumeLayout(false);

        }

        private void NetworkSetupForm_Load(object sender, EventArgs e)
        {

        }
    }
}
