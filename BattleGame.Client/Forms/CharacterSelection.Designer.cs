namespace BattleGame.Client.Forms
{
    partial class CharacterSelection
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterSelection));
            panel1 = new Panel();
            panel2 = new Panel();
            pictureBox2 = new PictureBox();
            label1 = new Label();
            btnPrev = new Button();
            btnNext = new Button();
            panel3 = new Panel();
            pictureBox5 = new PictureBox();
            label5 = new Label();
            lblHP = new Label();
            lblATK = new Label();
            lblDEF = new Label();
            lblSPD = new Label();
            lblSkill = new Label();
            button1 = new Button();
            button2 = new Button();

            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            SuspendLayout();

            // panel1
            panel1.BackColor = Color.MidnightBlue;
            panel1.BackgroundImage = Properties.Resources.login;
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(button2);
            panel1.Location = new Point(-6, -2);
            panel1.Name = "panel1";
            panel1.Size = new Size(1059, 581);
            panel1.TabIndex = 0;

            // panel2 - TRÁI
            panel2.BackColor = Color.Black;
            panel2.Controls.Add(pictureBox2);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(btnPrev);
            panel2.Controls.Add(btnNext);
            panel2.Location = new Point(53, 37);
            panel2.Name = "panel2";
            panel2.Size = new Size(387, 460);
            panel2.TabIndex = 0;

            // pictureBox2
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Location = new Point(83, 80);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(220, 230);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;

            // label1 - tên nhân vật trái
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Showcard Gothic", 14F);
            label1.ForeColor = Color.Aqua;
            label1.Location = new Point(37, 330);
            label1.Name = "label1";
            label1.Size = new Size(304, 40);
            label1.TabIndex = 1;
            label1.Text = "Warrior";
            label1.TextAlign = ContentAlignment.MiddleCenter;

            // btnPrev
            btnPrev.BackColor = Color.Crimson;
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Font = new Font("Arial", 18F, FontStyle.Bold);
            btnPrev.ForeColor = Color.White;
            btnPrev.Location = new Point(13, 176);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(55, 55);
            btnPrev.TabIndex = 2;
            btnPrev.Text = "◄";
            btnPrev.UseVisualStyleBackColor = false;

            // btnNext
            btnNext.BackColor = Color.Crimson;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Font = new Font("Arial", 18F, FontStyle.Bold);
            btnNext.ForeColor = Color.White;
            btnNext.Location = new Point(319, 176);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(55, 55);
            btnNext.TabIndex = 3;
            btnNext.Text = "►";
            btnNext.UseVisualStyleBackColor = false;

            // panel3 - PHẢI
            panel3.BackColor = Color.FromArgb(40, 40, 40);
            panel3.Controls.Add(pictureBox5);
            panel3.Controls.Add(label5);
            panel3.Controls.Add(lblHP);
            panel3.Controls.Add(lblATK);
            panel3.Controls.Add(lblDEF);
            panel3.Controls.Add(lblSPD);
            panel3.Controls.Add(lblSkill);
            panel3.Location = new Point(508, 21);
            panel3.Name = "panel3";
            panel3.Size = new Size(480, 475);
            panel3.TabIndex = 1;

            // pictureBox5
            pictureBox5.BackColor = Color.Transparent;
            pictureBox5.Location = new Point(115, 16);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(250, 200);
            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.TabIndex = 0;
            pictureBox5.TabStop = false;

            // label5 - tên nhân vật phải
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Showcard Gothic", 16.2F);
            label5.ForeColor = Color.Aqua;
            label5.Location = new Point(90, 225);
            label5.Name = "label5";
            label5.Size = new Size(300, 40);
            label5.TabIndex = 1;
            label5.Text = "Warrior";
            label5.TextAlign = ContentAlignment.MiddleCenter;

            // Thông số - bắt đầu ngay dưới label5
            var sf = new Font("Courier New", 10F, FontStyle.Bold);
            int sx = 20, sy = 275, gap = 36;

            SetStatLabel(lblHP, sx, sy, sf, Color.LightYellow, "❤  HP     :  120");
            SetStatLabel(lblATK, sx, sy + gap, sf, Color.OrangeRed, "⚔  ATK    :  40");
            SetStatLabel(lblDEF, sx, sy + gap * 2, sf, Color.SkyBlue, "🛡  DEF    :  25");
            SetStatLabel(lblSPD, sx, sy + gap * 3, sf, Color.LightGreen, "💨  SPEED  :  15");
            SetStatLabel(lblSkill, sx, sy + gap * 4, sf, Color.Gold, "✨  SKILL   :  Blade Slash");

            lblHP.Name = "lblHP"; lblHP.TabIndex = 2;
            lblATK.Name = "lblATK"; lblATK.TabIndex = 3;
            lblDEF.Name = "lblDEF"; lblDEF.TabIndex = 4;
            lblSPD.Name = "lblSPD"; lblSPD.TabIndex = 5;
            lblSkill.Name = "lblSkill"; lblSkill.TabIndex = 6;

            // button1 - SELECT
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button1.Location = new Point(508, 515);
            button1.Name = "button1";
            button1.Size = new Size(157, 43);
            button1.TabIndex = 1;
            button1.Click += button1_Click;

            // button2 - BACK
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.Location = new Point(818, 515);
            button2.Name = "button2";
            button2.Size = new Size(170, 43);
            button2.TabIndex = 0;

            // FORM
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1057, 581);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "CharacterSelection";
            Text = "CharacterSelection";
            Load += CharacterSelection_Load;

            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ResumeLayout(false);
        }

        private void SetStatLabel(Label lbl, int x, int y, Font f, Color c, string text)
        {
            lbl.AutoSize = false;
            lbl.Font = f;
            lbl.ForeColor = c;
            lbl.BackColor = Color.Transparent;
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(440, 28);
            lbl.Text = text;
        }

        #endregion

        private Panel panel1, panel2, panel3;
        private PictureBox pictureBox2, pictureBox5;
        private Label label1, label5;
        private Label lblHP, lblATK, lblDEF, lblSPD, lblSkill;
        private Button button1, button2;
        private Button btnPrev, btnNext;
    }
}