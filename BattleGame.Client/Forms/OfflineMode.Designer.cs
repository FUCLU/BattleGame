namespace BattleGame.Client.Forms
{
    partial class OfflineMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OfflineMode));
            picBotLogo = new PictureBox();
            picCpuLogo = new PictureBox();
            btnVsBot = new Button();
            btnDungeon = new Button();
            button1 = new Button();
            btnSetting = new GearButton();
            ((System.ComponentModel.ISupportInitialize)picBotLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCpuLogo).BeginInit();
            SuspendLayout();
            // 
            // picBotLogo
            // 
            picBotLogo.BackColor = Color.Transparent;
            picBotLogo.Image = (Image)resources.GetObject("picBotLogo.Image");
            picBotLogo.Location = new Point(91, 124);
            picBotLogo.Name = "picBotLogo";
            picBotLogo.Size = new Size(230, 180);
            picBotLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picBotLogo.TabIndex = 0;
            picBotLogo.TabStop = false;
            // 
            // picCpuLogo
            // 
            picCpuLogo.BackColor = Color.Transparent;
            picCpuLogo.Image = (Image)resources.GetObject("picCpuLogo.Image");
            picCpuLogo.Location = new Point(479, 124);
            picCpuLogo.Name = "picCpuLogo";
            picCpuLogo.Size = new Size(230, 180);
            picCpuLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picCpuLogo.TabIndex = 1;
            picCpuLogo.TabStop = false;
            // 
            // btnVsBot
            // 
            btnVsBot.BackColor = Color.FromArgb(42, 93, 143);
            btnVsBot.FlatStyle = FlatStyle.Popup;
            btnVsBot.Font = new Font("Algerian", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVsBot.ForeColor = Color.White;
            btnVsBot.Location = new Point(119, 324);
            btnVsBot.Name = "btnVsBot";
            btnVsBot.Size = new Size(174, 48);
            btnVsBot.TabIndex = 2;
            btnVsBot.Text = "PVP";
            btnVsBot.UseVisualStyleBackColor = false;
            btnVsBot.Click += btnVsBot_Click;
            // 
            // btnDungeon
            // 
            btnDungeon.BackColor = Color.FromArgb(42, 93, 143);
            btnDungeon.FlatStyle = FlatStyle.Popup;
            btnDungeon.Font = new Font("Algerian", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDungeon.ForeColor = Color.White;
            btnDungeon.Location = new Point(507, 324);
            btnDungeon.Name = "btnDungeon";
            btnDungeon.Size = new Size(174, 48);
            btnDungeon.TabIndex = 3;
            btnDungeon.Text = "Dungeon";
            btnDungeon.UseVisualStyleBackColor = false;
            btnDungeon.Click += btnDungeon_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(42, 93, 143);
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ButtonFace;
            button1.Location = new Point(315, 410);
            button1.Name = "button1";
            button1.Size = new Size(163, 56);
            button1.TabIndex = 4;
            button1.Text = "BACK";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // btnSetting
            // 
            btnSetting.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSetting.BackColor = Color.Black;
            btnSetting.FlatStyle = FlatStyle.Popup;
            btnSetting.ForeColor = Color.FromArgb(255, 235, 156);
            btnSetting.Location = new Point(739, 417);
            btnSetting.Name = "btnSetting";
            btnSetting.Size = new Size(48, 48);
            btnSetting.TabIndex = 5;
            btnSetting.UseVisualStyleBackColor = false;
            // 
            // OfflineMode
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(808, 487);
            Controls.Add(btnSetting);
            Controls.Add(button1);
            Controls.Add(btnDungeon);
            Controls.Add(btnVsBot);
            Controls.Add(picCpuLogo);
            Controls.Add(picBotLogo);
            Name = "OfflineMode";
            Text = "OfflineMode";
            ((System.ComponentModel.ISupportInitialize)picBotLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCpuLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox picBotLogo;
        private System.Windows.Forms.PictureBox picCpuLogo;
        private System.Windows.Forms.Button btnVsBot;
        private System.Windows.Forms.Button btnDungeon;
        private Button button1;
        private GearButton btnSetting;
    }
}
