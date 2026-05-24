namespace BattleGame.Client.Forms
{
    partial class MapSelectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            comboBoxMap = new ComboBox();
            label1 = new Label();
            pictureBoxMap = new PictureBox();
            buttonSelect = new Button();
            buttonCancel = new Button();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).BeginInit();
            SuspendLayout();
            // 
            // comboBoxMap
            // 
            comboBoxMap.BackColor = Color.WhiteSmoke;
            comboBoxMap.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMap.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxMap.ForeColor = Color.Black;
            comboBoxMap.FormattingEnabled = true;
            comboBoxMap.Items.AddRange(new object[] { "Map 1", "Map 2", "Map 3" });
            comboBoxMap.Location = new Point(77, 74);
            comboBoxMap.Name = "comboBoxMap";
            comboBoxMap.Size = new Size(700, 36);
            comboBoxMap.TabIndex = 0;
            comboBoxMap.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Courier New", 26F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.PaleTurquoise;
            label1.Location = new Point(90, 9);
            label1.Name = "label1";
            label1.Size = new Size(674, 58);
            label1.TabIndex = 1;
            label1.Text = "CHOOSE ARENA";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxMap
            // 
            pictureBoxMap.BackColor = Color.FromArgb(24, 36, 68);
            pictureBoxMap.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxMap.Location = new Point(77, 133);
            pictureBoxMap.Name = "pictureBoxMap";
            pictureBoxMap.Size = new Size(700, 317);
            pictureBoxMap.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxMap.TabIndex = 2;
            pictureBoxMap.TabStop = false;
            pictureBoxMap.Click += pictureBoxMap_Click;
            // 
            // buttonSelect
            // 
            buttonSelect.BackColor = Color.FromArgb(44, 74, 110);
            buttonSelect.FlatAppearance.BorderColor = Color.PaleTurquoise;
            buttonSelect.FlatAppearance.BorderSize = 2;
            buttonSelect.FlatStyle = FlatStyle.Flat;
            buttonSelect.Font = new Font("Courier New", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonSelect.ForeColor = Color.FromArgb(255, 235, 156);
            buttonSelect.Location = new Point(77, 474);
            buttonSelect.Name = "buttonSelect";
            buttonSelect.Size = new Size(142, 46);
            buttonSelect.TabIndex = 3;
            buttonSelect.Text = "SELECT";
            buttonSelect.UseVisualStyleBackColor = false;
            buttonSelect.Click += buttonSelect_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = Color.FromArgb(44, 74, 110);
            buttonCancel.FlatAppearance.BorderColor = Color.PaleTurquoise;
            buttonCancel.FlatAppearance.BorderSize = 2;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Courier New", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonCancel.ForeColor = Color.FromArgb(255, 235, 156);
            buttonCancel.Location = new Point(635, 474);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(142, 46);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "CANCEL";
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(44, 74, 110);
            button1.FlatAppearance.BorderColor = Color.PaleTurquoise;
            button1.FlatAppearance.BorderSize = 2;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Courier New", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.FromArgb(255, 235, 156);
            button1.Location = new Point(348, 474);
            button1.Name = "button1";
            button1.Size = new Size(160, 46);
            button1.TabIndex = 5;
            button1.Text = "RANDOM";
            button1.UseVisualStyleBackColor = false;
            button1.Click += buttonRandom_Click;
            // 
            // MapSelectionForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(36, 58, 94);
            ClientSize = new Size(851, 548);
            Controls.Add(button1);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSelect);
            Controls.Add(pictureBoxMap);
            Controls.Add(label1);
            Controls.Add(comboBoxMap);
            Name = "MapSelectionForm";
            Text = "Map Selection";
            Load += MapSelectionForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ComboBox comboBoxMap;
        private Label label1;
        private PictureBox pictureBoxMap;
        private Button buttonSelect;
        private Button buttonCancel;
        private Button button1;
    }
}
