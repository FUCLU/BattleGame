namespace BattleGame.Client.Forms
{
    partial class OfflineMode_CPU
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OfflineMode_CPU));
            panel1 = new Panel();
            btnHard = new Button();
            btnMedium = new Button();
            btnEasy = new Button();
            button2 = new Button();
            btnPlay = new Button();
            comboBoxMap = new ComboBox();
            label1 = new Label();
            panel2 = new Panel();
            pictureBoxMap = new PictureBox();
            btnSelCharPlayer = new Button();
            lblNameCharPlayer = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(btnHard);
            panel1.Controls.Add(btnMedium);
            panel1.Controls.Add(btnEasy);
            panel1.ForeColor = Color.Black;
            panel1.Location = new Point(93, 143);
            panel1.Name = "panel1";
            panel1.Size = new Size(360, 399);
            panel1.TabIndex = 1;
            // 
            // btnHard
            // 
            btnHard.BackColor = Color.Silver;
            btnHard.BackgroundImage = (Image)resources.GetObject("btnHard.BackgroundImage");
            btnHard.BackgroundImageLayout = ImageLayout.Stretch;
            btnHard.FlatAppearance.BorderColor = Color.LightBlue;
            btnHard.FlatAppearance.BorderSize = 2;
            btnHard.FlatStyle = FlatStyle.Flat;
            btnHard.Font = new Font("Algerian", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnHard.ForeColor = SystemColors.ActiveCaptionText;
            btnHard.Location = new Point(38, 288);
            btnHard.Name = "btnHard";
            btnHard.Size = new Size(285, 57);
            btnHard.TabIndex = 16;
            btnHard.UseVisualStyleBackColor = false;
            btnHard.Click += btnHard_Click;
            // 
            // btnMedium
            // 
            btnMedium.BackColor = Color.Transparent;
            btnMedium.BackgroundImage = (Image)resources.GetObject("btnMedium.BackgroundImage");
            btnMedium.BackgroundImageLayout = ImageLayout.Stretch;
            btnMedium.FlatAppearance.BorderColor = Color.LightBlue;
            btnMedium.FlatAppearance.BorderSize = 2;
            btnMedium.FlatStyle = FlatStyle.Flat;
            btnMedium.Font = new Font("Algerian", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMedium.ForeColor = SystemColors.ActiveCaptionText;
            btnMedium.Location = new Point(38, 205);
            btnMedium.Name = "btnMedium";
            btnMedium.Size = new Size(285, 58);
            btnMedium.TabIndex = 15;
            btnMedium.UseVisualStyleBackColor = false;
            btnMedium.Click += btnMedium_Click;
            // 
            // btnEasy
            // 
            btnEasy.BackColor = Color.Transparent;
            btnEasy.BackgroundImage = (Image)resources.GetObject("btnEasy.BackgroundImage");
            btnEasy.BackgroundImageLayout = ImageLayout.Stretch;
            btnEasy.FlatAppearance.BorderColor = Color.LightBlue;
            btnEasy.FlatAppearance.BorderSize = 2;
            btnEasy.FlatStyle = FlatStyle.Flat;
            btnEasy.Font = new Font("Algerian", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEasy.ForeColor = SystemColors.InactiveCaptionText;
            btnEasy.Location = new Point(38, 124);
            btnEasy.Name = "btnEasy";
            btnEasy.Size = new Size(285, 57);
            btnEasy.TabIndex = 14;
            btnEasy.UseVisualStyleBackColor = false;
            btnEasy.Click += btnEasy_Click;
            // 
            // button2
            // 
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.FlatStyle = FlatStyle.Popup;
            button2.Location = new Point(205, 548);
            button2.Name = "button2";
            button2.Size = new Size(126, 52);
            button2.TabIndex = 13;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // btnPlay
            // 
            btnPlay.BackgroundImage = (Image)resources.GetObject("btnPlay.BackgroundImage");
            btnPlay.BackgroundImageLayout = ImageLayout.Stretch;
            btnPlay.FlatStyle = FlatStyle.Popup;
            btnPlay.Location = new Point(712, 548);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(134, 52);
            btnPlay.TabIndex = 12;
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // comboBoxMap
            // 
            comboBoxMap.BackColor = Color.White;
            comboBoxMap.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMap.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxMap.ForeColor = Color.Black;
            comboBoxMap.FormattingEnabled = true;
            comboBoxMap.Items.AddRange(new object[] { "Map 1", "Map 2", "Map 3", "Forest" });
            comboBoxMap.Location = new Point(110, 140);
            comboBoxMap.Name = "comboBoxMap";
            comboBoxMap.Size = new Size(250, 36);
            comboBoxMap.TabIndex = 9;
            comboBoxMap.SelectedIndexChanged += comboBoxMap_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Algerian", 28.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Crimson;
            label1.Location = new Point(376, 26);
            label1.Name = "label1";
            label1.Size = new Size(0, 53);
            label1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.BackgroundImage = (Image)resources.GetObject("panel2.BackgroundImage");
            panel2.BackgroundImageLayout = ImageLayout.Stretch;
            panel2.Controls.Add(pictureBoxMap);
            panel2.Controls.Add(btnSelCharPlayer);
            panel2.Controls.Add(lblNameCharPlayer);
            panel2.Controls.Add(comboBoxMap);
            panel2.ForeColor = SystemColors.ControlText;
            panel2.Location = new Point(560, 143);
            panel2.Name = "panel2";
            panel2.Size = new Size(427, 399);
            panel2.TabIndex = 18;
            // 
            // pictureBoxMap
            // 
            pictureBoxMap.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBoxMap.Location = new Point(63, 196);
            pictureBoxMap.Name = "pictureBoxMap";
            pictureBoxMap.Size = new Size(297, 162);
            pictureBoxMap.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxMap.TabIndex = 22;
            pictureBoxMap.TabStop = false;
            // 
            // btnSelCharPlayer
            // 
            btnSelCharPlayer.BackgroundImage = (Image)resources.GetObject("btnSelCharPlayer.BackgroundImage");
            btnSelCharPlayer.BackgroundImageLayout = ImageLayout.Stretch;
            btnSelCharPlayer.FlatStyle = FlatStyle.Flat;
            btnSelCharPlayer.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSelCharPlayer.Location = new Point(51, 88);
            btnSelCharPlayer.Name = "btnSelCharPlayer";
            btnSelCharPlayer.Size = new Size(320, 46);
            btnSelCharPlayer.TabIndex = 21;
            btnSelCharPlayer.UseVisualStyleBackColor = true;
            btnSelCharPlayer.Click += btnSelCharPlayer_Click;
            // 
            // lblNameCharPlayer
            // 
            lblNameCharPlayer.AutoSize = true;
            lblNameCharPlayer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNameCharPlayer.ForeColor = Color.Orange;
            lblNameCharPlayer.Location = new Point(160, 53);
            lblNameCharPlayer.Name = "lblNameCharPlayer";
            lblNameCharPlayer.Size = new Size(75, 28);
            lblNameCharPlayer.TabIndex = 20;
            lblNameCharPlayer.Text = "Kabold";
            // 
            // OfflineMode_CPU
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1057, 646);
            Controls.Add(panel1);
            Controls.Add(button2);
            Controls.Add(btnPlay);
            Controls.Add(label1);
            Controls.Add(panel2);
            DoubleBuffered = true;
            Name = "OfflineMode_CPU";
            Text = "OfflineMode_CPU";
            Load += OfflineModeSelection_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private ComboBox comboBoxMap;
        private Button btnPlay;
        private Button button2;
        private Button btnHard;
        private Button btnMedium;
        private Button btnEasy;
        private Panel panel2;
        private Button btnSelCharPlayer;
        private Label lblNameCharPlayer;
        private PictureBox pictureBoxMap;
    }
}
