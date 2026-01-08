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

            lblTitle = new Label();
            lblTitle.Text = "Alege modul de joc în rețea";
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            lblTitle.Location = new Point(60, 20);
            Controls.Add(lblTitle);

            btnHostGame = new Button();
            btnHostGame.Text = "Host joc";
            btnHostGame.Location = new Point(120, 70);
            btnHostGame.Size = new Size(180, 40);
            btnHostGame.Click += new EventHandler(BtnHostGame_Click);
            Controls.Add(btnHostGame);

            btnJoinGame = new Button();
            btnJoinGame.Text = "Conectează-te la joc";
            btnJoinGame.Location = new Point(120, 120);
            btnJoinGame.Size = new Size(180, 40);
            btnJoinGame.Click += new EventHandler(BtnJoinGame_Click);
            Controls.Add(btnJoinGame);

            txtIpAddress = new TextBox();
            txtIpAddress.Text = "IP gazdă";
            txtIpAddress.ForeColor = Color.Gray;
            txtIpAddress.Location = new Point(120, 170);
            txtIpAddress.Size = new Size(180, 26);
            txtIpAddress.Visible = false;
            // Fara lambda aici
            txtIpAddress.Enter += new EventHandler(TxtIpAddress_Enter);
            txtIpAddress.Leave += new EventHandler(TxtIpAddress_Leave);
            Controls.Add(txtIpAddress);

            btnConnect = new Button();
            btnConnect.Text = "Connect";
            btnConnect.Location = new Point(310, 168);
            btnConnect.Size = new Size(70, 28);
            btnConnect.Visible = false;
            btnConnect.Click += new EventHandler(BtnConnect_Click);
            Controls.Add(btnConnect);

            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Blue;
            lblStatus.Location = new Point(20, 240);
            Controls.Add(lblStatus);

            btnBack = new Button();
            btnBack.Text = "Înapoi";
            btnBack.Location = new Point(120, 270);
            btnBack.Size = new Size(180, 28);
            btnBack.Click += new EventHandler(BtnBack_Click);
            Controls.Add(btnBack);
        }

        private void TxtIpAddress_Enter(object sender, EventArgs e)
        {
            if (txtIpAddress.Text == "IP gazdă")
            {
                txtIpAddress.Text = "";
                txtIpAddress.ForeColor = Color.Black;
            }
        }

        private void TxtIpAddress_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIpAddress.Text))
            {
                txtIpAddress.Text = "IP gazdă";
                txtIpAddress.ForeColor = Color.Gray;
            }
        }

        private void BtnHostGame_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Pornești ca Host. Așteaptă conexiuni...";
            try
            {
                NetworkGameForm gameForm = new NetworkGameForm(true, null);
                this.Hide();
                DialogResult result = gameForm.ShowDialog();
                gameForm.Dispose();

                if (result == DialogResult.OK)
                {
                    this.Close();
                }
                else
                {
                    this.Show();
                    lblStatus.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message);
                this.Show();
            }
        }

        private void BtnJoinGame_Click(object sender, EventArgs e)
        {
            txtIpAddress.Visible = true;
            btnConnect.Visible = true;
            txtIpAddress.Focus();
            if (txtIpAddress.Text == "IP gazdă")
            {
                txtIpAddress.SelectAll();
            }
            lblStatus.Text = "Introdu IP-ul gazdei și apasă Connect.";
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            string ipText = txtIpAddress.Text;
            if (ipText != null) ipText = ipText.Trim();

            if (string.IsNullOrWhiteSpace(ipText) || ipText == "IP gazdă") return;

            lblStatus.Text = "Încerc conectarea la " + ipText + " ...";
            try
            {
                NetworkGameForm gameForm = new NetworkGameForm(false, ipText);
                this.Hide();
                DialogResult result = gameForm.ShowDialog();
                gameForm.Dispose();

                if (result == DialogResult.OK)
                {
                    this.Close();
                }
                else
                {
                    this.Show();
                    lblStatus.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message);
                this.Show();
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NetworkSetupForm_Load(object sender, EventArgs e) { }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NetworkSetupForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "NetworkSetupForm";
            this.Load += new System.EventHandler(this.NetworkSetupForm_Load_1);
            this.ResumeLayout(false);

        }

        private void NetworkSetupForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}