namespace CriptografiaMusical
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblEntrada = new System.Windows.Forms.Label();
            this.txtEntrada = new System.Windows.Forms.TextBox();
            this.btnCriptografar = new System.Windows.Forms.Button();
            this.btnDescriptografar = new System.Windows.Forms.Button();
            this.lblSaida = new System.Windows.Forms.Label();
            this.txtSaida = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            // lblEntrada
            this.lblEntrada.AutoSize = true;
            this.lblEntrada.Location = new System.Drawing.Point(12, 15);
            this.lblEntrada.Text = "Digite ou cole a mensagem:";

            // txtEntrada
            this.txtEntrada.Location = new System.Drawing.Point(12, 35);
            this.txtEntrada.Size = new System.Drawing.Size(460, 100);
            this.txtEntrada.Multiline = true;
            this.txtEntrada.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            // btnCriptografar
            this.btnCriptografar.Location = new System.Drawing.Point(12, 150);
            this.btnCriptografar.Size = new System.Drawing.Size(220, 35);
            this.btnCriptografar.Text = "Criptografar";
            this.btnCriptografar.Click += new System.EventHandler(this.btnCriptografar_Click);

            // btnDescriptografar
            this.btnDescriptografar.Location = new System.Drawing.Point(252, 150);
            this.btnDescriptografar.Size = new System.Drawing.Size(220, 35);
            this.btnDescriptografar.Text = "Descriptografar";
            this.btnDescriptografar.Click += new System.EventHandler(this.btnDescriptografar_Click);

            // lblSaida
            this.lblSaida.AutoSize = true;
            this.lblSaida.Location = new System.Drawing.Point(12, 205);
            this.lblSaida.Text = "Resultado:";

            // txtSaida
            this.txtSaida.Location = new System.Drawing.Point(12, 225);
            this.txtSaida.Size = new System.Drawing.Size(460, 100);
            this.txtSaida.Multiline = true;
            this.txtSaida.ReadOnly = true;
            this.txtSaida.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSaida.BackColor = System.Drawing.SystemColors.Window;

            // Form1
            this.ClientSize = new System.Drawing.Size(484, 345);
            this.Controls.Add(this.lblEntrada);
            this.Controls.Add(this.txtEntrada);
            this.Controls.Add(this.btnCriptografar);
            this.Controls.Add(this.btnDescriptografar);
            this.Controls.Add(this.lblSaida);
            this.Controls.Add(this.txtSaida);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Criptografia Musical";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblEntrada;
        private System.Windows.Forms.TextBox txtEntrada;
        private System.Windows.Forms.Button btnCriptografar;
        private System.Windows.Forms.Button btnDescriptografar;
        private System.Windows.Forms.Label lblSaida;
        private System.Windows.Forms.TextBox txtSaida;
    }
}