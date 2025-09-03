namespace OffCrypt
{
    partial class IdentityRegisterForm
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
            RegisterIDBtn = new CryptButtonC();
            modernGroupBox1 = new ServerManager.ModernGroupBox();
            modernCard1 = new ModernCard();
            RegisterNameTxtbx = new ModernTextBox();
            label2 = new Label();
            label1 = new Label();
            modernGroupBox1.SuspendLayout();
            modernCard1.SuspendLayout();
            SuspendLayout();
            // 
            // offCryptTheme1
            // 
            offCryptTheme1.AccentColor = Color.FromArgb(39, 174, 96);
            offCryptTheme1.FormTitle = "OffCrypt";
            // 
            // RegisterIDBtn
            // 
            RegisterIDBtn.BackColor = Color.Transparent;
            RegisterIDBtn.FlatAppearance.BorderSize = 0;
            RegisterIDBtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RegisterIDBtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RegisterIDBtn.FlatStyle = FlatStyle.Flat;
            RegisterIDBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            RegisterIDBtn.ForeColor = Color.White;
            RegisterIDBtn.Location = new Point(112, 121);
            RegisterIDBtn.Name = "RegisterIDBtn";
            RegisterIDBtn.Size = new Size(140, 33);
            RegisterIDBtn.TabIndex = 1;
            RegisterIDBtn.Text = "Register identity";
            RegisterIDBtn.UseVisualStyleBackColor = false;
            // 
            // modernGroupBox1
            // 
            modernGroupBox1.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox1.Controls.Add(modernCard1);
            modernGroupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox1.ForeColor = Color.White;
            modernGroupBox1.Location = new Point(5, 43);
            modernGroupBox1.Name = "modernGroupBox1";
            modernGroupBox1.Padding = new Padding(8, 30, 8, 8);
            modernGroupBox1.Size = new Size(449, 254);
            modernGroupBox1.TabIndex = 2;
            modernGroupBox1.TabStop = false;
            modernGroupBox1.Text = "Register";
            // 
            // modernCard1
            // 
            modernCard1.BackColor = Color.Transparent;
            modernCard1.BorderStyle = BorderStyle.FixedSingle;
            modernCard1.CardColor = Color.Transparent;
            modernCard1.Controls.Add(RegisterNameTxtbx);
            modernCard1.Controls.Add(label2);
            modernCard1.Controls.Add(label1);
            modernCard1.Controls.Add(RegisterIDBtn);
            modernCard1.CornerRadius = 10;
            modernCard1.Location = new Point(32, 42);
            modernCard1.Name = "modernCard1";
            modernCard1.Padding = new Padding(15);
            modernCard1.Size = new Size(374, 185);
            modernCard1.TabIndex = 13;
            // 
            // RegisterNameTxtbx
            // 
            RegisterNameTxtbx.BackColor = Color.FromArgb(40, 40, 40);
            RegisterNameTxtbx.BorderColor = Color.FromArgb(39, 174, 96);
            RegisterNameTxtbx.BorderStyle = BorderStyle.FixedSingle;
            RegisterNameTxtbx.FocusColor = Color.Lime;
            RegisterNameTxtbx.Font = new Font("Segoe UI", 10F);
            RegisterNameTxtbx.ForeColor = Color.Silver;
            RegisterNameTxtbx.Location = new Point(89, 32);
            RegisterNameTxtbx.Margin = new Padding(4, 3, 4, 3);
            RegisterNameTxtbx.Name = "RegisterNameTxtbx";
            RegisterNameTxtbx.PlaceholderText = "Leave empty to use only random ID";
            RegisterNameTxtbx.ReadOnly = true;
            RegisterNameTxtbx.ScrollBars = ScrollBars.Horizontal;
            RegisterNameTxtbx.Size = new Size(217, 25);
            RegisterNameTxtbx.TabIndex = 10;
            RegisterNameTxtbx.Text = "\r\n\r\n\r\n";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(31, 74);
            label2.Name = "label2";
            label2.Size = new Size(301, 26);
            label2.TabIndex = 12;
            label2.Text = "A random 8-character ID  will be generated automatically.\r\n\r\n";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 32);
            label1.Name = "label1";
            label1.Size = new Size(53, 19);
            label1.TabIndex = 11;
            label1.Text = "Name:";
            // 
            // IdentityRegisterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(459, 306);
            Controls.Add(modernGroupBox1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Name = "IdentityRegisterForm";
            Padding = new Padding(2);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "IdentityRegisterForm";
            Load += IdentityRegisterForm_Load;
            modernGroupBox1.ResumeLayout(false);
            modernCard1.ResumeLayout(false);
            modernCard1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private OffCryptTheme offCryptTheme1;
        private CryptButtonC RegisterIDBtn;
        private ServerManager.ModernGroupBox modernGroupBox1;
        private ModernTextBox RegisterNameTxtbx;
        private Label label1;
        private Label label2;
        private ModernCard modernCard1;
    }
}