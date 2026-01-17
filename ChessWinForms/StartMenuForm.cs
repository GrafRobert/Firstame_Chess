using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessWinForms
{
    public class StartMenuForm : Form
    {
        private Button btnLocalGame;
        private Button btnNetworkGame;
        private Button btnExit;
        private Label lblTitle;

        public StartMenuForm()
        {
            
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Sah - Meniu principal";
            this.ClientSize = new Size(420, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            lblTitle = new Label
            {
                Text = "Sah",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true
            };
            Controls.Add(lblTitle);

            btnLocalGame = new Button
            {
                Text = "Joc local",
                Size = new Size(180, 48)
            };
            btnLocalGame.Click += BtnLocalGame_Click;
            Controls.Add(btnLocalGame);

            btnNetworkGame = new Button
            {
                Text = "Joc în rețea",
                Size = new Size(180, 48)
            };
            btnNetworkGame.Click += BtnNetworkGame_Click;
            Controls.Add(btnNetworkGame);

            btnExit = new Button
            {
                Text = "Ieșire",
                Size = new Size(180, 40)
            };
            btnExit.Click += BtnExit_Click;
            Controls.Add(btnExit);


            PositionControls();
            this.Resize += (s, e) => PositionControls();
        }

        private void PositionControls()
        {
            int centerX = (this.ClientSize.Width) / 2;
            lblTitle.Location = new Point(centerX - 40, 20);

            btnLocalGame.Location = new Point(centerX - 90, 90);
            btnNetworkGame.Location = new Point(centerX - 90, 150);
            btnExit.Location = new Point(centerX - 90, 210);
        }

        private void BtnLocalGame_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                using (var gameForm = new Form1()) 
                {
                    gameForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la deschiderea jocului local: " + ex.Message, "Eroare");
            }
            finally
            {
                
                if (!this.IsDisposed && !this.Disposing)
                {
                    try
                    {
                        this.Show();
                    }
                    catch (ObjectDisposedException)
                    {
                       
                    }
                }
            }
        }

        private void BtnNetworkGame_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                using (var netForm = new NetworkSetupForm())
                {
                    netForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la deschiderea configurării rețea: " + ex.Message, "Eroare");
            }
            finally
            {
                if (!this.IsDisposed && !this.Disposing)
                {
                    try
                    {
                        this.Show();
                    }
                    catch (ObjectDisposedException)
                    {
                      
                    }
                }
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // StartMenuForm
        //    // 
        //    this.ClientSize = new System.Drawing.Size(284, 261);
        //    this.Name = "StartMenuForm";
        //    this.Load += new System.EventHandler(this.StartMenuForm_Load);
        //    this.ResumeLayout(false);

        //}

        private void StartMenuForm_Load(object sender, EventArgs e)
        {

        }

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // StartMenuForm
        //    // 
        //    this.ClientSize = new System.Drawing.Size(284, 261);
        //    this.Name = "StartMenuForm";
        //    this.Load += new System.EventHandler(this.StartMenuForm_Load_1);
        //    this.ResumeLayout(false);

        //}

        private void StartMenuForm_Load_1(object sender, EventArgs e)
        {

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // StartMenuForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "StartMenuForm";
            this.Load += new System.EventHandler(this.StartMenuForm_Load_2);
            this.ResumeLayout(false);

        }

        private void StartMenuForm_Load_2(object sender, EventArgs e)
        {

        }
    }




}
