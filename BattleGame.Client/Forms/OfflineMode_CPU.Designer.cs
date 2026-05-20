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
            label9 = new Label();
            btnHard = new Button();
            btnMedium = new Button();
            btnEasy = new Button();
            label2 = new Label();
            button2 = new Button();
            btnPlay = new Button();
            label7 = new Label();
            comboBoxMap = new ComboBox();
            label6 = new Label();
            label3 = new Label();
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
            panel1.BackColor = Color.White;
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(label9);
            panel1.Controls.Add(btnHard);
            panel1.Controls.Add(btnMedium);
            panel1.Controls.Add(btnEasy);
            panel1.Controls.Add(label2);
            panel1.ForeColor = Color.Black;
            panel1.Location = new Point(74, 152);
            panel1.Name = "panel1";
            panel1.Size = new Size(360, 390);
            panel1.TabIndex = 1;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = SystemColors.ActiveCaptionText;
            label9.Location = new Point(29, 67);
            label9.Name = "label9";
            label9.Size = new Size(84, 38);
            label9.TabIndex = 19;
            label9.Text = "Level";
            // 
            // btnHard
            // 
            btnHard.BackColor = Color.Silver;
            btnHard.FlatAppearance.BorderColor = Color.LightBlue;
            btnHard.FlatAppearance.BorderSize = 2;
            btnHard.Font = new Font("Algerian", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnHard.ForeColor = SystemColors.ActiveCaptionText;
            btnHard.Location = new Point(29, 298);
            btnHard.Name = "btnHard";
            btnHard.Size = new Size(304, 53);
            btnHard.TabIndex = 16;
            btnHard.Text = "HARD";
            btnHard.UseVisualStyleBackColor = false;
            btnHard.Click += btnHard_Click;
            // 
            // btnMedium
            // 
            btnMedium.BackColor = Color.Silver;
            btnMedium.FlatAppearance.BorderColor = Color.LightBlue;
            btnMedium.FlatAppearance.BorderSize = 2;
            btnMedium.Font = new Font("Algerian", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMedium.ForeColor = SystemColors.ActiveCaptionText;
            btnMedium.Location = new Point(29, 215);
            btnMedium.Name = "btnMedium";
            btnMedium.Size = new Size(304, 54);
            btnMedium.TabIndex = 15;
            btnMedium.Text = "MEDIUM";
            btnMedium.UseVisualStyleBackColor = false;
            btnMedium.Click += btnMedium_Click;
            // 
            // btnEasy
            // 
            btnEasy.BackColor = Color.Silver;
            btnEasy.FlatAppearance.BorderColor = Color.LightBlue;
            btnEasy.FlatAppearance.BorderSize = 2;
            btnEasy.Font = new Font("Algerian", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEasy.ForeColor = SystemColors.InactiveCaptionText;
            btnEasy.Location = new Point(29, 124);
            btnEasy.Name = "btnEasy";
            btnEasy.Size = new Size(304, 57);
            btnEasy.TabIndex = 14;
            btnEasy.Text = "EASY";
            btnEasy.UseVisualStyleBackColor = false;
            btnEasy.Click += btnEasy_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Algerian", 19.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(139, 23);
            label2.Name = "label2";
            label2.Size = new Size(80, 38);
            label2.TabIndex = 1;
            label2.Text = "Bot";
            // 
            // button2
            // 
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.FlatStyle = FlatStyle.Popup;
            button2.Location = new Point(178, 568);
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
            btnPlay.Location = new Point(732, 558);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(134, 52);
            btnPlay.TabIndex = 12;
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Book Antiqua", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = SystemColors.ActiveCaptionText;
            label7.Location = new Point(36, 152);
            label7.Name = "label7";
            label7.Size = new Size(57, 24);
            label7.TabIndex = 10;
            label7.Text = "Map:";
            // 
            // comboBoxMap
            // 
            comboBoxMap.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMap.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxMap.ForeColor = Color.Black;
            comboBoxMap.FormattingEnabled = true;
            comboBoxMap.Items.AddRange(new object[] { "Map 1", "Map 2", "Map 3", "Forest" });
            comboBoxMap.Location = new Point(106, 145);
            comboBoxMap.Name = "comboBoxMap";
            comboBoxMap.Size = new Size(250, 36);
            comboBoxMap.TabIndex = 9;
            comboBoxMap.SelectedIndexChanged += comboBoxMap_SelectedIndexChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Book Antiqua", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ActiveCaptionText;
            label6.Location = new Point(34, 58);
            label6.Name = "label6";
            label6.Size = new Size(103, 24);
            label6.TabIndex = 8;
            label6.Text = "Character:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Arial Narrow", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.MenuHighlight;
            label3.Location = new Point(160, 11);
            label3.Name = "label3";
            label3.Size = new Size(58, 33);
            label3.TabIndex = 2;
            label3.Text = "You";
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
            panel2.BackColor = Color.White;
            panel2.BackgroundImageLayout = ImageLayout.Stretch;
            panel2.Controls.Add(pictureBoxMap);
            panel2.Controls.Add(btnSelCharPlayer);
            panel2.Controls.Add(lblNameCharPlayer);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(comboBoxMap);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(label6);
            panel2.ForeColor = SystemColors.ControlText;
            panel2.Location = new Point(604, 152);
            panel2.Name = "panel2";
            panel2.Size = new Size(395, 390);
            panel2.TabIndex = 18;
            // 
            // pictureBoxMap
            // 
            pictureBoxMap.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBoxMap.Location = new Point(36, 198);
            pictureBoxMap.Name = "pictureBoxMap";
            pictureBoxMap.Size = new Size(320, 172);
            pictureBoxMap.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxMap.TabIndex = 22;
            pictureBoxMap.TabStop = false;
            // 
            // btnSelCharPlayer
            // 
            btnSelCharPlayer.BackgroundImageLayout = ImageLayout.Stretch;
            btnSelCharPlayer.FlatStyle = FlatStyle.Popup;
            btnSelCharPlayer.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSelCharPlayer.Location = new Point(36, 89);
            btnSelCharPlayer.Name = "btnSelCharPlayer";
            btnSelCharPlayer.Size = new Size(320, 41);
            btnSelCharPlayer.TabIndex = 21;
            btnSelCharPlayer.Text = "SELECT CHARACTER";
            btnSelCharPlayer.UseVisualStyleBackColor = true;
            btnSelCharPlayer.Click += btnSelCharPlayer_Click;
            // 
            // lblNameCharPlayer
            // 
            lblNameCharPlayer.AutoSize = true;
            lblNameCharPlayer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNameCharPlayer.ForeColor = Color.Red;
            lblNameCharPlayer.Location = new Point(143, 55);
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
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label7;
        private ComboBox comboBoxMap;
        private Label label6;
        private Button btnPlay;
        private Button button2;
        private Button btnHard;
        private Button btnMedium;
        private Button btnEasy;
        private Panel panel2;
        private Label label9;
        private Button btnSelCharPlayer;
        private Label lblNameCharPlayer;
        private PictureBox pictureBoxMap;
    }
}
