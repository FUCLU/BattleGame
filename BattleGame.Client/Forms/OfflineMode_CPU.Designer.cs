namespace BattleGame.Client.Forms
{
    partial class OfflineMode_CPU
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
            panel2 = new Panel();
            lblYouTitle = new Label();
            lblCharacterCaption = new Label();
            lblNameCharPlayer = new Label();
            btnSelCharPlayer = new Button();
            panel1 = new Panel();
            lblBotTitle = new Label();
            lblPlayer2CharacterCaption = new Label();
            lblNameCharPlayer2 = new Label();
            btnSelCharPlayer2 = new Button();
            panelMap = new Panel();
            lblMapTitle = new Label();
            lblMapCaption = new Label();
            comboBoxMap = new ComboBox();
            pictureBoxMap = new PictureBox();
            button2 = new Button();
            btnPlay = new Button();
            label1 = new Label();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            panelMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(24, 36, 68);
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(lblYouTitle);
            panel2.Controls.Add(lblCharacterCaption);
            panel2.Controls.Add(lblNameCharPlayer);
            panel2.Controls.Add(btnSelCharPlayer);
            panel2.Location = new Point(73, 112);
            panel2.Name = "panel2";
            panel2.Size = new Size(390, 250);
            panel2.TabIndex = 18;
            // 
            // lblYouTitle
            // 
            lblYouTitle.BackColor = Color.Transparent;
            lblYouTitle.Font = new Font("Book Antiqua", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblYouTitle.ForeColor = Color.FromArgb(255, 235, 156);
            lblYouTitle.Location = new Point(0, 16);
            lblYouTitle.Name = "lblYouTitle";
            lblYouTitle.Size = new Size(390, 46);
            lblYouTitle.TabIndex = 25;
            lblYouTitle.Text = "PLAYER 1";
            lblYouTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblCharacterCaption
            // 
            lblCharacterCaption.BackColor = Color.Transparent;
            lblCharacterCaption.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCharacterCaption.ForeColor = Color.FromArgb(220, 235, 255);
            lblCharacterCaption.Location = new Point(42, 88);
            lblCharacterCaption.Name = "lblCharacterCaption";
            lblCharacterCaption.Size = new Size(126, 30);
            lblCharacterCaption.TabIndex = 24;
            lblCharacterCaption.Text = "Character:";
            lblCharacterCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblNameCharPlayer
            // 
            lblNameCharPlayer.BackColor = Color.Transparent;
            lblNameCharPlayer.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNameCharPlayer.ForeColor = Color.FromArgb(255, 235, 156);
            lblNameCharPlayer.Location = new Point(166, 88);
            lblNameCharPlayer.Name = "lblNameCharPlayer";
            lblNameCharPlayer.Size = new Size(180, 30);
            lblNameCharPlayer.TabIndex = 20;
            lblNameCharPlayer.Text = "Lord";
            lblNameCharPlayer.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnSelCharPlayer
            // 
            btnSelCharPlayer.BackColor = Color.FromArgb(44, 74, 110);
            btnSelCharPlayer.FlatAppearance.BorderColor = Color.PaleTurquoise;
            btnSelCharPlayer.FlatAppearance.BorderSize = 2;
            btnSelCharPlayer.FlatStyle = FlatStyle.Flat;
            btnSelCharPlayer.Font = new Font("Courier New", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSelCharPlayer.ForeColor = Color.FromArgb(255, 235, 156);
            btnSelCharPlayer.Location = new Point(42, 152);
            btnSelCharPlayer.Name = "btnSelCharPlayer";
            btnSelCharPlayer.Size = new Size(306, 52);
            btnSelCharPlayer.TabIndex = 21;
            btnSelCharPlayer.Text = "SELECT CHARACTER";
            btnSelCharPlayer.UseVisualStyleBackColor = false;
            btnSelCharPlayer.Click += btnSelCharPlayer_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(24, 36, 68);
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(lblBotTitle);
            panel1.Controls.Add(lblPlayer2CharacterCaption);
            panel1.Controls.Add(lblNameCharPlayer2);
            panel1.Controls.Add(btnSelCharPlayer2);
            panel1.Location = new Point(605, 112);
            panel1.Name = "panel1";
            panel1.Size = new Size(390, 250);
            panel1.TabIndex = 1;
            // 
            // lblBotTitle
            // 
            lblBotTitle.BackColor = Color.Transparent;
            lblBotTitle.Font = new Font("Book Antiqua", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBotTitle.ForeColor = Color.FromArgb(255, 235, 156);
            lblBotTitle.Location = new Point(0, 16);
            lblBotTitle.Name = "lblBotTitle";
            lblBotTitle.Size = new Size(390, 46);
            lblBotTitle.TabIndex = 18;
            lblBotTitle.Text = "PLAYER 2";
            lblBotTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPlayer2CharacterCaption
            // 
            lblPlayer2CharacterCaption.BackColor = Color.Transparent;
            lblPlayer2CharacterCaption.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPlayer2CharacterCaption.ForeColor = Color.FromArgb(220, 235, 255);
            lblPlayer2CharacterCaption.Location = new Point(42, 88);
            lblPlayer2CharacterCaption.Name = "lblPlayer2CharacterCaption";
            lblPlayer2CharacterCaption.Size = new Size(126, 30);
            lblPlayer2CharacterCaption.TabIndex = 27;
            lblPlayer2CharacterCaption.Text = "Character:";
            lblPlayer2CharacterCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblNameCharPlayer2
            // 
            lblNameCharPlayer2.BackColor = Color.Transparent;
            lblNameCharPlayer2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNameCharPlayer2.ForeColor = Color.FromArgb(255, 235, 156);
            lblNameCharPlayer2.Location = new Point(166, 88);
            lblNameCharPlayer2.Name = "lblNameCharPlayer2";
            lblNameCharPlayer2.Size = new Size(180, 30);
            lblNameCharPlayer2.TabIndex = 28;
            lblNameCharPlayer2.Text = "Samurai";
            lblNameCharPlayer2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnSelCharPlayer2
            // 
            btnSelCharPlayer2.BackColor = Color.FromArgb(44, 74, 110);
            btnSelCharPlayer2.FlatAppearance.BorderColor = Color.PaleTurquoise;
            btnSelCharPlayer2.FlatAppearance.BorderSize = 2;
            btnSelCharPlayer2.FlatStyle = FlatStyle.Flat;
            btnSelCharPlayer2.Font = new Font("Courier New", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSelCharPlayer2.ForeColor = Color.FromArgb(255, 235, 156);
            btnSelCharPlayer2.Location = new Point(42, 152);
            btnSelCharPlayer2.Name = "btnSelCharPlayer2";
            btnSelCharPlayer2.Size = new Size(306, 52);
            btnSelCharPlayer2.TabIndex = 29;
            btnSelCharPlayer2.Text = "SELECT CHARACTER";
            btnSelCharPlayer2.UseVisualStyleBackColor = false;
            btnSelCharPlayer2.Click += btnSelCharPlayer2_Click;
            // 
            // panelMap
            // 
            panelMap.BackColor = Color.FromArgb(24, 36, 68);
            panelMap.BorderStyle = BorderStyle.FixedSingle;
            panelMap.Controls.Add(lblMapTitle);
            panelMap.Controls.Add(lblMapCaption);
            panelMap.Controls.Add(comboBoxMap);
            panelMap.Controls.Add(pictureBoxMap);
            panelMap.Location = new Point(74, 399);
            panelMap.Name = "panelMap";
            panelMap.Size = new Size(923, 235);
            panelMap.TabIndex = 31;
            // 
            // lblMapTitle
            // 
            lblMapTitle.BackColor = Color.Transparent;
            lblMapTitle.Font = new Font("Book Antiqua", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMapTitle.ForeColor = Color.FromArgb(255, 235, 156);
            lblMapTitle.Location = new Point(0, 10);
            lblMapTitle.Name = "lblMapTitle";
            lblMapTitle.Size = new Size(923, 34);
            lblMapTitle.TabIndex = 32;
            lblMapTitle.Text = "MAP SELECT";
            lblMapTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblMapCaption
            // 
            lblMapCaption.BackColor = Color.Transparent;
            lblMapCaption.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMapCaption.ForeColor = Color.FromArgb(220, 235, 255);
            lblMapCaption.Location = new Point(82, 103);
            lblMapCaption.Name = "lblMapCaption";
            lblMapCaption.Size = new Size(90, 34);
            lblMapCaption.TabIndex = 33;
            lblMapCaption.Text = "Arena:";
            lblMapCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // comboBoxMap
            // 
            comboBoxMap.BackColor = Color.WhiteSmoke;
            comboBoxMap.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMap.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxMap.ForeColor = Color.Black;
            comboBoxMap.FormattingEnabled = true;
            comboBoxMap.Items.AddRange(new object[] { "Map 1", "Map 2", "Map 3" });
            comboBoxMap.Location = new Point(174, 102);
            comboBoxMap.Name = "comboBoxMap";
            comboBoxMap.Size = new Size(230, 36);
            comboBoxMap.TabIndex = 34;
            comboBoxMap.SelectedIndexChanged += comboBoxMap_SelectedIndexChanged;
            // 
            // pictureBoxMap
            // 
            pictureBoxMap.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxMap.Location = new Point(589, 47);
            pictureBoxMap.Name = "pictureBoxMap";
            pictureBoxMap.Size = new Size(288, 145);
            pictureBoxMap.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxMap.TabIndex = 35;
            pictureBoxMap.TabStop = false;
            pictureBoxMap.Click += pictureBoxMap_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(44, 74, 110);
            button2.FlatAppearance.BorderColor = Color.PaleTurquoise;
            button2.FlatAppearance.BorderSize = 2;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Courier New", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.FromArgb(255, 235, 156);
            button2.Location = new Point(177, 651);
            button2.Name = "button2";
            button2.Size = new Size(132, 54);
            button2.TabIndex = 13;
            button2.Text = "BACK";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // btnPlay
            // 
            btnPlay.BackColor = Color.FromArgb(44, 74, 110);
            btnPlay.FlatAppearance.BorderColor = Color.PaleTurquoise;
            btnPlay.FlatAppearance.BorderSize = 2;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.Font = new Font("Courier New", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPlay.ForeColor = Color.FromArgb(255, 235, 156);
            btnPlay.Location = new Point(763, 651);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(132, 54);
            btnPlay.TabIndex = 12;
            btnPlay.Text = "PLAY";
            btnPlay.UseVisualStyleBackColor = false;
            btnPlay.Click += btnPlay_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Courier New", 26F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.PaleTurquoise;
            label1.Location = new Point(160, 32);
            label1.Name = "label1";
            label1.Size = new Size(735, 62);
            label1.TabIndex = 0;
            label1.Text = "OFFLINE MODE";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // OfflineMode_CPU
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(36, 58, 94);
            ClientSize = new Size(1057, 717);
            Controls.Add(panelMap);
            Controls.Add(panel1);
            Controls.Add(button2);
            Controls.Add(btnPlay);
            Controls.Add(label1);
            Controls.Add(panel2);
            DoubleBuffered = true;
            Name = "OfflineMode_CPU";
            Text = "OfflineMode_CPU";
            Load += OfflineModeSelection_Load;
            panel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panelMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Label label1;
        private Button btnPlay;
        private Button button2;
        private Button btnSelCharPlayer;
        private Label lblNameCharPlayer;
        private Label lblBotTitle;
        private Label lblYouTitle;
        private Label lblCharacterCaption;
        private Label lblPlayer2CharacterCaption;
        private Label lblNameCharPlayer2;
        private Button btnSelCharPlayer2;
        private Panel panelMap;
        private Label lblMapTitle;
        private Label lblMapCaption;
        private ComboBox comboBoxMap;
        private PictureBox pictureBoxMap;
    }
}
