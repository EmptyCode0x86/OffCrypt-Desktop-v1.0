namespace OffCrypt
{
    partial class RecipientForm
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
            Recptxtbx = new ModernTextBox();
            cancelRecpbtn = new CryptButtonC();
            PasetRecpbtn = new CryptButtonC();
            ImportRecpbtn = new CryptButtonC();
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
            modernGroupBox1.Controls.Add(ImportRecpbtn);
            modernGroupBox1.Controls.Add(Recptxtbx);
            modernGroupBox1.Controls.Add(cancelRecpbtn);
            modernGroupBox1.Controls.Add(PasetRecpbtn);
            modernGroupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox1.ForeColor = Color.White;
            modernGroupBox1.Location = new Point(5, 39);
            modernGroupBox1.Name = "modernGroupBox1";
            modernGroupBox1.Padding = new Padding(8, 30, 8, 8);
            modernGroupBox1.Size = new Size(526, 287);
            modernGroupBox1.TabIndex = 1;
            modernGroupBox1.TabStop = false;
            modernGroupBox1.Text = "Recipient's public key";
            // 
            // Recptxtbx
            // 
            Recptxtbx.BackColor = Color.FromArgb(40, 40, 40);
            Recptxtbx.BorderColor = Color.FromArgb(39, 174, 96);
            Recptxtbx.BorderStyle = BorderStyle.FixedSingle;
            Recptxtbx.FocusColor = Color.FromArgb(39, 174, 96);
            Recptxtbx.Font = new Font("Segoe UI", 10F);
            Recptxtbx.ForeColor = Color.White;
            Recptxtbx.Location = new Point(11, 36);
            Recptxtbx.Multiline = true;
            Recptxtbx.Name = "Recptxtbx";
            Recptxtbx.Size = new Size(504, 155);
            Recptxtbx.TabIndex = 0;
            // 
            // cancelRecpbtn
            // 
            cancelRecpbtn.BackColor = Color.Transparent;
            cancelRecpbtn.CornerRadius = 1;
            cancelRecpbtn.Cursor = Cursors.Hand;
            cancelRecpbtn.DialogResult = DialogResult.Cancel;
            cancelRecpbtn.FlatAppearance.BorderSize = 0;
            cancelRecpbtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cancelRecpbtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cancelRecpbtn.FlatStyle = FlatStyle.Flat;
            cancelRecpbtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cancelRecpbtn.ForeColor = Color.White;
            cancelRecpbtn.Location = new Point(345, 213);
            cancelRecpbtn.Name = "cancelRecpbtn";
            cancelRecpbtn.PrimaryColor = Color.FromArgb(108, 117, 125);
            cancelRecpbtn.Size = new Size(101, 32);
            cancelRecpbtn.TabIndex = 3;
            cancelRecpbtn.Text = "Cancel";
            cancelRecpbtn.UseVisualStyleBackColor = false;
            // 
            // PasetRecpbtn
            // 
            PasetRecpbtn.BackColor = Color.Transparent;
            PasetRecpbtn.CornerRadius = 1;
            PasetRecpbtn.Cursor = Cursors.Hand;
            PasetRecpbtn.DialogResult = DialogResult.OK;
            PasetRecpbtn.FlatAppearance.BorderSize = 0;
            PasetRecpbtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            PasetRecpbtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            PasetRecpbtn.FlatStyle = FlatStyle.Flat;
            PasetRecpbtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            PasetRecpbtn.ForeColor = Color.White;
            PasetRecpbtn.Location = new Point(46, 213);
            PasetRecpbtn.Name = "PasetRecpbtn";
            PasetRecpbtn.Size = new Size(101, 32);
            PasetRecpbtn.TabIndex = 2;
            PasetRecpbtn.Text = "Paste";
            PasetRecpbtn.UseVisualStyleBackColor = false;
            // 
            // ImportRecpbtn
            // 
            ImportRecpbtn.BackColor = Color.Transparent;
            ImportRecpbtn.CornerRadius = 1;
            ImportRecpbtn.Cursor = Cursors.Hand;
            ImportRecpbtn.DialogResult = DialogResult.OK;
            ImportRecpbtn.FlatAppearance.BorderSize = 0;
            ImportRecpbtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            ImportRecpbtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            ImportRecpbtn.FlatStyle = FlatStyle.Flat;
            ImportRecpbtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            ImportRecpbtn.ForeColor = Color.White;
            ImportRecpbtn.Location = new Point(186, 213);
            ImportRecpbtn.Name = "ImportRecpbtn";
            ImportRecpbtn.Size = new Size(120, 32);
            ImportRecpbtn.TabIndex = 4;
            ImportRecpbtn.Text = "Import public key";
            ImportRecpbtn.UseVisualStyleBackColor = false;
            // 
            // RecipientForm
            // 
            AcceptButton = PasetRecpbtn;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelRecpbtn;
            ClientSize = new Size(538, 329);
            Controls.Add(modernGroupBox1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Name = "RecipientForm";
            Padding = new Padding(2);
            Text = "RecipientForm";
            modernGroupBox1.ResumeLayout(false);
            modernGroupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private OffCryptTheme offCryptTheme1;
        private ServerManager.ModernGroupBox modernGroupBox1;
        private ModernTextBox Recptxtbx;
        private CryptButtonC PasetRecpbtn;
        private CryptButtonC cancelRecpbtn;
        private CryptButtonC ImportRecpbtn;
    }
}