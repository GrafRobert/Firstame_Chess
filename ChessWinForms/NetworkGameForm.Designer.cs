namespace ChessWinForms
{
    partial class NetworkGameForm
    {
       
        private System.ComponentModel.IContainer components = null;

        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        
        private void InitializeComponent()
        {
            this.SuspendLayout();
             
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 450);
            this.Name = "NetworkGameForm";
            this.Text = "NetworkGameForm";
            this.Load += new System.EventHandler(this.NetworkGameForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}