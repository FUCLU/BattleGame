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
            panel4 = new Panel();
            label5 = new Label();
            panel2 = new Panel();
            pnlWizard = new Panel();
            pbWizard = new PictureBox();
            lblWizardName = new Label();
            pnlSamurai = new Panel();
            pbSamurai = new PictureBox();
            lblSamuraiName = new Label();
            pnlKitsune = new Panel();
            pbKitsune = new PictureBox();
            lblKitsuneName = new Label();
            pnlLord = new Panel();
            pbLord = new PictureBox();
            lblLordName = new Label();
            panel3 = new Panel();
            panel1 = new Panel();
            pbInfor = new PictureBox();
            panel5 = new Panel();
            lblDEF = new Label();
            lblSkill = new Label();
            lblSPD = new Label();
            lblATK = new Label();
            lblHP = new Label();
            btnSellect = new Button();
            button2 = new Button();
            label2 = new Label();
            panel6 = new Panel();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            panel4.SuspendLayout();
            panel2.SuspendLayout();
            pnlWizard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbWizard).BeginInit();
            pnlSamurai.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbSamurai).BeginInit();
            pnlKitsune.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbKitsune).BeginInit();
            pnlLord.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLord).BeginInit();
            panel3.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbInfor).BeginInit();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel4
            // 
            panel4.BackColor = Color.SteelBlue;
            panel4.BorderStyle = BorderStyle.Fixed3D;
            panel4.Controls.Add(label5);
            panel4.Controls.Add(panel2);
            panel4.Controls.Add(panel3);
            panel4.Location = new Point(20, 20);
            panel4.Name = "panel4";
            panel4.Size = new Size(1013, 539);
            panel4.TabIndex = 3;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Britannic Bold", 19.8F);
            label5.ForeColor = Color.PaleTurquoise;
            label5.Location = new Point(368, 2);
            label5.Name = "label5";
            label5.Size = new Size(292, 37);
            label5.TabIndex = 3;
            label5.Text = "🛡️Chọn nhân vật🛡️";
            // 
            // panel2
            // 
            panel2.AutoScroll = true;
            panel2.BackColor = Color.FromArgb(36, 58, 94);
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(pnlWizard);
            panel2.Controls.Add(pnlSamurai);
            panel2.Controls.Add(pnlKitsune);
            panel2.Controls.Add(pnlLord);
            panel2.Location = new Point(15, 51);
            panel2.Name = "panel2";
            panel2.Size = new Size(421, 443);
            panel2.TabIndex = 2;
            // 
            // pnlWizard
            // 
            pnlWizard.BackColor = Color.FromArgb(44, 74, 110);
            pnlWizard.Controls.Add(pbWizard);
            pnlWizard.Controls.Add(lblWizardName);
            pnlWizard.Location = new Point(10, 10);
            pnlWizard.Name = "pnlWizard";
            pnlWizard.Size = new Size(366, 68);
            pnlWizard.TabIndex = 0;
            pnlWizard.Click += CharacterPanel_Click;
            // 
            // pbWizard
            // 
            pbWizard.BackColor = Color.Transparent;
            pbWizard.Location = new Point(3, 3);
            pbWizard.Name = "pbWizard";
            pbWizard.Size = new Size(78, 57);
            pbWizard.SizeMode = PictureBoxSizeMode.StretchImage;
            pbWizard.TabIndex = 0;
            pbWizard.TabStop = false;
            pbWizard.Click += CharacterPanel_Click;
            // 
            // lblWizardName
            // 
            lblWizardName.AutoSize = true;
            lblWizardName.Font = new Font("Book Antiqua", 19.8F, FontStyle.Bold | FontStyle.Italic);
            lblWizardName.ForeColor = Color.FromArgb(208, 230, 255);
            lblWizardName.Location = new Point(110, 15);
            lblWizardName.Name = "lblWizardName";
            lblWizardName.Size = new Size(127, 39);
            lblWizardName.TabIndex = 1;
            lblWizardName.Text = "Wizard";
            lblWizardName.Click += CharacterPanel_Click;
            // 
            // pnlSamurai
            // 
            pnlSamurai.BackColor = Color.FromArgb(44, 74, 110);
            pnlSamurai.Controls.Add(pbSamurai);
            pnlSamurai.Controls.Add(lblSamuraiName);
            pnlSamurai.Location = new Point(10, 101);
            pnlSamurai.Name = "pnlSamurai";
            pnlSamurai.Size = new Size(366, 68);
            pnlSamurai.TabIndex = 0;
            pnlSamurai.Click += CharacterPanel_Click;
            // 
            // pbSamurai
            // 
            pbSamurai.BackColor = Color.Transparent;
            pbSamurai.Location = new Point(3, 3);
            pbSamurai.Name = "pbSamurai";
            pbSamurai.Size = new Size(78, 57);
            pbSamurai.SizeMode = PictureBoxSizeMode.StretchImage;
            pbSamurai.TabIndex = 0;
            pbSamurai.TabStop = false;
            pbSamurai.Click += CharacterPanel_Click;
            // 
            // lblSamuraiName
            // 
            lblSamuraiName.AutoSize = true;
            lblSamuraiName.Font = new Font("Book Antiqua", 19.8F, FontStyle.Bold | FontStyle.Italic);
            lblSamuraiName.ForeColor = Color.FromArgb(208, 230, 255);
            lblSamuraiName.Location = new Point(110, 12);
            lblSamuraiName.Name = "lblSamuraiName";
            lblSamuraiName.Size = new Size(140, 39);
            lblSamuraiName.TabIndex = 1;
            lblSamuraiName.Text = "Samurai";
            lblSamuraiName.Click += CharacterPanel_Click;
            // 
            // pnlKitsune
            // 
            pnlKitsune.BackColor = Color.FromArgb(44, 74, 110);
            pnlKitsune.Controls.Add(pbKitsune);
            pnlKitsune.Controls.Add(lblKitsuneName);
            pnlKitsune.Location = new Point(10, 193);
            pnlKitsune.Name = "pnlKitsune";
            pnlKitsune.Size = new Size(366, 68);
            pnlKitsune.TabIndex = 0;
            pnlKitsune.Click += CharacterPanel_Click;
            pnlKitsune.Paint += pnlKitsune_Paint;
            // 
            // pbKitsune
            // 
            pbKitsune.BackColor = Color.Transparent;
            pbKitsune.Location = new Point(3, 3);
            pbKitsune.Name = "pbKitsune";
            pbKitsune.Size = new Size(78, 57);
            pbKitsune.SizeMode = PictureBoxSizeMode.StretchImage;
            pbKitsune.TabIndex = 0;
            pbKitsune.TabStop = false;
            pbKitsune.Click += CharacterPanel_Click;
            // 
            // lblKitsuneName
            // 
            lblKitsuneName.AutoSize = true;
            lblKitsuneName.Font = new Font("Book Antiqua", 19.8F, FontStyle.Bold | FontStyle.Italic);
            lblKitsuneName.ForeColor = Color.FromArgb(208, 230, 255);
            lblKitsuneName.Location = new Point(110, 11);
            lblKitsuneName.Name = "lblKitsuneName";
            lblKitsuneName.Size = new Size(131, 39);
            lblKitsuneName.TabIndex = 1;
            lblKitsuneName.Text = "Kitsune";
            lblKitsuneName.Click += CharacterPanel_Click;
            // 
            // pnlLord
            // 
            pnlLord.BackColor = Color.FromArgb(44, 74, 110);
            pnlLord.Controls.Add(pbLord);
            pnlLord.Controls.Add(lblLordName);
            pnlLord.Location = new Point(11, 287);
            pnlLord.Name = "pnlLord";
            pnlLord.Size = new Size(366, 68);
            pnlLord.TabIndex = 0;
            pnlLord.Click += CharacterPanel_Click;
            // 
            // pbLord
            // 
            pbLord.BackColor = Color.Transparent;
            pbLord.Location = new Point(3, 3);
            pbLord.Name = "pbLord";
            pbLord.Size = new Size(78, 57);
            pbLord.SizeMode = PictureBoxSizeMode.StretchImage;
            pbLord.TabIndex = 0;
            pbLord.TabStop = false;
            pbLord.Click += CharacterPanel_Click;
            // 
            // lblLordName
            // 
            lblLordName.AutoSize = true;
            lblLordName.Font = new Font("Book Antiqua", 19.8F, FontStyle.Bold | FontStyle.Italic);
            lblLordName.ForeColor = Color.FromArgb(208, 230, 255);
            lblLordName.Location = new Point(110, 9);
            lblLordName.Name = "lblLordName";
            lblLordName.Size = new Size(86, 39);
            lblLordName.TabIndex = 1;
            lblLordName.Text = "Lord";
            lblLordName.Click += CharacterPanel_Click;
            // 
            // panel3
            // 
            panel3.BackColor = Color.FromArgb(31, 47, 86);
            panel3.BorderStyle = BorderStyle.Fixed3D;
            panel3.Controls.Add(panel1);
            panel3.Controls.Add(panel5);
            panel3.Controls.Add(btnSellect);
            panel3.Controls.Add(button2);
            panel3.Controls.Add(label2);
            panel3.Location = new Point(451, 51);
            panel3.Name = "panel3";
            panel3.Size = new Size(555, 443);
            panel3.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(15, 28, 46);
            panel1.Controls.Add(pbInfor);
            panel1.Location = new Point(11, 94);
            panel1.Name = "panel1";
            panel1.Size = new Size(230, 212);
            panel1.TabIndex = 8;
            // 
            // pbInfor
            // 
            pbInfor.BackColor = Color.Transparent;
            pbInfor.Image = (Image)resources.GetObject("pbInfor.Image");
            pbInfor.Location = new Point(3, 7);
            pbInfor.Name = "pbInfor";
            pbInfor.Size = new Size(224, 202);
            pbInfor.SizeMode = PictureBoxSizeMode.Zoom;
            pbInfor.TabIndex = 2;
            pbInfor.TabStop = false;
            // 
            // panel5
            // 
            panel5.BackColor = Color.FromArgb(46, 76, 109);
            panel5.Controls.Add(lblDEF);
            panel5.Controls.Add(lblSkill);
            panel5.Controls.Add(lblSPD);
            panel5.Controls.Add(lblATK);
            panel5.Controls.Add(lblHP);
            panel5.Location = new Point(247, 91);
            panel5.Name = "panel5";
            panel5.Size = new Size(291, 215);
            panel5.TabIndex = 7;
            // 
            // lblDEF
            // 
            lblDEF.AutoSize = true;
            lblDEF.Font = new Font("Showcard Gothic", 10.8F, FontStyle.Italic);
            lblDEF.ForeColor = Color.DarkOrange;
            lblDEF.Location = new Point(20, 89);
            lblDEF.Name = "lblDEF";
            lblDEF.Size = new Size(43, 23);
            lblDEF.TabIndex = 10;
            lblDEF.Text = "DEF";
            // 
            // lblSkill
            // 
            lblSkill.AutoSize = true;
            lblSkill.Font = new Font("Showcard Gothic", 10.8F, FontStyle.Italic);
            lblSkill.ForeColor = Color.DarkOrange;
            lblSkill.Location = new Point(18, 174);
            lblSkill.Name = "lblSkill";
            lblSkill.Size = new Size(58, 23);
            lblSkill.TabIndex = 9;
            lblSkill.Text = "Skill";
            // 
            // lblSPD
            // 
            lblSPD.AutoSize = true;
            lblSPD.Font = new Font("Showcard Gothic", 10.8F, FontStyle.Italic);
            lblSPD.ForeColor = Color.DarkOrange;
            lblSPD.Location = new Point(19, 130);
            lblSPD.Name = "lblSPD";
            lblSPD.Size = new Size(44, 23);
            lblSPD.TabIndex = 8;
            lblSPD.Text = "SPD";
            // 
            // lblATK
            // 
            lblATK.AutoSize = true;
            lblATK.Font = new Font("Showcard Gothic", 10.8F, FontStyle.Italic);
            lblATK.ForeColor = Color.DarkOrange;
            lblATK.Location = new Point(20, 48);
            lblATK.Name = "lblATK";
            lblATK.Size = new Size(45, 23);
            lblATK.TabIndex = 6;
            lblATK.Text = "ATK";
            // 
            // lblHP
            // 
            lblHP.AutoSize = true;
            lblHP.Font = new Font("Showcard Gothic", 10.8F, FontStyle.Italic);
            lblHP.ForeColor = Color.DarkOrange;
            lblHP.Location = new Point(20, 10);
            lblHP.Name = "lblHP";
            lblHP.Size = new Size(36, 23);
            lblHP.TabIndex = 5;
            lblHP.Text = "HP";
            // 
            // btnSellect
            // 
            btnSellect.BackgroundImage = (Image)resources.GetObject("btnSellect.BackgroundImage");
            btnSellect.BackgroundImageLayout = ImageLayout.Stretch;
            btnSellect.Location = new Point(247, 364);
            btnSellect.Name = "btnSellect";
            btnSellect.Size = new Size(138, 43);
            btnSellect.TabIndex = 1;
            btnSellect.Click += btnSellect_Click;
            // 
            // button2
            // 
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.Location = new Point(409, 364);
            button2.Name = "button2";
            button2.Size = new Size(129, 43);
            button2.TabIndex = 0;
            button2.Click += button2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Book Antiqua", 36F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(208, 230, 255);
            label2.Location = new Point(167, 6);
            label2.Name = "label2";
            label2.Size = new Size(229, 72);
            label2.TabIndex = 3;
            label2.Text = "Wizard";
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(44, 74, 110);
            panel6.Controls.Add(pictureBox1);
            panel6.Controls.Add(label1);
            panel6.Location = new Point(10, 380);
            panel6.Name = "panel6";
            panel6.Size = new Size(366, 68);
            panel6.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(78, 57);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Book Antiqua", 19.8F, FontStyle.Bold | FontStyle.Italic);
            label1.ForeColor = Color.FromArgb(208, 230, 255);
            label1.Location = new Point(111, 10);
            label1.Name = "label1";
            label1.Size = new Size(86, 39);
            label1.TabIndex = 1;
            label1.Text = "Lord";
            // 
            // CharacterSelection
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1057, 579);
            Controls.Add(panel4);
            Name = "CharacterSelection";
            Text = "Character Selection";
            Load += CharacterSelection_Load;
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel2.ResumeLayout(false);
            pnlWizard.ResumeLayout(false);
            pnlWizard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbWizard).EndInit();
            pnlSamurai.ResumeLayout(false);
            pnlSamurai.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbSamurai).EndInit();
            pnlKitsune.ResumeLayout(false);
            pnlKitsune.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbKitsune).EndInit();
            pnlLord.ResumeLayout(false);
            pnlLord.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbLord).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbInfor).EndInit();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel4;
        private Label label5;
        private Panel panel2;
        private Panel pnlWizard;
        private PictureBox pbWizard;
        private Label lblWizardName;
        private Panel pnlSamurai;
        private PictureBox pbSamurai;
        private Label lblSamuraiName;
        private Panel pnlKitsune;
        private PictureBox pbKitsune;
        private Label lblKitsuneName;
        private Panel pnlLord;
        private PictureBox pbLord;
        private Label lblLordName;
        private Panel panel3;
        private Panel panel1;
        private PictureBox pbInfor;
        private Panel panel5;
        private Label lblDEF;
        private Label lblSkill;
        private Label lblSPD;
        private Label lblATK;
        private Label lblHP;
        private Button btnSellect;
        private Button button2;
        private Label label2;
        private Panel panel6;
        private PictureBox pictureBox1;
        private Label label1;
    }
}
