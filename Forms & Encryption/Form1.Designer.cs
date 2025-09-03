namespace OffCrypt
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            LoyalTabControl loyalTabControl1;
            tabPage2 = new TabPage();
            loyalSubTabBar1 = new LoyalSubTabBar();
            subTabPage1 = new SubTabPage();
            modernGroupBox7 = new ServerManager.ModernGroupBox();
            label1 = new Label();
            WeekX255Chck = new ModernCheckBox();
            MonthX255Chck = new ModernCheckBox();
            DayX255Chck = new ModernCheckBox();
            HourX255Chck = new ModernCheckBox();
            DispX255txtbox = new ModernTextBox();
            ToggleDispX255 = new ModernToggleSwitch();
            modernGroupBox10 = new ServerManager.ModernGroupBox();
            Loadinibtn = new CryptButtonC();
            Saveinibtn = new CryptButtonC();
            RemoveDataButton = new CryptButtonC();
            modernGroupBox8 = new ServerManager.ModernGroupBox();
            modernGroupBox9 = new ServerManager.ModernGroupBox();
            label5 = new Label();
            ECDHCombobx = new ModernComboBox();
            label2 = new Label();
            WeekPassChck = new ModernCheckBox();
            MonthPassChck = new ModernCheckBox();
            DayPassCheck = new ModernCheckBox();
            HourPassChck = new ModernCheckBox();
            DispPassTxtbox = new ModernTextBox();
            ToggleDispPass = new ModernToggleSwitch();
            subTabPage2 = new SubTabPage();
            modernGroupBox11 = new ServerManager.ModernGroupBox();
            Copyidentbtn = new CryptButtonC();
            CreateNewIDbtn = new CryptButtonC();
            CurntIdntTxt = new ModernTextBox();
            tabPage1 = new TabPage();
            cryptButtonc3 = new CryptButtonC();
            PasswordCombobx = new ModernComboBox();
            modernGroupBox3 = new ServerManager.ModernGroupBox();
            CopyDecryptedtxt = new CryptButtonC();
            DecryptE = new CryptButtonC();
            label4 = new Label();
            Decryptpasstxt = new ModernTextBox();
            label3 = new Label();
            Decryptmsg = new ModernTextBox();
            modernGroupBox2 = new ServerManager.ModernGroupBox();
            ImportE = new CryptButtonC();
            Createcryptedfile1 = new CryptButtonC();
            modernButton6 = new CryptButtonC();
            Encryptbutton1 = new CryptButtonC();
            Writemsgbox = new ModernTextBox();
            modernGroupBox1 = new ServerManager.ModernGroupBox();
            Setpasswordbutton = new CryptButtonC();
            label6 = new Label();
            Setpasswordtxt = new ModernTextBox();
            tabPage3 = new TabPage();
            cryptButtonc2 = new CryptButtonC();
            modernGroupBox6 = new ServerManager.ModernGroupBox();
            RsCopydecryptedmsg = new CryptButtonC();
            RSDecryptmsgbox = new ModernTextBox();
            RSDecrypt = new CryptButtonC();
            RSAMethodComboBox = new ModernComboBox();
            modernGroupBox4 = new ServerManager.ModernGroupBox();
            RecpientBtn = new CryptButtonC();
            cryptButtonc1 = new CryptButtonC();
            RSPublickeymsgbox = new ModernTextBox();
            RSGenerate = new CryptButtonC();
            modernGroupBox5 = new ServerManager.ModernGroupBox();
            RSimport = new CryptButtonC();
            RSshowmessage = new CryptButtonC();
            RsCreatefile = new CryptButtonC();
            RSWritemsgbox = new ModernTextBox();
            RsEncrypt = new CryptButtonC();
            subTabPageInfoBindingSource = new BindingSource(components);
            offCryptTheme1 = new OffCryptTheme();
            loyalTabControl1 = new LoyalTabControl();
            loyalTabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            subTabPage1.SuspendLayout();
            modernGroupBox7.SuspendLayout();
            modernGroupBox10.SuspendLayout();
            modernGroupBox8.SuspendLayout();
            modernGroupBox9.SuspendLayout();
            subTabPage2.SuspendLayout();
            modernGroupBox11.SuspendLayout();
            tabPage1.SuspendLayout();
            modernGroupBox3.SuspendLayout();
            modernGroupBox2.SuspendLayout();
            modernGroupBox1.SuspendLayout();
            tabPage3.SuspendLayout();
            modernGroupBox6.SuspendLayout();
            modernGroupBox4.SuspendLayout();
            modernGroupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)subTabPageInfoBindingSource).BeginInit();
            SuspendLayout();
            // 
            // loyalTabControl1
            // 
            loyalTabControl1.Appearance = TabAppearance.FlatButtons;
            loyalTabControl1.Controls.Add(tabPage2);
            loyalTabControl1.Controls.Add(tabPage1);
            loyalTabControl1.Controls.Add(tabPage3);
            loyalTabControl1.DataBindings.Add(new Binding("Tag", subTabPageInfoBindingSource, "", true));
            loyalTabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            loyalTabControl1.ForeColor = Color.White;
            loyalTabControl1.ItemSize = new Size(120, 35);
            loyalTabControl1.Location = new Point(6, 43);
            loyalTabControl1.Margin = new Padding(4, 3, 4, 3);
            loyalTabControl1.Name = "loyalTabControl1";
            loyalTabControl1.SelectedIndex = 0;
            loyalTabControl1.Size = new Size(418, 960);
            loyalTabControl1.SizeMode = TabSizeMode.Fixed;
            loyalTabControl1.TabHighlightColor = Color.DarkGoldenrod;
            loyalTabControl1.TabIndex = 3;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.FromArgb(40, 40, 40);
            tabPage2.Controls.Add(loyalSubTabBar1);
            tabPage2.Location = new Point(4, 39);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(410, 917);
            tabPage2.TabIndex = 3;
            tabPage2.Text = "Settings";
            // 
            // loyalSubTabBar1
            // 
            loyalSubTabBar1.BackColor = Color.FromArgb(31, 31, 31);
            loyalSubTabBar1.Font = new Font("Segoe UI", 9F);
            loyalSubTabBar1.ItemSelectedColor = Color.DarkGoldenrod;
            loyalSubTabBar1.Location = new Point(5, 6);
            loyalSubTabBar1.Name = "loyalSubTabBar1";
            loyalSubTabBar1.Pages.Add(subTabPage1);
            loyalSubTabBar1.Pages.Add(subTabPage2);
            loyalSubTabBar1.SelectedIndex = 0;
            loyalSubTabBar1.Size = new Size(400, 897);
            loyalSubTabBar1.TabIndex = 5;
            loyalSubTabBar1.Text = "loyalSubTabBar1";
            loyalSubTabBar1.SelectedIndexChanged += loyalSubTabBar1_SelectedIndexChanged;
            // 
            // subTabPage1
            // 
            subTabPage1.BackColor = Color.FromArgb(28, 28, 28);
            subTabPage1.Controls.Add(modernGroupBox7);
            subTabPage1.Controls.Add(modernGroupBox10);
            subTabPage1.Controls.Add(modernGroupBox8);
            subTabPage1.Dock = DockStyle.Fill;
            subTabPage1.ForeColor = Color.White;
            subTabPage1.Location = new Point(0, 0);
            subTabPage1.Name = "subTabPage1";
            subTabPage1.Size = new Size(400, 859);
            subTabPage1.TabIndex = 0;
            subTabPage1.Text = "Settings";
            // 
            // modernGroupBox7
            // 
            modernGroupBox7.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox7.BorderColor = Color.DarkGreen;
            modernGroupBox7.Controls.Add(label1);
            modernGroupBox7.Controls.Add(WeekX255Chck);
            modernGroupBox7.Controls.Add(MonthX255Chck);
            modernGroupBox7.Controls.Add(DayX255Chck);
            modernGroupBox7.Controls.Add(HourX255Chck);
            modernGroupBox7.Controls.Add(DispX255txtbox);
            modernGroupBox7.Controls.Add(ToggleDispX255);
            modernGroupBox7.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox7.ForeColor = Color.White;
            modernGroupBox7.HeaderColor = Color.DarkRed;
            modernGroupBox7.Location = new Point(0, 3);
            modernGroupBox7.Name = "modernGroupBox7";
            modernGroupBox7.Padding = new Padding(8, 30, 8, 8);
            modernGroupBox7.Size = new Size(400, 216);
            modernGroupBox7.TabIndex = 1;
            modernGroupBox7.TabStop = false;
            modernGroupBox7.Text = "Disappearing messages - Public key mode";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 48);
            label1.Name = "label1";
            label1.Size = new Size(115, 19);
            label1.TabIndex = 6;
            label1.Text = "Dissapear after:";
            // 
            // WeekX255Chck
            // 
            WeekX255Chck.BackColor = Color.Transparent;
            WeekX255Chck.BorderColor = Color.FromArgb(10, 150, 50);
            WeekX255Chck.BorderFocusColor = Color.FromArgb(46, 204, 113);
            WeekX255Chck.BorderHoverColor = Color.FromArgb(39, 174, 96);
            WeekX255Chck.Checked = false;
            WeekX255Chck.CheckedColor = Color.FromArgb(39, 174, 96);
            WeekX255Chck.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            WeekX255Chck.Location = new Point(285, 41);
            WeekX255Chck.Name = "WeekX255Chck";
            WeekX255Chck.Size = new Size(91, 34);
            WeekX255Chck.TabIndex = 5;
            WeekX255Chck.Text = "Week";
            WeekX255Chck.TextColor = Color.White;
            WeekX255Chck.UncheckedColor = Color.FromArgb(45, 45, 48);
            // 
            // MonthX255Chck
            // 
            MonthX255Chck.BackColor = Color.Transparent;
            MonthX255Chck.BorderColor = Color.FromArgb(10, 150, 50);
            MonthX255Chck.BorderFocusColor = Color.FromArgb(46, 204, 113);
            MonthX255Chck.BorderHoverColor = Color.FromArgb(39, 174, 96);
            MonthX255Chck.Checked = false;
            MonthX255Chck.CheckedColor = Color.FromArgb(39, 174, 96);
            MonthX255Chck.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            MonthX255Chck.Location = new Point(285, 81);
            MonthX255Chck.Name = "MonthX255Chck";
            MonthX255Chck.Size = new Size(91, 34);
            MonthX255Chck.TabIndex = 4;
            MonthX255Chck.Text = "Month";
            MonthX255Chck.TextColor = Color.White;
            MonthX255Chck.UncheckedColor = Color.FromArgb(45, 45, 48);
            // 
            // DayX255Chck
            // 
            DayX255Chck.BackColor = Color.Transparent;
            DayX255Chck.BorderColor = Color.FromArgb(10, 150, 50);
            DayX255Chck.BorderFocusColor = Color.FromArgb(46, 204, 113);
            DayX255Chck.BorderHoverColor = Color.FromArgb(39, 174, 96);
            DayX255Chck.Checked = false;
            DayX255Chck.CheckedColor = Color.FromArgb(39, 174, 96);
            DayX255Chck.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            DayX255Chck.Location = new Point(188, 81);
            DayX255Chck.Name = "DayX255Chck";
            DayX255Chck.Size = new Size(70, 34);
            DayX255Chck.TabIndex = 3;
            DayX255Chck.Text = "Day";
            DayX255Chck.TextColor = Color.White;
            DayX255Chck.UncheckedColor = Color.FromArgb(45, 45, 48);
            // 
            // HourX255Chck
            // 
            HourX255Chck.BackColor = Color.Transparent;
            HourX255Chck.BorderColor = Color.FromArgb(10, 150, 50);
            HourX255Chck.BorderFocusColor = Color.FromArgb(46, 204, 113);
            HourX255Chck.BorderHoverColor = Color.FromArgb(39, 174, 96);
            HourX255Chck.Checked = false;
            HourX255Chck.CheckedColor = Color.FromArgb(39, 174, 96);
            HourX255Chck.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            HourX255Chck.Location = new Point(188, 41);
            HourX255Chck.Name = "HourX255Chck";
            HourX255Chck.Size = new Size(70, 34);
            HourX255Chck.TabIndex = 2;
            HourX255Chck.Text = "Hour";
            HourX255Chck.TextColor = Color.White;
            HourX255Chck.UncheckedColor = Color.FromArgb(45, 45, 48);
            // 
            // DispX255txtbox
            // 
            DispX255txtbox.BackColor = Color.FromArgb(45, 45, 48);
            DispX255txtbox.BorderColor = Color.DarkGreen;
            DispX255txtbox.BorderStyle = BorderStyle.FixedSingle;
            DispX255txtbox.FocusColor = Color.FromArgb(10, 150, 50);
            DispX255txtbox.Font = new Font("Segoe UI", 10F);
            DispX255txtbox.ForeColor = Color.White;
            DispX255txtbox.Location = new Point(11, 81);
            DispX255txtbox.Name = "DispX255txtbox";
            DispX255txtbox.Size = new Size(154, 25);
            DispX255txtbox.TabIndex = 1;
            DispX255txtbox.TextAlign = HorizontalAlignment.Center;
            // 
            // ToggleDispX255
            // 
            ToggleDispX255.BorderColor = Color.FromArgb(10, 150, 50);
            ToggleDispX255.BorderWidth = 2;
            ToggleDispX255.ForeColor = Color.FromArgb(40, 40, 40);
            ToggleDispX255.HoverColor = Color.FromArgb(40, 40, 40);
            ToggleDispX255.Location = new Point(57, 123);
            ToggleDispX255.Name = "ToggleDispX255";
            ToggleDispX255.OnColor = Color.FromArgb(10, 150, 50);
            ToggleDispX255.Size = new Size(58, 24);
            ToggleDispX255.TabIndex = 0;
            ToggleDispX255.Text = "modernToggleSwitch1";
            ToggleDispX255.ThumbOffColor = Color.FromArgb(220, 220, 220);
            ToggleDispX255.ThumbOnColor = Color.FromArgb(40, 40, 40);
            // 
            // modernGroupBox10
            // 
            modernGroupBox10.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox10.BorderColor = Color.DarkGreen;
            modernGroupBox10.Controls.Add(Loadinibtn);
            modernGroupBox10.Controls.Add(Saveinibtn);
            modernGroupBox10.Controls.Add(RemoveDataButton);
            modernGroupBox10.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox10.ForeColor = Color.White;
            modernGroupBox10.HeaderColor = Color.DarkRed;
            modernGroupBox10.Location = new Point(1, 549);
            modernGroupBox10.Name = "modernGroupBox10";
            modernGroupBox10.Padding = new Padding(8, 30, 8, 8);
            modernGroupBox10.Size = new Size(402, 307);
            modernGroupBox10.TabIndex = 4;
            modernGroupBox10.TabStop = false;
            modernGroupBox10.Text = "Configs";
            // 
            // Loadinibtn
            // 
            Loadinibtn.BackColor = Color.Transparent;
            Loadinibtn.BorderColor = Color.DarkGreen;
            Loadinibtn.BorderWidth = 3;
            Loadinibtn.CornerRadius = 2;
            Loadinibtn.FlatAppearance.BorderSize = 0;
            Loadinibtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            Loadinibtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            Loadinibtn.FlatStyle = FlatStyle.Flat;
            Loadinibtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Loadinibtn.ForeColor = Color.White;
            Loadinibtn.HoverColor = Color.Green;
            Loadinibtn.Location = new Point(223, 80);
            Loadinibtn.Name = "Loadinibtn";
            Loadinibtn.PrimaryColor = Color.FromArgb(45, 45, 45);
            Loadinibtn.Size = new Size(161, 30);
            Loadinibtn.TabIndex = 6;
            Loadinibtn.Text = "Load";
            Loadinibtn.UseVisualStyleBackColor = false;
            // 
            // Saveinibtn
            // 
            Saveinibtn.BackColor = Color.Transparent;
            Saveinibtn.BorderColor = Color.DarkGreen;
            Saveinibtn.BorderWidth = 3;
            Saveinibtn.CornerRadius = 2;
            Saveinibtn.FlatAppearance.BorderSize = 0;
            Saveinibtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            Saveinibtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            Saveinibtn.FlatStyle = FlatStyle.Flat;
            Saveinibtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Saveinibtn.ForeColor = Color.White;
            Saveinibtn.HoverColor = Color.Green;
            Saveinibtn.Location = new Point(19, 80);
            Saveinibtn.Name = "Saveinibtn";
            Saveinibtn.PrimaryColor = Color.FromArgb(45, 45, 45);
            Saveinibtn.Size = new Size(161, 30);
            Saveinibtn.TabIndex = 5;
            Saveinibtn.Text = "Save";
            Saveinibtn.UseVisualStyleBackColor = false;
            // 
            // RemoveDataButton
            // 
            RemoveDataButton.BackColor = Color.Transparent;
            RemoveDataButton.BorderColor = Color.DarkGreen;
            RemoveDataButton.BorderWidth = 3;
            RemoveDataButton.CornerRadius = 2;
            RemoveDataButton.FlatAppearance.BorderSize = 0;
            RemoveDataButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RemoveDataButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RemoveDataButton.FlatStyle = FlatStyle.Flat;
            RemoveDataButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            RemoveDataButton.ForeColor = Color.White;
            RemoveDataButton.HoverColor = Color.FromArgb(34, 153, 84);
            RemoveDataButton.Location = new Point(115, 189);
            RemoveDataButton.Name = "RemoveDataButton";
            RemoveDataButton.PrimaryColor = Color.DarkRed;
            RemoveDataButton.Size = new Size(161, 40);
            RemoveDataButton.TabIndex = 3;
            RemoveDataButton.Text = "Remove all data";
            RemoveDataButton.UseVisualStyleBackColor = false;
            RemoveDataButton.Click += RemoveDataButton_Click;
            // 
            // modernGroupBox8
            // 
            modernGroupBox8.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox8.BorderColor = Color.DarkGreen;
            modernGroupBox8.Controls.Add(modernGroupBox9);
            modernGroupBox8.Controls.Add(label2);
            modernGroupBox8.Controls.Add(WeekPassChck);
            modernGroupBox8.Controls.Add(MonthPassChck);
            modernGroupBox8.Controls.Add(DayPassCheck);
            modernGroupBox8.Controls.Add(HourPassChck);
            modernGroupBox8.Controls.Add(DispPassTxtbox);
            modernGroupBox8.Controls.Add(ToggleDispPass);
            modernGroupBox8.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox8.ForeColor = Color.White;
            modernGroupBox8.HeaderColor = Color.DarkRed;
            modernGroupBox8.Location = new Point(1, 225);
            modernGroupBox8.Name = "modernGroupBox8";
            modernGroupBox8.Padding = new Padding(8, 30, 8, 8);
            modernGroupBox8.Size = new Size(401, 318);
            modernGroupBox8.TabIndex = 2;
            modernGroupBox8.TabStop = false;
            modernGroupBox8.Text = "Disappearing messages - Password mode";
            // 
            // modernGroupBox9
            // 
            modernGroupBox9.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox9.Controls.Add(label5);
            modernGroupBox9.Controls.Add(ECDHCombobx);
            modernGroupBox9.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox9.ForeColor = Color.White;
            modernGroupBox9.HeaderColor = Color.DarkRed;
            modernGroupBox9.Location = new Point(11, 176);
            modernGroupBox9.Name = "modernGroupBox9";
            modernGroupBox9.Padding = new Padding(8, 30, 8, 8);
            modernGroupBox9.Size = new Size(379, 131);
            modernGroupBox9.TabIndex = 7;
            modernGroupBox9.TabStop = false;
            modernGroupBox9.Text = "Advanced security - Password mode";
            modernGroupBox9.UseCompatibleTextRendering = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(40, 54);
            label5.Name = "label5";
            label5.Size = new Size(93, 19);
            label5.TabIndex = 7;
            label5.Text = "ECDH Mode:";
            // 
            // ECDHCombobx
            // 
            ECDHCombobx.BackColor = Color.FromArgb(45, 45, 48);
            ECDHCombobx.BorderColor = Color.DarkGreen;
            ECDHCombobx.BorderWidth = 2;
            ECDHCombobx.DrawMode = DrawMode.OwnerDrawFixed;
            ECDHCombobx.DropDownStyle = ComboBoxStyle.DropDownList;
            ECDHCombobx.FlatStyle = FlatStyle.Flat;
            ECDHCombobx.FocusColor = Color.FromArgb(10, 150, 50);
            ECDHCombobx.Font = new Font("Segoe UI", 9.5F);
            ECDHCombobx.ForeColor = Color.Silver;
            ECDHCombobx.FormattingEnabled = true;
            ECDHCombobx.ItemHeight = 23;
            ECDHCombobx.Items.AddRange(new object[] { "Static Key (P-256)", "Static Key (P-384)", "Static Key (P-521)", "Ephemeral Keys" });
            ECDHCombobx.Location = new Point(15, 96);
            ECDHCombobx.Name = "ECDHCombobx";
            ECDHCombobx.Size = new Size(180, 29);
            ECDHCombobx.TabIndex = 4;
            ECDHCombobx.Tag = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 48);
            label2.Name = "label2";
            label2.Size = new Size(115, 19);
            label2.TabIndex = 6;
            label2.Text = "Dissapear after:";
            // 
            // WeekPassChck
            // 
            WeekPassChck.BackColor = Color.Transparent;
            WeekPassChck.BorderColor = Color.FromArgb(10, 150, 50);
            WeekPassChck.BorderFocusColor = Color.FromArgb(46, 204, 113);
            WeekPassChck.BorderHoverColor = Color.FromArgb(39, 174, 96);
            WeekPassChck.Checked = false;
            WeekPassChck.CheckedColor = Color.FromArgb(39, 174, 96);
            WeekPassChck.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            WeekPassChck.Location = new Point(300, 42);
            WeekPassChck.Name = "WeekPassChck";
            WeekPassChck.Size = new Size(75, 34);
            WeekPassChck.TabIndex = 5;
            WeekPassChck.Text = "Week";
            WeekPassChck.TextColor = Color.White;
            WeekPassChck.UncheckedColor = Color.FromArgb(45, 45, 48);
            // 
            // MonthPassChck
            // 
            MonthPassChck.BackColor = Color.Transparent;
            MonthPassChck.BorderColor = Color.FromArgb(10, 150, 50);
            MonthPassChck.BorderFocusColor = Color.FromArgb(46, 204, 113);
            MonthPassChck.BorderHoverColor = Color.FromArgb(39, 174, 96);
            MonthPassChck.Checked = false;
            MonthPassChck.CheckedColor = Color.FromArgb(39, 174, 96);
            MonthPassChck.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            MonthPassChck.Location = new Point(300, 82);
            MonthPassChck.Name = "MonthPassChck";
            MonthPassChck.Size = new Size(84, 34);
            MonthPassChck.TabIndex = 4;
            MonthPassChck.Text = "Month";
            MonthPassChck.TextColor = Color.White;
            MonthPassChck.UncheckedColor = Color.FromArgb(45, 45, 48);
            // 
            // DayPassCheck
            // 
            DayPassCheck.BackColor = Color.Transparent;
            DayPassCheck.BorderColor = Color.FromArgb(10, 150, 50);
            DayPassCheck.BorderFocusColor = Color.FromArgb(46, 204, 113);
            DayPassCheck.BorderHoverColor = Color.FromArgb(39, 174, 96);
            DayPassCheck.Checked = false;
            DayPassCheck.CheckedColor = Color.FromArgb(39, 174, 96);
            DayPassCheck.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            DayPassCheck.Location = new Point(203, 82);
            DayPassCheck.Name = "DayPassCheck";
            DayPassCheck.Size = new Size(73, 34);
            DayPassCheck.TabIndex = 3;
            DayPassCheck.Text = "Day";
            DayPassCheck.TextColor = Color.White;
            DayPassCheck.UncheckedColor = Color.FromArgb(45, 45, 48);
            // 
            // HourPassChck
            // 
            HourPassChck.BackColor = Color.Transparent;
            HourPassChck.BorderColor = Color.FromArgb(10, 150, 50);
            HourPassChck.BorderFocusColor = Color.FromArgb(46, 204, 113);
            HourPassChck.BorderHoverColor = Color.FromArgb(39, 174, 96);
            HourPassChck.Checked = false;
            HourPassChck.CheckedColor = Color.FromArgb(39, 174, 96);
            HourPassChck.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            HourPassChck.Location = new Point(203, 42);
            HourPassChck.Name = "HourPassChck";
            HourPassChck.Size = new Size(73, 34);
            HourPassChck.TabIndex = 2;
            HourPassChck.Text = "Hour";
            HourPassChck.TextColor = Color.White;
            HourPassChck.UncheckedColor = Color.FromArgb(45, 45, 48);
            // 
            // DispPassTxtbox
            // 
            DispPassTxtbox.BackColor = Color.FromArgb(40, 40, 40);
            DispPassTxtbox.BorderColor = Color.DarkGreen;
            DispPassTxtbox.BorderStyle = BorderStyle.FixedSingle;
            DispPassTxtbox.FocusColor = Color.FromArgb(10, 150, 50);
            DispPassTxtbox.Font = new Font("Segoe UI", 10F);
            DispPassTxtbox.ForeColor = Color.Silver;
            DispPassTxtbox.Location = new Point(10, 82);
            DispPassTxtbox.Name = "DispPassTxtbox";
            DispPassTxtbox.Size = new Size(154, 25);
            DispPassTxtbox.TabIndex = 1;
            DispPassTxtbox.TextAlign = HorizontalAlignment.Center;
            // 
            // ToggleDispPass
            // 
            ToggleDispPass.BorderColor = Color.FromArgb(10, 150, 50);
            ToggleDispPass.BorderWidth = 2;
            ToggleDispPass.ForeColor = Color.FromArgb(40, 40, 40);
            ToggleDispPass.HoverColor = Color.FromArgb(40, 40, 40);
            ToggleDispPass.Location = new Point(56, 126);
            ToggleDispPass.Name = "ToggleDispPass";
            ToggleDispPass.OnColor = Color.FromArgb(10, 150, 50);
            ToggleDispPass.Size = new Size(58, 24);
            ToggleDispPass.TabIndex = 0;
            ToggleDispPass.Text = "modernToggleSwitch2";
            ToggleDispPass.ThumbOffColor = Color.FromArgb(220, 220, 220);
            ToggleDispPass.ThumbOnColor = Color.FromArgb(40, 40, 40);
            // 
            // subTabPage2
            // 
            subTabPage2.BackColor = Color.FromArgb(40, 40, 40);
            subTabPage2.Controls.Add(modernGroupBox11);
            subTabPage2.Dock = DockStyle.Fill;
            subTabPage2.ForeColor = Color.White;
            subTabPage2.Location = new Point(0, 0);
            subTabPage2.Name = "subTabPage2";
            subTabPage2.Size = new Size(0, 182);
            subTabPage2.TabIndex = 1;
            subTabPage2.Text = "IdentityManager";
            // 
            // modernGroupBox11
            // 
            modernGroupBox11.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox11.Controls.Add(Copyidentbtn);
            modernGroupBox11.Controls.Add(CreateNewIDbtn);
            modernGroupBox11.Controls.Add(CurntIdntTxt);
            modernGroupBox11.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox11.ForeColor = Color.White;
            modernGroupBox11.HeaderColor = Color.DarkRed;
            modernGroupBox11.Location = new Point(3, 3);
            modernGroupBox11.Name = "modernGroupBox11";
            modernGroupBox11.Padding = new Padding(8, 30, 8, 8);
            modernGroupBox11.Size = new Size(395, 340);
            modernGroupBox11.TabIndex = 4;
            modernGroupBox11.TabStop = false;
            modernGroupBox11.Text = "Current identity";
            // 
            // Copyidentbtn
            // 
            Copyidentbtn.BackColor = Color.Transparent;
            Copyidentbtn.BorderColor = Color.FromArgb(34, 153, 84);
            Copyidentbtn.BorderWidth = 3;
            Copyidentbtn.CornerRadius = 2;
            Copyidentbtn.FlatAppearance.BorderSize = 0;
            Copyidentbtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            Copyidentbtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            Copyidentbtn.FlatStyle = FlatStyle.Flat;
            Copyidentbtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Copyidentbtn.ForeColor = Color.White;
            Copyidentbtn.HoverColor = Color.Green;
            Copyidentbtn.Location = new Point(222, 250);
            Copyidentbtn.Name = "Copyidentbtn";
            Copyidentbtn.PrimaryColor = Color.FromArgb(45, 45, 45);
            Copyidentbtn.Size = new Size(161, 30);
            Copyidentbtn.TabIndex = 10;
            Copyidentbtn.Text = "Copy";
            Copyidentbtn.UseVisualStyleBackColor = false;
            // 
            // CreateNewIDbtn
            // 
            CreateNewIDbtn.BackColor = Color.Transparent;
            CreateNewIDbtn.BorderColor = Color.FromArgb(34, 153, 84);
            CreateNewIDbtn.BorderWidth = 3;
            CreateNewIDbtn.CornerRadius = 2;
            CreateNewIDbtn.FlatAppearance.BorderSize = 0;
            CreateNewIDbtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            CreateNewIDbtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            CreateNewIDbtn.FlatStyle = FlatStyle.Flat;
            CreateNewIDbtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            CreateNewIDbtn.ForeColor = Color.White;
            CreateNewIDbtn.HoverColor = Color.Green;
            CreateNewIDbtn.Location = new Point(12, 250);
            CreateNewIDbtn.Name = "CreateNewIDbtn";
            CreateNewIDbtn.PrimaryColor = Color.FromArgb(45, 45, 45);
            CreateNewIDbtn.Size = new Size(161, 30);
            CreateNewIDbtn.TabIndex = 9;
            CreateNewIDbtn.Text = "Create new identity";
            CreateNewIDbtn.UseVisualStyleBackColor = false;
            // 
            // CurntIdntTxt
            // 
            CurntIdntTxt.BackColor = Color.FromArgb(45, 45, 48);
            CurntIdntTxt.BorderColor = Color.FromArgb(39, 174, 96);
            CurntIdntTxt.BorderStyle = BorderStyle.FixedSingle;
            CurntIdntTxt.FocusColor = Color.FromArgb(10, 150, 50);
            CurntIdntTxt.Font = new Font("Segoe UI", 10F);
            CurntIdntTxt.ForeColor = Color.Silver;
            CurntIdntTxt.Location = new Point(12, 39);
            CurntIdntTxt.Margin = new Padding(4, 3, 4, 3);
            CurntIdntTxt.Multiline = true;
            CurntIdntTxt.Name = "CurntIdntTxt";
            CurntIdntTxt.ReadOnly = true;
            CurntIdntTxt.ScrollBars = ScrollBars.Horizontal;
            CurntIdntTxt.Size = new Size(371, 186);
            CurntIdntTxt.TabIndex = 8;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.FromArgb(40, 40, 40);
            tabPage1.Controls.Add(cryptButtonc3);
            tabPage1.Controls.Add(PasswordCombobx);
            tabPage1.Controls.Add(modernGroupBox3);
            tabPage1.Controls.Add(modernGroupBox2);
            tabPage1.Controls.Add(modernGroupBox1);
            tabPage1.ForeColor = Color.White;
            tabPage1.Location = new Point(4, 39);
            tabPage1.Margin = new Padding(4, 3, 4, 3);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(4, 3, 4, 3);
            tabPage1.Size = new Size(410, 917);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Password mode";
            // 
            // cryptButtonc3
            // 
            cryptButtonc3.BackColor = Color.Transparent;
            cryptButtonc3.CornerRadius = 2;
            cryptButtonc3.Enabled = false;
            cryptButtonc3.FlatAppearance.BorderSize = 0;
            cryptButtonc3.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cryptButtonc3.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cryptButtonc3.FlatStyle = FlatStyle.Flat;
            cryptButtonc3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            cryptButtonc3.ForeColor = Color.White;
            cryptButtonc3.HoverColor = Color.FromArgb(34, 153, 84);
            cryptButtonc3.Location = new Point(12, 6);
            cryptButtonc3.Name = "cryptButtonc3";
            cryptButtonc3.PrimaryColor = Color.DarkRed;
            cryptButtonc3.Size = new Size(69, 21);
            cryptButtonc3.TabIndex = 13;
            cryptButtonc3.Text = "Mode:";
            cryptButtonc3.UseVisualStyleBackColor = false;
            // 
            // PasswordCombobx
            // 
            PasswordCombobx.BackColor = Color.FromArgb(45, 45, 48);
            PasswordCombobx.BorderColor = Color.DarkGreen;
            PasswordCombobx.DrawMode = DrawMode.OwnerDrawFixed;
            PasswordCombobx.DropDownStyle = ComboBoxStyle.DropDownList;
            PasswordCombobx.FlatStyle = FlatStyle.Flat;
            PasswordCombobx.FocusColor = Color.FromArgb(10, 150, 50);
            PasswordCombobx.Font = new Font("Segoe UI", 9.5F);
            PasswordCombobx.ForeColor = Color.White;
            PasswordCombobx.FormattingEnabled = true;
            PasswordCombobx.ItemHeight = 23;
            PasswordCombobx.Items.AddRange(new object[] { "ECDH + HKDF + AES-256-GCM" });
            PasswordCombobx.Location = new Point(96, 6);
            PasswordCombobx.Name = "PasswordCombobx";
            PasswordCombobx.Size = new Size(282, 29);
            PasswordCombobx.TabIndex = 0;
            // 
            // modernGroupBox3
            // 
            modernGroupBox3.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox3.BorderColor = Color.Green;
            modernGroupBox3.Controls.Add(CopyDecryptedtxt);
            modernGroupBox3.Controls.Add(DecryptE);
            modernGroupBox3.Controls.Add(label4);
            modernGroupBox3.Controls.Add(Decryptpasstxt);
            modernGroupBox3.Controls.Add(label3);
            modernGroupBox3.Controls.Add(Decryptmsg);
            modernGroupBox3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox3.ForeColor = Color.White;
            modernGroupBox3.HeaderColor = Color.DarkRed;
            modernGroupBox3.Location = new Point(3, 527);
            modernGroupBox3.Margin = new Padding(4, 3, 4, 3);
            modernGroupBox3.Name = "modernGroupBox3";
            modernGroupBox3.Padding = new Padding(9, 35, 9, 9);
            modernGroupBox3.Size = new Size(400, 384);
            modernGroupBox3.TabIndex = 2;
            modernGroupBox3.TabStop = false;
            modernGroupBox3.Text = "Decrypt message";
            // 
            // CopyDecryptedtxt
            // 
            CopyDecryptedtxt.BackColor = Color.Transparent;
            CopyDecryptedtxt.BorderColor = Color.DarkGreen;
            CopyDecryptedtxt.BorderWidth = 3;
            CopyDecryptedtxt.CornerRadius = 2;
            CopyDecryptedtxt.Cursor = Cursors.Hand;
            CopyDecryptedtxt.FlatAppearance.BorderSize = 0;
            CopyDecryptedtxt.FlatAppearance.MouseDownBackColor = Color.Transparent;
            CopyDecryptedtxt.FlatAppearance.MouseOverBackColor = Color.Transparent;
            CopyDecryptedtxt.FlatStyle = FlatStyle.Flat;
            CopyDecryptedtxt.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CopyDecryptedtxt.ForeColor = Color.White;
            CopyDecryptedtxt.HoverColor = Color.Green;
            CopyDecryptedtxt.Location = new Point(211, 282);
            CopyDecryptedtxt.Margin = new Padding(4, 3, 4, 3);
            CopyDecryptedtxt.Name = "CopyDecryptedtxt";
            CopyDecryptedtxt.PrimaryColor = Color.FromArgb(45, 45, 45);
            CopyDecryptedtxt.Size = new Size(138, 33);
            CopyDecryptedtxt.TabIndex = 12;
            CopyDecryptedtxt.Text = "Copy message";
            CopyDecryptedtxt.UseVisualStyleBackColor = false;
            // 
            // DecryptE
            // 
            DecryptE.BackColor = Color.Transparent;
            DecryptE.BorderColor = Color.DarkGreen;
            DecryptE.BorderWidth = 3;
            DecryptE.CornerRadius = 2;
            DecryptE.Cursor = Cursors.Hand;
            DecryptE.FlatAppearance.BorderSize = 0;
            DecryptE.FlatAppearance.MouseDownBackColor = Color.Transparent;
            DecryptE.FlatAppearance.MouseOverBackColor = Color.Transparent;
            DecryptE.FlatStyle = FlatStyle.Flat;
            DecryptE.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            DecryptE.ForeColor = Color.White;
            DecryptE.HoverColor = Color.Green;
            DecryptE.Location = new Point(31, 282);
            DecryptE.Margin = new Padding(4, 3, 4, 3);
            DecryptE.Name = "DecryptE";
            DecryptE.PrimaryColor = Color.FromArgb(45, 45, 45);
            DecryptE.Size = new Size(138, 33);
            DecryptE.TabIndex = 11;
            DecryptE.Text = "Decrypt message";
            DecryptE.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(9, 122);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(126, 15);
            label4.TabIndex = 9;
            label4.Text = "Decrypted message";
            // 
            // Decryptpasstxt
            // 
            Decryptpasstxt.BackColor = Color.FromArgb(45, 45, 48);
            Decryptpasstxt.BorderColor = Color.DarkGreen;
            Decryptpasstxt.BorderStyle = BorderStyle.FixedSingle;
            Decryptpasstxt.FocusColor = Color.FromArgb(10, 150, 50);
            Decryptpasstxt.Font = new Font("Segoe UI", 10F);
            Decryptpasstxt.ForeColor = Color.FromArgb(44, 62, 80);
            Decryptpasstxt.Location = new Point(13, 75);
            Decryptpasstxt.Margin = new Padding(4, 3, 4, 3);
            Decryptpasstxt.Multiline = true;
            Decryptpasstxt.Name = "Decryptpasstxt";
            Decryptpasstxt.Size = new Size(362, 30);
            Decryptpasstxt.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(9, 42);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(105, 15);
            label3.TabIndex = 8;
            label3.Text = "Enter password";
            // 
            // Decryptmsg
            // 
            Decryptmsg.BackColor = Color.FromArgb(45, 45, 48);
            Decryptmsg.BorderColor = Color.DarkGreen;
            Decryptmsg.BorderStyle = BorderStyle.FixedSingle;
            Decryptmsg.FocusColor = Color.FromArgb(10, 150, 50);
            Decryptmsg.Font = new Font("Segoe UI", 10F);
            Decryptmsg.ForeColor = Color.FromArgb(44, 62, 80);
            Decryptmsg.Location = new Point(13, 159);
            Decryptmsg.Margin = new Padding(4, 3, 4, 3);
            Decryptmsg.Multiline = true;
            Decryptmsg.Name = "Decryptmsg";
            Decryptmsg.Size = new Size(362, 101);
            Decryptmsg.TabIndex = 0;
            // 
            // modernGroupBox2
            // 
            modernGroupBox2.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox2.BorderColor = Color.DarkGreen;
            modernGroupBox2.Controls.Add(ImportE);
            modernGroupBox2.Controls.Add(Createcryptedfile1);
            modernGroupBox2.Controls.Add(modernButton6);
            modernGroupBox2.Controls.Add(Encryptbutton1);
            modernGroupBox2.Controls.Add(Writemsgbox);
            modernGroupBox2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox2.ForeColor = Color.White;
            modernGroupBox2.HeaderColor = Color.DarkRed;
            modernGroupBox2.Location = new Point(0, 223);
            modernGroupBox2.Margin = new Padding(4, 3, 4, 3);
            modernGroupBox2.Name = "modernGroupBox2";
            modernGroupBox2.Padding = new Padding(9, 35, 9, 9);
            modernGroupBox2.Size = new Size(403, 296);
            modernGroupBox2.TabIndex = 2;
            modernGroupBox2.TabStop = false;
            modernGroupBox2.Text = "Write message";
            // 
            // ImportE
            // 
            ImportE.BackColor = Color.Transparent;
            ImportE.BorderColor = Color.DarkGreen;
            ImportE.BorderWidth = 3;
            ImportE.CornerRadius = 2;
            ImportE.Cursor = Cursors.Hand;
            ImportE.FlatAppearance.BorderSize = 0;
            ImportE.FlatAppearance.MouseDownBackColor = Color.Transparent;
            ImportE.FlatAppearance.MouseOverBackColor = Color.Transparent;
            ImportE.FlatStyle = FlatStyle.Flat;
            ImportE.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            ImportE.ForeColor = Color.White;
            ImportE.HoverColor = Color.Green;
            ImportE.Location = new Point(211, 236);
            ImportE.Margin = new Padding(4, 3, 4, 3);
            ImportE.Name = "ImportE";
            ImportE.PrimaryColor = Color.FromArgb(45, 45, 45);
            ImportE.Size = new Size(138, 33);
            ImportE.TabIndex = 13;
            ImportE.Text = "Import encrypted file for read";
            ImportE.UseVisualStyleBackColor = false;
            // 
            // Createcryptedfile1
            // 
            Createcryptedfile1.BackColor = Color.Transparent;
            Createcryptedfile1.BorderColor = Color.DarkGreen;
            Createcryptedfile1.BorderWidth = 3;
            Createcryptedfile1.CornerRadius = 2;
            Createcryptedfile1.Cursor = Cursors.Hand;
            Createcryptedfile1.FlatAppearance.BorderSize = 0;
            Createcryptedfile1.FlatAppearance.MouseDownBackColor = Color.Transparent;
            Createcryptedfile1.FlatAppearance.MouseOverBackColor = Color.Transparent;
            Createcryptedfile1.FlatStyle = FlatStyle.Flat;
            Createcryptedfile1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Createcryptedfile1.ForeColor = Color.White;
            Createcryptedfile1.HoverColor = Color.Green;
            Createcryptedfile1.Location = new Point(211, 174);
            Createcryptedfile1.Margin = new Padding(4, 3, 4, 3);
            Createcryptedfile1.Name = "Createcryptedfile1";
            Createcryptedfile1.PrimaryColor = Color.FromArgb(45, 45, 45);
            Createcryptedfile1.Size = new Size(138, 33);
            Createcryptedfile1.TabIndex = 12;
            Createcryptedfile1.Text = "Create encrypted file";
            Createcryptedfile1.UseVisualStyleBackColor = false;
            // 
            // modernButton6
            // 
            modernButton6.BackColor = Color.Transparent;
            modernButton6.BorderColor = Color.DarkGreen;
            modernButton6.BorderWidth = 3;
            modernButton6.CornerRadius = 2;
            modernButton6.Cursor = Cursors.Hand;
            modernButton6.FlatAppearance.BorderSize = 0;
            modernButton6.FlatAppearance.MouseDownBackColor = Color.Transparent;
            modernButton6.FlatAppearance.MouseOverBackColor = Color.Transparent;
            modernButton6.FlatStyle = FlatStyle.Flat;
            modernButton6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            modernButton6.ForeColor = Color.White;
            modernButton6.HoverColor = Color.Green;
            modernButton6.Location = new Point(31, 236);
            modernButton6.Margin = new Padding(4, 3, 4, 3);
            modernButton6.Name = "modernButton6";
            modernButton6.PrimaryColor = Color.FromArgb(45, 45, 45);
            modernButton6.Size = new Size(138, 33);
            modernButton6.TabIndex = 11;
            modernButton6.Text = "Show encrypted message";
            modernButton6.UseVisualStyleBackColor = false;
            // 
            // Encryptbutton1
            // 
            Encryptbutton1.BackColor = Color.Transparent;
            Encryptbutton1.BorderColor = Color.DarkGreen;
            Encryptbutton1.BorderWidth = 3;
            Encryptbutton1.CornerRadius = 2;
            Encryptbutton1.Cursor = Cursors.Hand;
            Encryptbutton1.FlatAppearance.BorderSize = 0;
            Encryptbutton1.FlatAppearance.MouseDownBackColor = Color.Transparent;
            Encryptbutton1.FlatAppearance.MouseOverBackColor = Color.Transparent;
            Encryptbutton1.FlatStyle = FlatStyle.Flat;
            Encryptbutton1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Encryptbutton1.ForeColor = Color.White;
            Encryptbutton1.HoverColor = Color.Green;
            Encryptbutton1.Location = new Point(31, 174);
            Encryptbutton1.Margin = new Padding(4, 3, 4, 3);
            Encryptbutton1.Name = "Encryptbutton1";
            Encryptbutton1.PrimaryColor = Color.FromArgb(45, 45, 45);
            Encryptbutton1.Size = new Size(138, 33);
            Encryptbutton1.TabIndex = 10;
            Encryptbutton1.Text = "Encrypt message";
            Encryptbutton1.UseVisualStyleBackColor = false;
            // 
            // Writemsgbox
            // 
            Writemsgbox.BackColor = Color.FromArgb(45, 45, 48);
            Writemsgbox.BorderColor = Color.DarkGreen;
            Writemsgbox.BorderStyle = BorderStyle.FixedSingle;
            Writemsgbox.FocusColor = Color.FromArgb(10, 150, 50);
            Writemsgbox.Font = new Font("Segoe UI", 10F);
            Writemsgbox.ForeColor = Color.FromArgb(44, 62, 80);
            Writemsgbox.Location = new Point(13, 50);
            Writemsgbox.Margin = new Padding(4, 3, 4, 3);
            Writemsgbox.Multiline = true;
            Writemsgbox.Name = "Writemsgbox";
            Writemsgbox.Size = new Size(362, 101);
            Writemsgbox.TabIndex = 0;
            // 
            // modernGroupBox1
            // 
            modernGroupBox1.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox1.BorderColor = Color.DarkGreen;
            modernGroupBox1.Controls.Add(Setpasswordbutton);
            modernGroupBox1.Controls.Add(label6);
            modernGroupBox1.Controls.Add(Setpasswordtxt);
            modernGroupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox1.ForeColor = Color.White;
            modernGroupBox1.HeaderColor = Color.DarkRed;
            modernGroupBox1.Location = new Point(0, 41);
            modernGroupBox1.Margin = new Padding(4, 3, 4, 3);
            modernGroupBox1.Name = "modernGroupBox1";
            modernGroupBox1.Padding = new Padding(9, 35, 9, 9);
            modernGroupBox1.Size = new Size(403, 175);
            modernGroupBox1.TabIndex = 2;
            modernGroupBox1.TabStop = false;
            modernGroupBox1.Text = "Set password";
            // 
            // Setpasswordbutton
            // 
            Setpasswordbutton.BackColor = Color.Transparent;
            Setpasswordbutton.BorderColor = Color.DarkGreen;
            Setpasswordbutton.BorderWidth = 3;
            Setpasswordbutton.CornerRadius = 2;
            Setpasswordbutton.Cursor = Cursors.Hand;
            Setpasswordbutton.FlatAppearance.BorderSize = 0;
            Setpasswordbutton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            Setpasswordbutton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            Setpasswordbutton.FlatStyle = FlatStyle.Flat;
            Setpasswordbutton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Setpasswordbutton.ForeColor = Color.White;
            Setpasswordbutton.GradientColor = Color.Black;
            Setpasswordbutton.HoverColor = Color.Green;
            Setpasswordbutton.Location = new Point(126, 111);
            Setpasswordbutton.Margin = new Padding(4, 3, 4, 3);
            Setpasswordbutton.Name = "Setpasswordbutton";
            Setpasswordbutton.PrimaryColor = Color.FromArgb(45, 45, 45);
            Setpasswordbutton.Size = new Size(138, 33);
            Setpasswordbutton.TabIndex = 9;
            Setpasswordbutton.Text = "Set password";
            Setpasswordbutton.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(13, 36);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(112, 15);
            label6.TabIndex = 8;
            label6.Text = "Custom password";
            // 
            // Setpasswordtxt
            // 
            Setpasswordtxt.BackColor = Color.FromArgb(45, 45, 48);
            Setpasswordtxt.BorderColor = Color.DarkGreen;
            Setpasswordtxt.BorderStyle = BorderStyle.FixedSingle;
            Setpasswordtxt.FocusColor = Color.FromArgb(10, 150, 50);
            Setpasswordtxt.Font = new Font("Segoe UI", 10F);
            Setpasswordtxt.ForeColor = Color.FromArgb(44, 62, 80);
            Setpasswordtxt.Location = new Point(16, 70);
            Setpasswordtxt.Margin = new Padding(4, 3, 4, 3);
            Setpasswordtxt.Multiline = true;
            Setpasswordtxt.Name = "Setpasswordtxt";
            Setpasswordtxt.Size = new Size(359, 30);
            Setpasswordtxt.TabIndex = 7;
            // 
            // tabPage3
            // 
            tabPage3.BackColor = Color.FromArgb(40, 40, 40);
            tabPage3.Controls.Add(cryptButtonc2);
            tabPage3.Controls.Add(modernGroupBox6);
            tabPage3.Controls.Add(RSAMethodComboBox);
            tabPage3.Controls.Add(modernGroupBox4);
            tabPage3.Controls.Add(modernGroupBox5);
            tabPage3.Location = new Point(4, 39);
            tabPage3.Margin = new Padding(4, 3, 4, 3);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(4, 3, 4, 3);
            tabPage3.Size = new Size(410, 917);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Public key - mode";
            tabPage3.Click += tabPage3_Click;
            // 
            // cryptButtonc2
            // 
            cryptButtonc2.BackColor = Color.Transparent;
            cryptButtonc2.CornerRadius = 1;
            cryptButtonc2.Enabled = false;
            cryptButtonc2.FlatAppearance.BorderSize = 0;
            cryptButtonc2.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cryptButtonc2.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cryptButtonc2.FlatStyle = FlatStyle.Flat;
            cryptButtonc2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            cryptButtonc2.ForeColor = Color.White;
            cryptButtonc2.Location = new Point(8, 8);
            cryptButtonc2.Name = "cryptButtonc2";
            cryptButtonc2.PrimaryColor = Color.DarkRed;
            cryptButtonc2.Size = new Size(69, 21);
            cryptButtonc2.TabIndex = 11;
            cryptButtonc2.Text = "Mode:";
            cryptButtonc2.UseVisualStyleBackColor = false;
            // 
            // modernGroupBox6
            // 
            modernGroupBox6.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox6.BorderColor = Color.DarkGreen;
            modernGroupBox6.Controls.Add(RsCopydecryptedmsg);
            modernGroupBox6.Controls.Add(RSDecryptmsgbox);
            modernGroupBox6.Controls.Add(RSDecrypt);
            modernGroupBox6.CornerRadius = 1;
            modernGroupBox6.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox6.ForeColor = Color.White;
            modernGroupBox6.HeaderColor = Color.DarkRed;
            modernGroupBox6.Location = new Point(1, 620);
            modernGroupBox6.Margin = new Padding(4, 3, 4, 3);
            modernGroupBox6.Name = "modernGroupBox6";
            modernGroupBox6.Padding = new Padding(9, 35, 9, 9);
            modernGroupBox6.Size = new Size(409, 291);
            modernGroupBox6.TabIndex = 7;
            modernGroupBox6.TabStop = false;
            modernGroupBox6.Text = "Decrypt message";
            // 
            // RsCopydecryptedmsg
            // 
            RsCopydecryptedmsg.BackColor = Color.Transparent;
            RsCopydecryptedmsg.BorderColor = Color.DarkGreen;
            RsCopydecryptedmsg.BorderWidth = 3;
            RsCopydecryptedmsg.CornerRadius = 2;
            RsCopydecryptedmsg.Cursor = Cursors.Hand;
            RsCopydecryptedmsg.FlatAppearance.BorderSize = 0;
            RsCopydecryptedmsg.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RsCopydecryptedmsg.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RsCopydecryptedmsg.FlatStyle = FlatStyle.Flat;
            RsCopydecryptedmsg.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RsCopydecryptedmsg.ForeColor = Color.White;
            RsCopydecryptedmsg.HoverColor = Color.Green;
            RsCopydecryptedmsg.Location = new Point(229, 195);
            RsCopydecryptedmsg.Margin = new Padding(4, 3, 4, 3);
            RsCopydecryptedmsg.Name = "RsCopydecryptedmsg";
            RsCopydecryptedmsg.PrimaryColor = Color.FromArgb(45, 45, 45);
            RsCopydecryptedmsg.Size = new Size(138, 33);
            RsCopydecryptedmsg.TabIndex = 7;
            RsCopydecryptedmsg.Text = "Copy message";
            RsCopydecryptedmsg.UseVisualStyleBackColor = false;
            // 
            // RSDecryptmsgbox
            // 
            RSDecryptmsgbox.BackColor = Color.FromArgb(45, 45, 48);
            RSDecryptmsgbox.BorderColor = Color.DarkGreen;
            RSDecryptmsgbox.BorderStyle = BorderStyle.FixedSingle;
            RSDecryptmsgbox.FocusColor = Color.FromArgb(10, 150, 50);
            RSDecryptmsgbox.Font = new Font("Segoe UI", 10F);
            RSDecryptmsgbox.ForeColor = Color.FromArgb(44, 62, 80);
            RSDecryptmsgbox.Location = new Point(14, 67);
            RSDecryptmsgbox.Margin = new Padding(4, 3, 4, 3);
            RSDecryptmsgbox.Multiline = true;
            RSDecryptmsgbox.Name = "RSDecryptmsgbox";
            RSDecryptmsgbox.Size = new Size(382, 101);
            RSDecryptmsgbox.TabIndex = 0;
            // 
            // RSDecrypt
            // 
            RSDecrypt.BackColor = Color.Transparent;
            RSDecrypt.BorderColor = Color.DarkGreen;
            RSDecrypt.BorderWidth = 3;
            RSDecrypt.CornerRadius = 2;
            RSDecrypt.Cursor = Cursors.Hand;
            RSDecrypt.FlatAppearance.BorderSize = 0;
            RSDecrypt.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RSDecrypt.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RSDecrypt.FlatStyle = FlatStyle.Flat;
            RSDecrypt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RSDecrypt.ForeColor = Color.White;
            RSDecrypt.HoverColor = Color.Green;
            RSDecrypt.Location = new Point(50, 195);
            RSDecrypt.Margin = new Padding(4, 3, 4, 3);
            RSDecrypt.Name = "RSDecrypt";
            RSDecrypt.PrimaryColor = Color.FromArgb(45, 45, 45);
            RSDecrypt.Size = new Size(138, 33);
            RSDecrypt.TabIndex = 6;
            RSDecrypt.Text = "Decrypt message";
            RSDecrypt.UseVisualStyleBackColor = false;
            // 
            // RSAMethodComboBox
            // 
            RSAMethodComboBox.BackColor = Color.FromArgb(45, 45, 48);
            RSAMethodComboBox.BorderColor = Color.DarkGreen;
            RSAMethodComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            RSAMethodComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            RSAMethodComboBox.FlatStyle = FlatStyle.Flat;
            RSAMethodComboBox.FocusColor = Color.FromArgb(10, 150, 50);
            RSAMethodComboBox.Font = new Font("Segoe UI", 9.5F);
            RSAMethodComboBox.ForeColor = Color.White;
            RSAMethodComboBox.FormattingEnabled = true;
            RSAMethodComboBox.ItemHeight = 23;
            RSAMethodComboBox.Items.AddRange(new object[] { " X25519 + AES-256-GCM" });
            RSAMethodComboBox.Location = new Point(92, 8);
            RSAMethodComboBox.Name = "RSAMethodComboBox";
            RSAMethodComboBox.Size = new Size(287, 29);
            RSAMethodComboBox.TabIndex = 1;
            // 
            // modernGroupBox4
            // 
            modernGroupBox4.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox4.BorderColor = Color.DarkGreen;
            modernGroupBox4.Controls.Add(RecpientBtn);
            modernGroupBox4.Controls.Add(cryptButtonc1);
            modernGroupBox4.Controls.Add(RSPublickeymsgbox);
            modernGroupBox4.Controls.Add(RSGenerate);
            modernGroupBox4.CornerRadius = 1;
            modernGroupBox4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            modernGroupBox4.ForeColor = Color.White;
            modernGroupBox4.HeaderColor = Color.DarkRed;
            modernGroupBox4.Location = new Point(4, 51);
            modernGroupBox4.Margin = new Padding(4, 3, 4, 3);
            modernGroupBox4.Name = "modernGroupBox4";
            modernGroupBox4.Padding = new Padding(9, 35, 9, 9);
            modernGroupBox4.Size = new Size(410, 221);
            modernGroupBox4.TabIndex = 5;
            modernGroupBox4.TabStop = false;
            modernGroupBox4.Text = "Public key";
            // 
            // RecpientBtn
            // 
            RecpientBtn.BackColor = Color.Transparent;
            RecpientBtn.BorderColor = Color.DarkGreen;
            RecpientBtn.BorderWidth = 3;
            RecpientBtn.CornerRadius = 2;
            RecpientBtn.Cursor = Cursors.Hand;
            RecpientBtn.FlatAppearance.BorderSize = 0;
            RecpientBtn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RecpientBtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RecpientBtn.FlatStyle = FlatStyle.Flat;
            RecpientBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RecpientBtn.ForeColor = Color.White;
            RecpientBtn.HoverColor = Color.Green;
            RecpientBtn.Location = new Point(285, 165);
            RecpientBtn.Margin = new Padding(4, 3, 4, 3);
            RecpientBtn.Name = "RecpientBtn";
            RecpientBtn.PrimaryColor = Color.FromArgb(45, 45, 45);
            RecpientBtn.Size = new Size(110, 33);
            RecpientBtn.TabIndex = 8;
            RecpientBtn.Text = "Recipient's key";
            RecpientBtn.UseVisualStyleBackColor = false;
            // 
            // cryptButtonc1
            // 
            cryptButtonc1.BackColor = Color.Transparent;
            cryptButtonc1.BorderColor = Color.DarkGreen;
            cryptButtonc1.BorderWidth = 3;
            cryptButtonc1.CornerRadius = 2;
            cryptButtonc1.Cursor = Cursors.Hand;
            cryptButtonc1.FlatAppearance.BorderSize = 0;
            cryptButtonc1.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cryptButtonc1.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cryptButtonc1.FlatStyle = FlatStyle.Flat;
            cryptButtonc1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cryptButtonc1.ForeColor = Color.White;
            cryptButtonc1.HoverColor = Color.Green;
            cryptButtonc1.Location = new Point(152, 165);
            cryptButtonc1.Margin = new Padding(4, 3, 4, 3);
            cryptButtonc1.Name = "cryptButtonc1";
            cryptButtonc1.PrimaryColor = Color.FromArgb(45, 45, 45);
            cryptButtonc1.Size = new Size(110, 33);
            cryptButtonc1.TabIndex = 7;
            cryptButtonc1.Text = "Copy key";
            cryptButtonc1.UseVisualStyleBackColor = false;
            // 
            // RSPublickeymsgbox
            // 
            RSPublickeymsgbox.BackColor = Color.FromArgb(45, 45, 48);
            RSPublickeymsgbox.BorderColor = Color.DarkGreen;
            RSPublickeymsgbox.BorderStyle = BorderStyle.FixedSingle;
            RSPublickeymsgbox.FocusColor = Color.FromArgb(10, 150, 50);
            RSPublickeymsgbox.Font = new Font("Segoe UI", 10F);
            RSPublickeymsgbox.ForeColor = Color.FromArgb(44, 62, 80);
            RSPublickeymsgbox.Location = new Point(13, 47);
            RSPublickeymsgbox.Margin = new Padding(4, 3, 4, 3);
            RSPublickeymsgbox.Multiline = true;
            RSPublickeymsgbox.Name = "RSPublickeymsgbox";
            RSPublickeymsgbox.Size = new Size(382, 101);
            RSPublickeymsgbox.TabIndex = 0;
            // 
            // RSGenerate
            // 
            RSGenerate.BackColor = Color.Transparent;
            RSGenerate.BorderColor = Color.DarkGreen;
            RSGenerate.BorderWidth = 3;
            RSGenerate.CornerRadius = 2;
            RSGenerate.Cursor = Cursors.Hand;
            RSGenerate.FlatAppearance.BorderSize = 0;
            RSGenerate.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RSGenerate.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RSGenerate.FlatStyle = FlatStyle.Flat;
            RSGenerate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RSGenerate.ForeColor = Color.White;
            RSGenerate.HoverColor = Color.Green;
            RSGenerate.Location = new Point(13, 165);
            RSGenerate.Margin = new Padding(4, 3, 4, 3);
            RSGenerate.Name = "RSGenerate";
            RSGenerate.PrimaryColor = Color.FromArgb(45, 45, 45);
            RSGenerate.Size = new Size(110, 33);
            RSGenerate.TabIndex = 6;
            RSGenerate.Text = "Generate";
            RSGenerate.UseVisualStyleBackColor = false;
            // 
            // modernGroupBox5
            // 
            modernGroupBox5.BackColor = Color.FromArgb(58, 58, 58);
            modernGroupBox5.BorderColor = Color.DarkGreen;
            modernGroupBox5.Controls.Add(RSimport);
            modernGroupBox5.Controls.Add(RSshowmessage);
            modernGroupBox5.Controls.Add(RsCreatefile);
            modernGroupBox5.Controls.Add(RSWritemsgbox);
            modernGroupBox5.Controls.Add(RsEncrypt);
            modernGroupBox5.CornerRadius = 1;
            modernGroupBox5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            modernGroupBox5.ForeColor = Color.White;
            modernGroupBox5.HeaderColor = Color.DarkRed;
            modernGroupBox5.Location = new Point(3, 278);
            modernGroupBox5.Margin = new Padding(4, 3, 4, 3);
            modernGroupBox5.Name = "modernGroupBox5";
            modernGroupBox5.Padding = new Padding(9, 35, 9, 9);
            modernGroupBox5.Size = new Size(409, 336);
            modernGroupBox5.TabIndex = 6;
            modernGroupBox5.TabStop = false;
            modernGroupBox5.Text = "Write message";
            // 
            // RSimport
            // 
            RSimport.BackColor = Color.Transparent;
            RSimport.BorderColor = Color.DarkGreen;
            RSimport.BorderWidth = 3;
            RSimport.CornerRadius = 2;
            RSimport.Cursor = Cursors.Hand;
            RSimport.FlatAppearance.BorderSize = 0;
            RSimport.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RSimport.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RSimport.FlatStyle = FlatStyle.Flat;
            RSimport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RSimport.ForeColor = Color.White;
            RSimport.HoverColor = Color.Green;
            RSimport.Location = new Point(125, 275);
            RSimport.Margin = new Padding(4, 3, 4, 3);
            RSimport.Name = "RSimport";
            RSimport.PrimaryColor = Color.FromArgb(45, 45, 45);
            RSimport.Size = new Size(138, 33);
            RSimport.TabIndex = 9;
            RSimport.Text = "Import encrypted file";
            RSimport.UseVisualStyleBackColor = false;
            // 
            // RSshowmessage
            // 
            RSshowmessage.BackColor = Color.Transparent;
            RSshowmessage.BorderColor = Color.DarkGreen;
            RSshowmessage.BorderWidth = 3;
            RSshowmessage.CornerRadius = 2;
            RSshowmessage.Cursor = Cursors.Hand;
            RSshowmessage.FlatAppearance.BorderSize = 0;
            RSshowmessage.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RSshowmessage.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RSshowmessage.FlatStyle = FlatStyle.Flat;
            RSshowmessage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RSshowmessage.ForeColor = Color.White;
            RSshowmessage.HoverColor = Color.Green;
            RSshowmessage.Location = new Point(14, 275);
            RSshowmessage.Margin = new Padding(4, 3, 4, 3);
            RSshowmessage.Name = "RSshowmessage";
            RSshowmessage.PrimaryColor = Color.FromArgb(45, 45, 45);
            RSshowmessage.Size = new Size(21, 33);
            RSshowmessage.TabIndex = 8;
            RSshowmessage.Text = "Show encrypted message";
            RSshowmessage.UseVisualStyleBackColor = false;
            RSshowmessage.Visible = false;
            // 
            // RsCreatefile
            // 
            RsCreatefile.BackColor = Color.Transparent;
            RsCreatefile.BorderColor = Color.DarkGreen;
            RsCreatefile.CornerRadius = 2;
            RsCreatefile.Cursor = Cursors.Hand;
            RsCreatefile.FlatAppearance.BorderSize = 0;
            RsCreatefile.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RsCreatefile.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RsCreatefile.FlatStyle = FlatStyle.Flat;
            RsCreatefile.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RsCreatefile.ForeColor = Color.White;
            RsCreatefile.HoverColor = Color.Green;
            RsCreatefile.Location = new Point(229, 203);
            RsCreatefile.Margin = new Padding(4, 3, 4, 3);
            RsCreatefile.Name = "RsCreatefile";
            RsCreatefile.PrimaryColor = Color.FromArgb(45, 45, 45);
            RsCreatefile.Size = new Size(138, 33);
            RsCreatefile.TabIndex = 7;
            RsCreatefile.Text = "Create encrypted file";
            RsCreatefile.UseVisualStyleBackColor = false;
            // 
            // RSWritemsgbox
            // 
            RSWritemsgbox.BackColor = Color.FromArgb(45, 45, 48);
            RSWritemsgbox.BorderColor = Color.DarkGreen;
            RSWritemsgbox.BorderStyle = BorderStyle.FixedSingle;
            RSWritemsgbox.FocusColor = Color.FromArgb(10, 150, 50);
            RSWritemsgbox.Font = new Font("Segoe UI", 10F);
            RSWritemsgbox.ForeColor = Color.FromArgb(44, 62, 80);
            RSWritemsgbox.Location = new Point(14, 48);
            RSWritemsgbox.Margin = new Padding(4, 3, 4, 3);
            RSWritemsgbox.Multiline = true;
            RSWritemsgbox.Name = "RSWritemsgbox";
            RSWritemsgbox.Size = new Size(382, 140);
            RSWritemsgbox.TabIndex = 0;
            // 
            // RsEncrypt
            // 
            RsEncrypt.BackColor = Color.Transparent;
            RsEncrypt.BorderColor = Color.DarkGreen;
            RsEncrypt.BorderWidth = 3;
            RsEncrypt.CornerRadius = 2;
            RsEncrypt.Cursor = Cursors.Hand;
            RsEncrypt.FlatAppearance.BorderSize = 0;
            RsEncrypt.FlatAppearance.MouseDownBackColor = Color.Transparent;
            RsEncrypt.FlatAppearance.MouseOverBackColor = Color.Transparent;
            RsEncrypt.FlatStyle = FlatStyle.Flat;
            RsEncrypt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RsEncrypt.ForeColor = Color.White;
            RsEncrypt.HoverColor = Color.Green;
            RsEncrypt.Location = new Point(34, 203);
            RsEncrypt.Margin = new Padding(4, 3, 4, 3);
            RsEncrypt.Name = "RsEncrypt";
            RsEncrypt.PrimaryColor = Color.FromArgb(45, 45, 45);
            RsEncrypt.Size = new Size(138, 33);
            RsEncrypt.TabIndex = 6;
            RsEncrypt.Text = "Encrypt message";
            RsEncrypt.UseVisualStyleBackColor = false;
            // 
            // offCryptTheme1
            // 
            offCryptTheme1.AccentColor = Color.FromArgb(39, 174, 96);
            offCryptTheme1.FormTitle = "OffCrypt";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(50, 50, 50);
            ClientSize = new Size(428, 1007);
            Controls.Add(loyalTabControl1);
            DoubleBuffered = true;
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            HelpButton = true;
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(428, 1007);
            MinimumSize = new Size(428, 1007);
            Name = "Form1";
            Padding = new Padding(2);
            Text = "OffCrypt - Desktop";
            loyalTabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            subTabPage1.ResumeLayout(false);
            modernGroupBox7.ResumeLayout(false);
            modernGroupBox7.PerformLayout();
            modernGroupBox10.ResumeLayout(false);
            modernGroupBox8.ResumeLayout(false);
            modernGroupBox8.PerformLayout();
            modernGroupBox9.ResumeLayout(false);
            modernGroupBox9.PerformLayout();
            subTabPage2.ResumeLayout(false);
            modernGroupBox11.ResumeLayout(false);
            modernGroupBox11.PerformLayout();
            tabPage1.ResumeLayout(false);
            modernGroupBox3.ResumeLayout(false);
            modernGroupBox3.PerformLayout();
            modernGroupBox2.ResumeLayout(false);
            modernGroupBox2.PerformLayout();
            modernGroupBox1.ResumeLayout(false);
            modernGroupBox1.PerformLayout();
            tabPage3.ResumeLayout(false);
            modernGroupBox6.ResumeLayout(false);
            modernGroupBox6.PerformLayout();
            modernGroupBox4.ResumeLayout(false);
            modernGroupBox4.PerformLayout();
            modernGroupBox5.ResumeLayout(false);
            modernGroupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)subTabPageInfoBindingSource).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private OffCryptTheme offCryptTheme1;
        private System.Windows.Forms.TabPage tabPage1;
        private ModernTextBox Writemsgbox; private System.Windows.Forms.Label label3;
        private ModernTextBox Decryptpasstxt;
        private ModernTextBox Decryptmsg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private ModernTextBox Setpasswordtxt;
        private System.Windows.Forms.TabPage tabPage3;
        private ModernTextBox RSPublickeymsgbox;
        private ModernTextBox RSDecryptmsgbox;
        private ServerManager.ModernGroupBox modernGroupBox3;
        private ServerManager.ModernGroupBox modernGroupBox2;
        private ServerManager.ModernGroupBox modernGroupBox1;
        private CryptButtonC Setpasswordbutton;
        private CryptButtonC Encryptbutton1;
        private CryptButtonC modernButton6;
        private CryptButtonC Createcryptedfile1;
        private CryptButtonC ImportE;
        private CryptButtonC DecryptE;
        private CryptButtonC CopyDecryptedtxt;
        private ServerManager.ModernGroupBox modernGroupBox4;
        private CryptButtonC RSGenerate;
        private ServerManager.ModernGroupBox modernGroupBox5;
        private CryptButtonC RsCreatefile;
        private ModernTextBox RSWritemsgbox;
        private CryptButtonC RsEncrypt;
        private CryptButtonC RSimport;
        private CryptButtonC RSshowmessage;
        private ServerManager.ModernGroupBox modernGroupBox6;
        private CryptButtonC RsCopydecryptedmsg;
        private CryptButtonC RSDecrypt;
        private CryptButtonC cryptButtonc1;
        private BindingSource subTabPageInfoBindingSource;
        private CryptButtonC RecpientBtn;
        private TabPage tabPage2;
        internal ModernToggleSwitch ToggleDispX255;
        internal ModernCheckBox HourX255Chck;
        internal ModernTextBox DispX255txtbox;
        private ServerManager.ModernGroupBox modernGroupBox8;
        private Label label2;
        internal ModernCheckBox WeekPassChck;
        internal ModernCheckBox MonthPassChck;
        internal ModernCheckBox DayPassCheck;
        internal ModernCheckBox HourPassChck;
        internal ModernTextBox DispPassTxtbox;
        internal ModernToggleSwitch ToggleDispPass;
        private Label label1;
        internal ModernCheckBox WeekX255Chck;
        internal ModernCheckBox MonthX255Chck;
        internal ModernCheckBox DayX255Chck;
        private CryptButtonC RemoveDataButton;
        private CryptButtonC cryptButtonc2;
        private ModernComboBox RSAMethodComboBox;
        private CryptButtonC cryptButtonc3;
        private ModernComboBox PasswordCombobx;
        private ServerManager.ModernGroupBox modernGroupBox9;
        private ModernCheckBox ECDHchckbox;
        internal ModernComboBox ECDHCombobx;
        private Label label5;
        private ServerManager.ModernGroupBox modernGroupBox10;
        private CryptButtonC Saveinibtn;
        private CryptButtonC Loadinibtn;
        public LoyalTabControl loyalTabControl1;
        private LoyalSubTabBar loyalSubTabBar1;
        private SubTabPage subTabPage2;
        private ServerManager.ModernGroupBox modernGroupBox11;
        private CryptButtonC CreateNewIDbtn;
        private ModernTextBox CurntIdntTxt;
        public ServerManager.ModernGroupBox modernGroupBox7;
        public SubTabPage subTabPage1;
        private CryptButtonC Copyidentbtn;
    }
}

