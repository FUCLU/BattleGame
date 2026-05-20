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
            lblSpdValue = new Label();
            lblDefValue = new Label();
            lblAtkValue = new Label();
            lblHpValue = new Label();
            panelSpdBack = new Panel();
            panelSpdFill = new Panel();
            panelDefBack = new Panel();
            panelDefFill = new Panel();
            panelAtkBack = new Panel();
            panelAtkFill = new Panel();
            panelHpBack = new Panel();
            panelHpFill = new Panel();
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
            panelSpdBack.SuspendLayout();
            panelDefBack.SuspendLayout();
            panelAtkBack.SuspendLayout();
            panelHpBack.SuspendLayout();
            panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel4
            // 
            panel4.BackColor = Color.FromArgb(36, 58, 94);
            panel4.BorderStyle = BorderStyle.Fixed3D;
            panel4.Controls.Add(label5);
            panel4.Controls.Add(panel2);
            panel4.Controls.Add(panel3);
            panel4.Location = new Point(22, 23);
            panel4.Name = "panel4";
            panel4.Size = new Size(1078, 543);
            panel4.TabIndex = 3;
            panel4.Paint += panel4_Paint_1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.WhiteSmoke;
            label5.Location = new Point(315, 10);
            label5.Name = "label5";
            label5.Size = new Size(396, 50);
            label5.TabIndex = 3;
            label5.Text = "⚔️ Chọn nhân vật⚔️ ";
            // 
            // panel2
            // 
            panel2.AutoScroll = true;
            panel2.BackColor = Color.FromArgb(28, 45, 72);
            panel2.BackgroundImageLayout = ImageLayout.Stretch;
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(pnlWizard);
            panel2.Controls.Add(pnlSamurai);
            panel2.Controls.Add(pnlKitsune);
            panel2.Controls.Add(pnlLord);
            panel2.Location = new Point(16, 59);
            panel2.Name = "panel2";
            panel2.Size = new Size(398, 463);
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
            lblWizardName.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblWizardName.ForeColor = Color.WhiteSmoke;
            lblWizardName.Location = new Point(110, 15);
            lblWizardName.Name = "lblWizardName";
            lblWizardName.Size = new Size(118, 41);
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
            lblSamuraiName.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSamuraiName.ForeColor = Color.WhiteSmoke;
            lblSamuraiName.Location = new Point(110, 12);
            lblSamuraiName.Name = "lblSamuraiName";
            lblSamuraiName.Size = new Size(133, 41);
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
            lblKitsuneName.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKitsuneName.ForeColor = Color.WhiteSmoke;
            lblKitsuneName.Location = new Point(110, 11);
            lblKitsuneName.Name = "lblKitsuneName";
            lblKitsuneName.Size = new Size(123, 41);
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
            lblLordName.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLordName.ForeColor = Color.WhiteSmoke;
            lblLordName.Location = new Point(110, 9);
            lblLordName.Name = "lblLordName";
            lblLordName.Size = new Size(82, 41);
            lblLordName.TabIndex = 1;
            lblLordName.Text = "Lord";
            lblLordName.Click += CharacterPanel_Click;
            // 
            // panel3
            // 
            panel3.BackColor = Color.FromArgb(24, 36, 68);
            panel3.BorderStyle = BorderStyle.Fixed3D;
            panel3.Controls.Add(panel1);
            panel3.Controls.Add(panel5);
            panel3.Controls.Add(btnSellect);
            panel3.Controls.Add(button2);
            panel3.Controls.Add(label2);
            panel3.Location = new Point(435, 59);
            panel3.Name = "panel3";
            panel3.Size = new Size(621, 463);
            panel3.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(15, 28, 46);
            panel1.Controls.Add(pbInfor);
            panel1.Location = new Point(11, 107);
            panel1.Name = "panel1";
            panel1.Size = new Size(230, 212);
            panel1.TabIndex = 8;
            // 
            // pbInfor
            // 
            pbInfor.BackColor = Color.Transparent;
            pbInfor.Image = (Image)resources.GetObject("pbInfor.Image");
            pbInfor.Location = new Point(3, 11);
            pbInfor.Name = "pbInfor";
            pbInfor.Size = new Size(227, 201);
            pbInfor.SizeMode = PictureBoxSizeMode.Zoom;
            pbInfor.TabIndex = 2;
            pbInfor.TabStop = false;
            // 
            // panel5
            // 
            panel5.BackColor = Color.FromArgb(34, 58, 92);
            panel5.Controls.Add(lblSpdValue);
            panel5.Controls.Add(lblDefValue);
            panel5.Controls.Add(lblAtkValue);
            panel5.Controls.Add(lblHpValue);
            panel5.Controls.Add(panelSpdBack);
            panel5.Controls.Add(panelDefBack);
            panel5.Controls.Add(panelAtkBack);
            panel5.Controls.Add(panelHpBack);
            panel5.Controls.Add(lblDEF);
            panel5.Controls.Add(lblSkill);
            panel5.Controls.Add(lblSPD);
            panel5.Controls.Add(lblATK);
            panel5.Controls.Add(lblHP);
            panel5.Location = new Point(247, 107);
            panel5.Name = "panel5";
            panel5.Size = new Size(351, 215);
            panel5.TabIndex = 7;
            // 
            // lblSpdValue
            // 
            lblSpdValue.AutoSize = true;
            lblSpdValue.ForeColor = Color.WhiteSmoke;
            lblSpdValue.Location = new Point(299, 131);
            lblSpdValue.Name = "lblSpdValue";
            lblSpdValue.Size = new Size(33, 20);
            lblSpdValue.TabIndex = 18;
            lblSpdValue.Text = "120";
            // 
            // lblDefValue
            // 
            lblDefValue.AutoSize = true;
            lblDefValue.ForeColor = Color.WhiteSmoke;
            lblDefValue.Location = new Point(299, 92);
            lblDefValue.Name = "lblDefValue";
            lblDefValue.Size = new Size(33, 20);
            lblDefValue.TabIndex = 17;
            lblDefValue.Text = "120";
            // 
            // lblAtkValue
            // 
            lblAtkValue.AutoSize = true;
            lblAtkValue.ForeColor = Color.WhiteSmoke;
            lblAtkValue.Location = new Point(299, 48);
            lblAtkValue.Name = "lblAtkValue";
            lblAtkValue.Size = new Size(33, 20);
            lblAtkValue.TabIndex = 16;
            lblAtkValue.Text = "120";
            // 
            // lblHpValue
            // 
            lblHpValue.AutoSize = true;
            lblHpValue.ForeColor = Color.WhiteSmoke;
            lblHpValue.Location = new Point(299, 13);
            lblHpValue.Name = "lblHpValue";
            lblHpValue.Size = new Size(33, 20);
            lblHpValue.TabIndex = 15;
            lblHpValue.Text = "120";
            // 
            // panelSpdBack
            // 
            panelSpdBack.BackColor = Color.FromArgb(30, 30, 30);
            panelSpdBack.BorderStyle = BorderStyle.Fixed3D;
            panelSpdBack.Controls.Add(panelSpdFill);
            panelSpdBack.Location = new Point(98, 133);
            panelSpdBack.Name = "panelSpdBack";
            panelSpdBack.Size = new Size(190, 16);
            panelSpdBack.TabIndex = 14;
            // 
            // panelSpdFill
            // 
            panelSpdFill.BackColor = Color.White;
            panelSpdFill.Dock = DockStyle.Left;
            panelSpdFill.Location = new Point(0, 0);
            panelSpdFill.Name = "panelSpdFill";
            panelSpdFill.Size = new Size(188, 12);
            panelSpdFill.TabIndex = 0;
            // 
            // panelDefBack
            // 
            panelDefBack.BackColor = Color.FromArgb(30, 30, 30);
            panelDefBack.BorderStyle = BorderStyle.Fixed3D;
            panelDefBack.Controls.Add(panelDefFill);
            panelDefBack.Location = new Point(98, 92);
            panelDefBack.Name = "panelDefBack";
            panelDefBack.Size = new Size(190, 16);
            panelDefBack.TabIndex = 13;
            // 
            // panelDefFill
            // 
            panelDefFill.BackColor = Color.White;
            panelDefFill.Dock = DockStyle.Left;
            panelDefFill.Location = new Point(0, 0);
            panelDefFill.Name = "panelDefFill";
            panelDefFill.Size = new Size(188, 12);
            panelDefFill.TabIndex = 0;
            // 
            // panelAtkBack
            // 
            panelAtkBack.BackColor = Color.FromArgb(30, 30, 30);
            panelAtkBack.BorderStyle = BorderStyle.Fixed3D;
            panelAtkBack.Controls.Add(panelAtkFill);
            panelAtkBack.Location = new Point(98, 52);
            panelAtkBack.Name = "panelAtkBack";
            panelAtkBack.Size = new Size(190, 16);
            panelAtkBack.TabIndex = 12;
            // 
            // panelAtkFill
            // 
            panelAtkFill.BackColor = Color.White;
            panelAtkFill.Dock = DockStyle.Left;
            panelAtkFill.Location = new Point(0, 0);
            panelAtkFill.Name = "panelAtkFill";
            panelAtkFill.Size = new Size(188, 12);
            panelAtkFill.TabIndex = 0;
            // 
            // panelHpBack
            // 
            panelHpBack.BackColor = Color.FromArgb(30, 30, 30);
            panelHpBack.BorderStyle = BorderStyle.Fixed3D;
            panelHpBack.Controls.Add(panelHpFill);
            panelHpBack.Location = new Point(98, 14);
            panelHpBack.Name = "panelHpBack";
            panelHpBack.Size = new Size(190, 16);
            panelHpBack.TabIndex = 11;
            // 
            // panelHpFill
            // 
            panelHpFill.BackColor = Color.White;
            panelHpFill.Dock = DockStyle.Left;
            panelHpFill.Location = new Point(0, 0);
            panelHpFill.Name = "panelHpFill";
            panelHpFill.Size = new Size(188, 12);
            panelHpFill.TabIndex = 0;
            // 
            // lblDEF
            // 
            lblDEF.AutoSize = true;
            lblDEF.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDEF.ForeColor = Color.Gold;
            lblDEF.Location = new Point(20, 88);
            lblDEF.Name = "lblDEF";
            lblDEF.Size = new Size(44, 25);
            lblDEF.TabIndex = 10;
            lblDEF.Text = "DEF";
            // 
            // lblSkill
            // 
            lblSkill.AutoSize = true;
            lblSkill.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSkill.ForeColor = Color.Gold;
            lblSkill.Location = new Point(18, 174);
            lblSkill.Name = "lblSkill";
            lblSkill.Size = new Size(47, 25);
            lblSkill.TabIndex = 9;
            lblSkill.Text = "Skill";
            // 
            // lblSPD
            // 
            lblSPD.AutoSize = true;
            lblSPD.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSPD.ForeColor = Color.Gold;
            lblSPD.Location = new Point(19, 129);
            lblSPD.Name = "lblSPD";
            lblSPD.Size = new Size(46, 25);
            lblSPD.TabIndex = 8;
            lblSPD.Text = "SPD";
            // 
            // lblATK
            // 
            lblATK.AutoSize = true;
            lblATK.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblATK.ForeColor = Color.Gold;
            lblATK.Location = new Point(20, 46);
            lblATK.Name = "lblATK";
            lblATK.Size = new Size(47, 25);
            lblATK.TabIndex = 6;
            lblATK.Text = "ATK";
            // 
            // lblHP
            // 
            lblHP.AutoSize = true;
            lblHP.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHP.ForeColor = Color.Gold;
            lblHP.Location = new Point(20, 8);
            lblHP.Name = "lblHP";
            lblHP.Size = new Size(37, 25);
            lblHP.TabIndex = 5;
            lblHP.Text = "HP";
            // 
            // btnSellect
            // 
            btnSellect.BackgroundImage = (Image)resources.GetObject("btnSellect.BackgroundImage");
            btnSellect.BackgroundImageLayout = ImageLayout.Stretch;
            btnSellect.FlatStyle = FlatStyle.Popup;
            btnSellect.Location = new Point(267, 381);
            btnSellect.Name = "btnSellect";
            btnSellect.Size = new Size(160, 54);
            btnSellect.TabIndex = 1;
            btnSellect.Click += btnSellect_Click;
            // 
            // button2
            // 
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.FlatStyle = FlatStyle.Popup;
            button2.Location = new Point(457, 381);
            button2.Name = "button2";
            button2.Size = new Size(141, 49);
            button2.TabIndex = 0;
            button2.Click += button2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 28F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.WhiteSmoke;
            label2.Location = new Point(212, 6);
            label2.Name = "label2";
            label2.Size = new Size(183, 62);
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
            BackColor = Color.SteelBlue;
            ClientSize = new Size(1112, 592);
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
            panelSpdBack.ResumeLayout(false);
            panelDefBack.ResumeLayout(false);
            panelAtkBack.ResumeLayout(false);
            panelHpBack.ResumeLayout(false);
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
        private Panel panelAtkBack;
        private Panel panelAtkFill;
        private Panel panelHpBack;
        private Panel panelHpFill;
        private Panel panelDefBack;
        private Panel panelDefFill;
        private Panel panelSpdBack;
        private Panel panelSpdFill;
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
        private Label lblDefValue;
        private Label lblAtkValue;
        private Label lblHpValue;
        private Label lblSpdValue;
    }
}
