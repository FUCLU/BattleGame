namespace BattleGame.Client.Forms
{
    partial class GameOverForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameOverForm));
            btnBackLobby = new Button();
            panel1 = new Panel();
            BtnLeaderBoard = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnBackLobby
            // 
            btnBackLobby.BackColor = Color.FromArgb(52, 152, 219);
            btnBackLobby.BackgroundImage = (Image)resources.GetObject("btnBackLobby.BackgroundImage");
            btnBackLobby.BackgroundImageLayout = ImageLayout.Stretch;
            btnBackLobby.FlatStyle = FlatStyle.Popup;
            btnBackLobby.Font = new Font("Book Antiqua", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBackLobby.ForeColor = Color.White;
            btnBackLobby.Location = new Point(145, 335);
            btnBackLobby.Name = "btnBackLobby";
            btnBackLobby.Size = new Size(276, 77);
            btnBackLobby.TabIndex = 3;
            btnBackLobby.UseVisualStyleBackColor = false;
            btnBackLobby.Click += btnBackLobby_Click;
            btnBackLobby.MouseHover += btnBackLobby_MouseHover;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(44, 74, 110);
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(BtnLeaderBoard);
            panel1.Controls.Add(btnBackLobby);
            panel1.Location = new Point(-4, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(952, 569);
            panel1.TabIndex = 0;
            // 
            // BtnLeaderBoard
            // 
            BtnLeaderBoard.BackColor = Color.FromArgb(52, 152, 219);
            BtnLeaderBoard.BackgroundImage = (Image)resources.GetObject("BtnLeaderBoard.BackgroundImage");
            BtnLeaderBoard.BackgroundImageLayout = ImageLayout.Stretch;
            BtnLeaderBoard.FlatStyle = FlatStyle.Popup;
            BtnLeaderBoard.Font = new Font("Book Antiqua", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BtnLeaderBoard.ForeColor = Color.White;
            BtnLeaderBoard.Location = new Point(523, 335);
            BtnLeaderBoard.Name = "BtnLeaderBoard";
            BtnLeaderBoard.Size = new Size(276, 77);
            BtnLeaderBoard.TabIndex = 4;
            BtnLeaderBoard.UseVisualStyleBackColor = false;
            // 
            // GameOverForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(943, 557);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "GameOverForm";
            Text = "GameOverForm";
            Load += GameOverForm_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnBackLobby;
        private Panel panel1;
        private Button BtnLeaderBoard;
    }
}