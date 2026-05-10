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
            this.components = new System.ComponentModel.Container();
            this.picBotLogo = new System.Windows.Forms.PictureBox();
            this.picCpuLogo = new System.Windows.Forms.PictureBox();
            this.btnVsBot = new System.Windows.Forms.Button();
            this.btnDungeon = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picBotLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCpuLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // picBotLogo
            // 
            this.picBotLogo.BackColor = System.Drawing.Color.Transparent;
            this.picBotLogo.Image = (System.Drawing.Image)resources.GetObject("picBotLogo.Image");
            this.picBotLogo.Location = new System.Drawing.Point(91, 124);
            this.picBotLogo.Name = "picBotLogo";
            this.picBotLogo.Size = new System.Drawing.Size(230, 180);
            this.picBotLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBotLogo.TabIndex = 0;
            this.picBotLogo.TabStop = false;
            // 
            // picCpuLogo
            // 
            this.picCpuLogo.BackColor = System.Drawing.Color.Transparent;
            this.picCpuLogo.Image = (System.Drawing.Image)resources.GetObject("picCpuLogo.Image");
            this.picCpuLogo.Location = new System.Drawing.Point(479, 124);
            this.picCpuLogo.Name = "picCpuLogo";
            this.picCpuLogo.Size = new System.Drawing.Size(230, 180);
            this.picCpuLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCpuLogo.TabIndex = 1;
            this.picCpuLogo.TabStop = false;
            // 
            // btnVsBot
            // 
            this.btnVsBot.BackColor = System.Drawing.Color.FromArgb(42, 93, 143);
            this.btnVsBot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVsBot.Font = new System.Drawing.Font("Algerian", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this.btnVsBot.ForeColor = System.Drawing.Color.White;
            this.btnVsBot.Location = new System.Drawing.Point(119, 324);
            this.btnVsBot.Name = "btnVsBot";
            this.btnVsBot.Size = new System.Drawing.Size(174, 48);
            this.btnVsBot.TabIndex = 2;
            this.btnVsBot.Text = "VS BOT";
            this.btnVsBot.UseVisualStyleBackColor = false;
            this.btnVsBot.Click += new System.EventHandler(this.btnVsBot_Click);
            // 
            // btnDungeon
            // 
            this.btnDungeon.BackColor = System.Drawing.Color.FromArgb(42, 93, 143);
            this.btnDungeon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDungeon.Font = new System.Drawing.Font("Algerian", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this.btnDungeon.ForeColor = System.Drawing.Color.White;
            this.btnDungeon.Location = new System.Drawing.Point(507, 324);
            this.btnDungeon.Name = "btnDungeon";
            this.btnDungeon.Size = new System.Drawing.Size(174, 48);
            this.btnDungeon.TabIndex = 3;
            this.btnDungeon.Text = "Dungeon";
            this.btnDungeon.UseVisualStyleBackColor = false;
            this.btnDungeon.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // OfflineMode
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnDungeon);
            this.Controls.Add(this.btnVsBot);
            this.Controls.Add(this.picCpuLogo);
            this.Controls.Add(this.picBotLogo);
            this.Text = "OfflineMode";
            ((System.ComponentModel.ISupportInitialize)(this.picBotLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCpuLogo)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox picBotLogo;
        private System.Windows.Forms.PictureBox picCpuLogo;
        private System.Windows.Forms.Button btnVsBot;
        private System.Windows.Forms.Button btnDungeon;
    }
}
