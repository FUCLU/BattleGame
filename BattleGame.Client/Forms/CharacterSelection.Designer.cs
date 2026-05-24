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
            btnInstruction = new Button();
            pnlCharListFrame = new Panel();
            flpnlSelChar = new FlowLayoutPanel();
            pnlCharacterSlotTemplate = new Panel();
            pbSlotTemplate = new PictureBox();
            lblSlotNameTemplate = new Label();
            lblSlotRoleTemplate = new Label();
            lblSlotHpTemplate = new Label();
            lblSlotSep1Template = new Label();
            lblSlotDmgTemplate = new Label();
            lblSlotSep2Template = new Label();
            lblSlotSpdTemplate = new Label();
            lblHeader = new Label();
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
            lblSkillIcon = new Label();
            lblSpdIcon = new Label();
            lblDefIcon = new Label();
            lblAtkIcon = new Label();
            lblHpIcon = new Label();
            lblDEF = new Label();
            lblSkill = new Label();
            lblSPD = new Label();
            lblATK = new Label();
            lblHP = new Label();
            btnSellect = new Button();
            button2 = new Button();
            label2 = new Label();
            panel4.SuspendLayout();
            pnlCharListFrame.SuspendLayout();
            flpnlSelChar.SuspendLayout();
            pnlCharacterSlotTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbSlotTemplate).BeginInit();
            panel3.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbInfor).BeginInit();
            panel5.SuspendLayout();
            panelSpdBack.SuspendLayout();
            panelDefBack.SuspendLayout();
            panelAtkBack.SuspendLayout();
            panelHpBack.SuspendLayout();
            SuspendLayout();
            // 
            // panel4
            // 
            panel4.BackColor = Color.FromArgb(36, 58, 94);
            panel4.BorderStyle = BorderStyle.Fixed3D;
            panel4.Controls.Add(btnInstruction);
            panel4.Controls.Add(pnlCharListFrame);
            panel4.Controls.Add(lblHeader);
            panel4.Controls.Add(panel3);
            panel4.Location = new Point(-2, -3);
            panel4.Name = "panel4";
            panel4.Size = new Size(1114, 668);
            panel4.TabIndex = 3;
            panel4.Paint += panel4_Paint_1;
            // 
            // btnInstruction
            // 
            btnInstruction.BackColor = Color.FromArgb(44, 74, 110);
            btnInstruction.Font = new Font("Courier New", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnInstruction.ForeColor = Color.Khaki;
            btnInstruction.Location = new Point(31, 557);
            btnInstruction.Name = "btnInstruction";
            btnInstruction.Size = new Size(405, 69);
            btnInstruction.TabIndex = 7;
            btnInstruction.Text = "INSTRUCTION";
            btnInstruction.UseVisualStyleBackColor = false;
            // 
            // pnlCharListFrame
            // 
            pnlCharListFrame.BackColor = Color.FromArgb(24, 36, 68);
            pnlCharListFrame.BorderStyle = BorderStyle.FixedSingle;
            pnlCharListFrame.Controls.Add(flpnlSelChar);
            pnlCharListFrame.Location = new Point(31, 59);
            pnlCharListFrame.Name = "pnlCharListFrame";
            pnlCharListFrame.Size = new Size(405, 488);
            pnlCharListFrame.TabIndex = 6;
            // 
            // flpnlSelChar
            // 
            flpnlSelChar.AutoScroll = true;
            flpnlSelChar.Controls.Add(pnlCharacterSlotTemplate);
            flpnlSelChar.FlowDirection = FlowDirection.TopDown;
            flpnlSelChar.Location = new Point(4, 4);
            flpnlSelChar.Name = "flpnlSelChar";
            flpnlSelChar.Padding = new Padding(8, 8, 2, 8);
            flpnlSelChar.Size = new Size(395, 479);
            flpnlSelChar.TabIndex = 4;
            flpnlSelChar.WrapContents = false;
            flpnlSelChar.Paint += flpnlSelChar_Paint;
            // 
            // pnlCharacterSlotTemplate
            // 
            pnlCharacterSlotTemplate.BackColor = Color.FromArgb(63, 110, 165);
            pnlCharacterSlotTemplate.Controls.Add(pbSlotTemplate);
            pnlCharacterSlotTemplate.Controls.Add(lblSlotNameTemplate);
            pnlCharacterSlotTemplate.Controls.Add(lblSlotRoleTemplate);
            pnlCharacterSlotTemplate.Controls.Add(lblSlotHpTemplate);
            pnlCharacterSlotTemplate.Controls.Add(lblSlotSep1Template);
            pnlCharacterSlotTemplate.Controls.Add(lblSlotDmgTemplate);
            pnlCharacterSlotTemplate.Controls.Add(lblSlotSep2Template);
            pnlCharacterSlotTemplate.Controls.Add(lblSlotSpdTemplate);
            pnlCharacterSlotTemplate.Cursor = Cursors.Hand;
            pnlCharacterSlotTemplate.Location = new Point(8, 8);
            pnlCharacterSlotTemplate.Margin = new Padding(0, 0, 0, 8);
            pnlCharacterSlotTemplate.Name = "pnlCharacterSlotTemplate";
            pnlCharacterSlotTemplate.Size = new Size(366, 92);
            pnlCharacterSlotTemplate.TabIndex = 0;
            // 
            // pbSlotTemplate
            // 
            pbSlotTemplate.BackColor = Color.Transparent;
            pbSlotTemplate.Location = new Point(8, 8);
            pbSlotTemplate.Name = "pbSlotTemplate";
            pbSlotTemplate.Size = new Size(76, 76);
            pbSlotTemplate.SizeMode = PictureBoxSizeMode.Zoom;
            pbSlotTemplate.TabIndex = 0;
            pbSlotTemplate.TabStop = false;
            // 
            // lblSlotNameTemplate
            // 
            lblSlotNameTemplate.BackColor = Color.Transparent;
            lblSlotNameTemplate.Cursor = Cursors.Hand;
            lblSlotNameTemplate.Font = new Font("Book Antiqua", 15F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            lblSlotNameTemplate.ForeColor = Color.FromArgb(255, 235, 156);
            lblSlotNameTemplate.Location = new Point(94, 4);
            lblSlotNameTemplate.Name = "lblSlotNameTemplate";
            lblSlotNameTemplate.Size = new Size(245, 34);
            lblSlotNameTemplate.TabIndex = 1;
            lblSlotNameTemplate.Text = "Golem";
            lblSlotNameTemplate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSlotRoleTemplate
            // 
            lblSlotRoleTemplate.BackColor = Color.Transparent;
            lblSlotRoleTemplate.Cursor = Cursors.Hand;
            lblSlotRoleTemplate.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSlotRoleTemplate.ForeColor = Color.FromArgb(160, 200, 240);
            lblSlotRoleTemplate.Location = new Point(94, 38);
            lblSlotRoleTemplate.Name = "lblSlotRoleTemplate";
            lblSlotRoleTemplate.Size = new Size(245, 24);
            lblSlotRoleTemplate.TabIndex = 2;
            lblSlotRoleTemplate.Text = "🗡 Fighter";
            lblSlotRoleTemplate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSlotHpTemplate
            // 
            lblSlotHpTemplate.BackColor = Color.Transparent;
            lblSlotHpTemplate.Cursor = Cursors.Hand;
            lblSlotHpTemplate.Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSlotHpTemplate.ForeColor = Color.FromArgb(120, 160, 200);
            lblSlotHpTemplate.Location = new Point(94, 62);
            lblSlotHpTemplate.Name = "lblSlotHpTemplate";
            lblSlotHpTemplate.Size = new Size(58, 24);
            lblSlotHpTemplate.TabIndex = 3;
            lblSlotHpTemplate.Text = "HP:165";
            lblSlotHpTemplate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSlotSep1Template
            // 
            lblSlotSep1Template.BackColor = Color.Transparent;
            lblSlotSep1Template.Cursor = Cursors.Hand;
            lblSlotSep1Template.Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSlotSep1Template.ForeColor = Color.FromArgb(120, 160, 200);
            lblSlotSep1Template.Location = new Point(154, 62);
            lblSlotSep1Template.Name = "lblSlotSep1Template";
            lblSlotSep1Template.Size = new Size(16, 24);
            lblSlotSep1Template.TabIndex = 4;
            lblSlotSep1Template.Text = "|";
            lblSlotSep1Template.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSlotDmgTemplate
            // 
            lblSlotDmgTemplate.BackColor = Color.Transparent;
            lblSlotDmgTemplate.Cursor = Cursors.Hand;
            lblSlotDmgTemplate.Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSlotDmgTemplate.ForeColor = Color.FromArgb(120, 160, 200);
            lblSlotDmgTemplate.Location = new Point(172, 62);
            lblSlotDmgTemplate.Name = "lblSlotDmgTemplate";
            lblSlotDmgTemplate.Size = new Size(62, 24);
            lblSlotDmgTemplate.TabIndex = 5;
            lblSlotDmgTemplate.Text = "DMG:22";
            lblSlotDmgTemplate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSlotSep2Template
            // 
            lblSlotSep2Template.BackColor = Color.Transparent;
            lblSlotSep2Template.Cursor = Cursors.Hand;
            lblSlotSep2Template.Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSlotSep2Template.ForeColor = Color.FromArgb(120, 160, 200);
            lblSlotSep2Template.Location = new Point(238, 62);
            lblSlotSep2Template.Name = "lblSlotSep2Template";
            lblSlotSep2Template.Size = new Size(16, 24);
            lblSlotSep2Template.TabIndex = 6;
            lblSlotSep2Template.Text = "|";
            lblSlotSep2Template.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSlotSpdTemplate
            // 
            lblSlotSpdTemplate.BackColor = Color.Transparent;
            lblSlotSpdTemplate.Cursor = Cursors.Hand;
            lblSlotSpdTemplate.Font = new Font("Consolas", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSlotSpdTemplate.ForeColor = Color.FromArgb(120, 160, 200);
            lblSlotSpdTemplate.Location = new Point(256, 62);
            lblSlotSpdTemplate.Name = "lblSlotSpdTemplate";
            lblSlotSpdTemplate.Size = new Size(70, 24);
            lblSlotSpdTemplate.TabIndex = 7;
            lblSlotSpdTemplate.Text = "SPD:165";
            lblSlotSpdTemplate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblHeader
            // 
            lblHeader.AutoSize = true;
            lblHeader.BackColor = Color.Transparent;
            lblHeader.Font = new Font("Courier New", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHeader.ForeColor = Color.PaleTurquoise;
            lblHeader.Location = new Point(298, 6);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(494, 46);
            lblHeader.TabIndex = 3;
            lblHeader.Text = "⚔️ Chọn nhân vật ⚔️ ";
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
            panel3.Location = new Point(462, 59);
            panel3.Name = "panel3";
            panel3.Size = new Size(585, 567);
            panel3.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(pbInfor);
            panel1.Location = new Point(185, 15);
            panel1.Name = "panel1";
            panel1.Size = new Size(210, 177);
            panel1.TabIndex = 8;
            // 
            // pbInfor
            // 
            pbInfor.BackColor = Color.Transparent;
            pbInfor.Location = new Point(11, 7);
            pbInfor.Name = "pbInfor";
            pbInfor.Size = new Size(188, 163);
            pbInfor.SizeMode = PictureBoxSizeMode.Zoom;
            pbInfor.TabIndex = 2;
            pbInfor.TabStop = false;
            // 
            // panel5
            // 
            panel5.BackColor = Color.Transparent;
            panel5.Controls.Add(lblSpdValue);
            panel5.Controls.Add(lblDefValue);
            panel5.Controls.Add(lblAtkValue);
            panel5.Controls.Add(lblHpValue);
            panel5.Controls.Add(panelSpdBack);
            panel5.Controls.Add(panelDefBack);
            panel5.Controls.Add(panelAtkBack);
            panel5.Controls.Add(panelHpBack);
            panel5.Controls.Add(lblSkillIcon);
            panel5.Controls.Add(lblSpdIcon);
            panel5.Controls.Add(lblDefIcon);
            panel5.Controls.Add(lblAtkIcon);
            panel5.Controls.Add(lblHpIcon);
            panel5.Controls.Add(lblDEF);
            panel5.Controls.Add(lblSkill);
            panel5.Controls.Add(lblSPD);
            panel5.Controls.Add(lblATK);
            panel5.Controls.Add(lblHP);
            panel5.Location = new Point(17, 265);
            panel5.Name = "panel5";
            panel5.Size = new Size(531, 221);
            panel5.TabIndex = 7;
            // 
            // lblSpdValue
            // 
            lblSpdValue.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSpdValue.ForeColor = Color.WhiteSmoke;
            lblSpdValue.Location = new Point(468, 135);
            lblSpdValue.Name = "lblSpdValue";
            lblSpdValue.Size = new Size(54, 23);
            lblSpdValue.TabIndex = 18;
            lblSpdValue.Text = "300";
            lblSpdValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblDefValue
            // 
            lblDefValue.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDefValue.ForeColor = Color.WhiteSmoke;
            lblDefValue.Location = new Point(468, 95);
            lblDefValue.Name = "lblDefValue";
            lblDefValue.Size = new Size(54, 23);
            lblDefValue.TabIndex = 17;
            lblDefValue.Text = "20";
            lblDefValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblAtkValue
            // 
            lblAtkValue.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAtkValue.ForeColor = Color.WhiteSmoke;
            lblAtkValue.Location = new Point(468, 54);
            lblAtkValue.Name = "lblAtkValue";
            lblAtkValue.Size = new Size(54, 23);
            lblAtkValue.TabIndex = 16;
            lblAtkValue.Text = "30";
            lblAtkValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblHpValue
            // 
            lblHpValue.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblHpValue.ForeColor = Color.WhiteSmoke;
            lblHpValue.Location = new Point(468, 15);
            lblHpValue.Name = "lblHpValue";
            lblHpValue.Size = new Size(54, 23);
            lblHpValue.TabIndex = 15;
            lblHpValue.Text = "200";
            lblHpValue.TextAlign = ContentAlignment.MiddleLeft;
            lblHpValue.Click += lblHpValue_Click;
            // 
            // panelSpdBack
            // 
            panelSpdBack.BackColor = Color.FromArgb(62, 47, 37);
            panelSpdBack.BorderStyle = BorderStyle.FixedSingle;
            panelSpdBack.Controls.Add(panelSpdFill);
            panelSpdBack.Location = new Point(139, 141);
            panelSpdBack.Name = "panelSpdBack";
            panelSpdBack.Size = new Size(315, 18);
            panelSpdBack.TabIndex = 14;
            // 
            // panelSpdFill
            // 
            panelSpdFill.BackColor = Color.FromArgb(106, 207, 124);
            panelSpdFill.Dock = DockStyle.Left;
            panelSpdFill.Location = new Point(0, 0);
            panelSpdFill.Name = "panelSpdFill";
            panelSpdFill.Size = new Size(247, 16);
            panelSpdFill.TabIndex = 0;
            // 
            // panelDefBack
            // 
            panelDefBack.BackColor = Color.FromArgb(62, 47, 37);
            panelDefBack.BorderStyle = BorderStyle.FixedSingle;
            panelDefBack.Controls.Add(panelDefFill);
            panelDefBack.Location = new Point(139, 101);
            panelDefBack.Name = "panelDefBack";
            panelDefBack.Size = new Size(315, 18);
            panelDefBack.TabIndex = 13;
            // 
            // panelDefFill
            // 
            panelDefFill.BackColor = Color.FromArgb(102, 159, 229);
            panelDefFill.Dock = DockStyle.Left;
            panelDefFill.Location = new Point(0, 0);
            panelDefFill.Name = "panelDefFill";
            panelDefFill.Size = new Size(189, 16);
            panelDefFill.TabIndex = 0;
            // 
            // panelAtkBack
            // 
            panelAtkBack.BackColor = Color.FromArgb(62, 47, 37);
            panelAtkBack.BorderStyle = BorderStyle.FixedSingle;
            panelAtkBack.Controls.Add(panelAtkFill);
            panelAtkBack.Location = new Point(139, 60);
            panelAtkBack.Name = "panelAtkBack";
            panelAtkBack.Size = new Size(315, 18);
            panelAtkBack.TabIndex = 12;
            // 
            // panelAtkFill
            // 
            panelAtkFill.BackColor = Color.FromArgb(238, 179, 73);
            panelAtkFill.Dock = DockStyle.Left;
            panelAtkFill.Location = new Point(0, 0);
            panelAtkFill.Name = "panelAtkFill";
            panelAtkFill.Size = new Size(231, 16);
            panelAtkFill.TabIndex = 0;
            // 
            // panelHpBack
            // 
            panelHpBack.BackColor = Color.FromArgb(62, 47, 37);
            panelHpBack.BorderStyle = BorderStyle.FixedSingle;
            panelHpBack.Controls.Add(panelHpFill);
            panelHpBack.Location = new Point(139, 22);
            panelHpBack.Name = "panelHpBack";
            panelHpBack.Size = new Size(315, 18);
            panelHpBack.TabIndex = 11;
            // 
            // panelHpFill
            // 
            panelHpFill.BackColor = Color.FromArgb(218, 73, 79);
            panelHpFill.Dock = DockStyle.Left;
            panelHpFill.Location = new Point(0, 0);
            panelHpFill.Name = "panelHpFill";
            panelHpFill.Size = new Size(197, 16);
            panelHpFill.TabIndex = 0;
            // 
            // lblSkillIcon
            // 
            lblSkillIcon.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSkillIcon.ForeColor = Color.Khaki;
            lblSkillIcon.Location = new Point(42, 181);
            lblSkillIcon.Name = "lblSkillIcon";
            lblSkillIcon.Size = new Size(20, 20);
            lblSkillIcon.TabIndex = 23;
            lblSkillIcon.Text = "✥";
            lblSkillIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSpdIcon
            // 
            lblSpdIcon.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSpdIcon.ForeColor = Color.Khaki;
            lblSpdIcon.Location = new Point(42, 140);
            lblSpdIcon.Name = "lblSpdIcon";
            lblSpdIcon.Size = new Size(20, 20);
            lblSpdIcon.TabIndex = 22;
            lblSpdIcon.Text = "✦";
            lblSpdIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblDefIcon
            // 
            lblDefIcon.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDefIcon.ForeColor = Color.Khaki;
            lblDefIcon.Location = new Point(42, 99);
            lblDefIcon.Name = "lblDefIcon";
            lblDefIcon.Size = new Size(20, 20);
            lblDefIcon.TabIndex = 21;
            lblDefIcon.Text = "♦";
            lblDefIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAtkIcon
            // 
            lblAtkIcon.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAtkIcon.ForeColor = Color.Khaki;
            lblAtkIcon.Location = new Point(42, 57);
            lblAtkIcon.Name = "lblAtkIcon";
            lblAtkIcon.Size = new Size(20, 20);
            lblAtkIcon.TabIndex = 20;
            lblAtkIcon.Text = "⚔";
            lblAtkIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblHpIcon
            // 
            lblHpIcon.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHpIcon.ForeColor = Color.Khaki;
            lblHpIcon.Location = new Point(42, 19);
            lblHpIcon.Name = "lblHpIcon";
            lblHpIcon.Size = new Size(20, 20);
            lblHpIcon.TabIndex = 19;
            lblHpIcon.Text = "♥";
            lblHpIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblDEF
            // 
            lblDEF.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDEF.ForeColor = Color.Khaki;
            lblDEF.Location = new Point(68, 99);
            lblDEF.Name = "lblDEF";
            lblDEF.Size = new Size(70, 20);
            lblDEF.TabIndex = 10;
            lblDEF.Text = "DEF:";
            lblDEF.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSkill
            // 
            lblSkill.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSkill.ForeColor = Color.Khaki;
            lblSkill.Location = new Point(68, 181);
            lblSkill.Name = "lblSkill";
            lblSkill.Size = new Size(420, 20);
            lblSkill.TabIndex = 9;
            lblSkill.Text = "SKILL : Crystal Crush / Crystal Burst";
            lblSkill.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSPD
            // 
            lblSPD.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSPD.ForeColor = Color.Khaki;
            lblSPD.Location = new Point(68, 140);
            lblSPD.Name = "lblSPD";
            lblSPD.Size = new Size(70, 20);
            lblSPD.TabIndex = 8;
            lblSPD.Text = "SPD:";
            lblSPD.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblATK
            // 
            lblATK.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblATK.ForeColor = Color.Khaki;
            lblATK.Location = new Point(68, 57);
            lblATK.Name = "lblATK";
            lblATK.Size = new Size(70, 20);
            lblATK.TabIndex = 6;
            lblATK.Text = "ATK:";
            lblATK.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblHP
            // 
            lblHP.Font = new Font("Bookman Old Style", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHP.ForeColor = Color.Khaki;
            lblHP.Location = new Point(68, 19);
            lblHP.Name = "lblHP";
            lblHP.Size = new Size(70, 20);
            lblHP.TabIndex = 5;
            lblHP.Text = "HP:";
            lblHP.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnSellect
            // 
            btnSellect.BackgroundImage = (Image)resources.GetObject("btnSellect.BackgroundImage");
            btnSellect.BackgroundImageLayout = ImageLayout.Stretch;
            btnSellect.FlatStyle = FlatStyle.Popup;
            btnSellect.Location = new Point(400, 492);
            btnSellect.Name = "btnSellect";
            btnSellect.Size = new Size(127, 54);
            btnSellect.TabIndex = 1;
            btnSellect.Click += btnSellect_Click;
            // 
            // button2
            // 
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.FlatStyle = FlatStyle.Popup;
            button2.Location = new Point(50, 501);
            button2.Name = "button2";
            button2.Size = new Size(113, 45);
            button2.TabIndex = 0;
            button2.Click += button2_Click;
            // 
            // label2
            // 
            label2.Font = new Font("Book Antiqua", 28.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Khaki;
            label2.Location = new Point(0, 195);
            label2.Name = "label2";
            label2.Size = new Size(581, 60);
            label2.TabIndex = 3;
            label2.Text = "Wizard";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.Click += label2_Click;
            // 
            // CharacterSelection
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.SteelBlue;
            ClientSize = new Size(1069, 652);
            Controls.Add(panel4);
            Name = "CharacterSelection";
            Text = "Character Selection";
            Load += CharacterSelection_Load;
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            pnlCharListFrame.ResumeLayout(false);
            flpnlSelChar.ResumeLayout(false);
            pnlCharacterSlotTemplate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbSlotTemplate).EndInit();
            panel3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbInfor).EndInit();
            panel5.ResumeLayout(false);
            panelSpdBack.ResumeLayout(false);
            panelDefBack.ResumeLayout(false);
            panelAtkBack.ResumeLayout(false);
            panelHpBack.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel4;
        private Panel pnlCharListFrame;
        private Panel pnlCharacterSlotTemplate;
        private PictureBox pbSlotTemplate;
        private Label lblSlotNameTemplate;
        private Label lblSlotRoleTemplate;
        private Label lblSlotHpTemplate;
        private Label lblSlotSep1Template;
        private Label lblSlotDmgTemplate;
        private Label lblSlotSep2Template;
        private Label lblSlotSpdTemplate;
        private Label lblHeader;
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
        private Label lblHpIcon;
        private Label lblAtkIcon;
        private Label lblDefIcon;
        private Label lblSpdIcon;
        private Label lblSkillIcon;
        private Label lblDEF;
        private Label lblSkill;
        private Label lblSPD;
        private Label lblATK;
        private Label lblHP;
        private Button btnSellect;
        private Button button2;
        private Button btnInstruction;
        private Label label2;
        private Label lblDefValue;
        private Label lblAtkValue;
        private Label lblHpValue;
        private Label lblSpdValue;
        private FlowLayoutPanel flpnlSelChar;
    }
}
