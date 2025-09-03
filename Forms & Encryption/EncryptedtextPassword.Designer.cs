namespace OffCrypt
{
    partial class EncryptedtextPassword
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            offCryptTheme1 = new OffCryptTheme();
            modernGroupBox1 = new ServerManager.ModernGroupBox();
            Encryptedtxtfrm = new ModernTextBox();
            modernButton1 = new ModernButton();
            modernGroupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // offCryptTheme1
            // 
            offCryptTheme1.AccentColor = Color.FromArgb(39, 174, 96);
            offCryptTheme1.FormTitle = "OffCrypt";
            // 
            // modernGroupBox1
            // 
            modernGroupBox1.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox1.Controls.Add(Encryptedtxtfrm);
            modernGroupBox1.Controls.Add(modernButton1);
            modernGroupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox1.ForeColor = Color.White;
            modernGroupBox1.Location = new Point(6, 47);
            modernGroupBox1.Margin = new Padding(4, 3, 4, 3);
            modernGroupBox1.Name = "modernGroupBox1";
            modernGroupBox1.Padding = new Padding(9, 35, 9, 9);
            modernGroupBox1.Size = new Size(538, 177);
            modernGroupBox1.TabIndex = 1;
            modernGroupBox1.TabStop = false;
            modernGroupBox1.Text = "Encrypted message";
            // 
            // Encryptedtxtfrm
            // 
            Encryptedtxtfrm.BackColor = Color.FromArgb(40, 40, 40);
            Encryptedtxtfrm.BorderColor = Color.Lime;
            Encryptedtxtfrm.BorderStyle = BorderStyle.FixedSingle;
            Encryptedtxtfrm.FocusColor = Color.FromArgb(39, 174, 96);
            Encryptedtxtfrm.Font = new Font("Segoe UI", 10F);
            Encryptedtxtfrm.ForeColor = Color.White;
            Encryptedtxtfrm.Location = new Point(13, 33);
            Encryptedtxtfrm.Margin = new Padding(4, 3, 4, 3);
            Encryptedtxtfrm.Multiline = true;
            Encryptedtxtfrm.Name = "Encryptedtxtfrm";
            Encryptedtxtfrm.Size = new Size(512, 39);
            Encryptedtxtfrm.TabIndex = 1;
            // 
            // modernButton1
            // 
            modernButton1.BackColor = Color.Transparent;
            modernButton1.CornerRadius = 8;
            modernButton1.Cursor = Cursors.Hand;
            modernButton1.FlatAppearance.BorderSize = 0;
            modernButton1.FlatStyle = FlatStyle.Flat;
            modernButton1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernButton1.ForeColor = Color.White;
            modernButton1.HoverColor = Color.FromArgb(46, 204, 113);
            modernButton1.Location = new Point(139, 98);
            modernButton1.Margin = new Padding(4, 3, 4, 3);
            modernButton1.Name = "modernButton1";
            modernButton1.PressedColor = Color.FromArgb(34, 153, 84);
            modernButton1.PrimaryColor = Color.FromArgb(39, 174, 96);
            modernButton1.Size = new Size(214, 46);
            modernButton1.TabIndex = 0;
            modernButton1.Text = "Copy";
            modernButton1.UseVisualStyleBackColor = false;
            modernButton1.Click += modernButton1_Click;
            // 
            // EncryptedtextPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(554, 233);
            Controls.Add(modernGroupBox1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "EncryptedtextPassword";
            Padding = new Padding(2);
            Text = "EncryptedtextPassword";
            modernGroupBox1.ResumeLayout(false);
            modernGroupBox1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private OffCryptTheme offCryptTheme1;
        private ServerManager.ModernGroupBox modernGroupBox1;
        private ModernTextBox Encryptedtxtfrm;
        private ModernButton modernButton1;
    }
}