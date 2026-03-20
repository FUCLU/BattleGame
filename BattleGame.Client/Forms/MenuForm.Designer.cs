namespace BattleGame.Client.Forms
{
    partial class MenuForm
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
            pictureBox1 = new PictureBox();
            btnPlay = new Button();
            btnLogout = new Button();
            btnExit = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Gemini_Generated_Image_141glb141glb141g;
            pictureBox1.Location = new Point(-59, -73);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1072, 638);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // btnPlay
            // 
            btnPlay.BackColor = Color.Transparent;
            btnPlay.Font = new Font("Stencil", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnPlay.ForeColor = Color.Fuchsia;
            btnPlay.Image = Properties.Resources.fr;
            btnPlay.Location = new Point(312, 185);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(198, 52);
            btnPlay.TabIndex = 1;
            btnPlay.Text = "Play";
            btnPlay.UseVisualStyleBackColor = false;
            btnPlay.Click += btnPlay_Click;
            btnPlay.MouseEnter += btnPlay_MouseEnter;
            btnPlay.MouseLeave += btnPlay_MouseLeave;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.Transparent;
            btnLogout.Font = new Font("Stencil", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnLogout.ForeColor = Color.Fuchsia;
            btnLogout.Image = Properties.Resources.fr;
            btnLogout.Location = new Point(312, 288);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(198, 51);
            btnLogout.TabIndex = 2;
            btnLogout.Text = "Log out";
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            btnLogout.MouseEnter += btnLogout_MouseEnter;
            btnLogout.MouseLeave += btnLogout_MouseLeave;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.Transparent;
            btnExit.Font = new Font("Stencil", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnExit.ForeColor = Color.Fuchsia;
            btnExit.Image = Properties.Resources.fr;
            btnExit.Location = new Point(312, 388);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(198, 51);
            btnExit.TabIndex = 3;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            btnExit.MouseEnter += btnExit_MouseEnter;
            btnExit.MouseLeave += btnExit_MouseLeave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ActiveCaptionText;
            label1.FlatStyle = FlatStyle.Popup;
            label1.Font = new Font("Castellar", 36F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.DeepPink;
            label1.Location = new Point(170, 25);
            label1.Name = "label1";
            label1.Size = new Size(507, 73);
            label1.TabIndex = 4;
            label1.Text = "BATTLE GAME";
            label1.TextAlign = ContentAlignment.TopRight;
            // 
            // MenuForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(853, 530);
            Controls.Add(label1);
            Controls.Add(btnExit);
            Controls.Add(btnLogout);
            Controls.Add(btnPlay);
            Controls.Add(pictureBox1);
            Name = "MenuForm";
            Text = "MenuForm";
            Load += MenuForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button btnPlay;
        private Button btnLogout;
        private Button btnExit;
        private Label label1;
    }
}