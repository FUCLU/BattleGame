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
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).BeginInit();
            SuspendLayout();
            // 
            // comboBoxMap
            // 
            comboBoxMap.BackColor = Color.FromArgb(31, 47, 86);
            comboBoxMap.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMap.Font = new Font("Showcard Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxMap.ForeColor = Color.SandyBrown;
            comboBoxMap.FormattingEnabled = true;
            comboBoxMap.Items.AddRange(new object[] { "Map 1", "Map 2", "Map 3" });
            comboBoxMap.Location = new Point(48, 93);
            comboBoxMap.Name = "comboBoxMap";
            comboBoxMap.Size = new Size(653, 34);
            comboBoxMap.TabIndex = 0;
            comboBoxMap.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Goudy Stout", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.MidnightBlue;
            label1.Location = new Point(23, 21);
            label1.Name = "label1";
            label1.Size = new Size(717, 47);
            label1.TabIndex = 1;
            label1.Text = "Choose Background";
            // 
            // pictureBoxMap
            // 
            pictureBoxMap.BackColor = Color.CadetBlue;
            pictureBoxMap.Location = new Point(48, 138);
            pictureBoxMap.Name = "pictureBoxMap";
            pictureBoxMap.Size = new Size(653, 310);
            pictureBoxMap.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxMap.TabIndex = 2;
            pictureBoxMap.TabStop = false;
            pictureBoxMap.Click += pictureBoxMap_Click;
            // 
            // buttonSelect
            // 
            buttonSelect.BackColor = Color.BlanchedAlmond;
            buttonSelect.FlatAppearance.BorderColor = Color.YellowGreen;
            buttonSelect.FlatAppearance.BorderSize = 2;
            buttonSelect.FlatStyle = FlatStyle.Flat;
            buttonSelect.Font = new Font("Book Antiqua", 13.8F, FontStyle.Bold);
            buttonSelect.Location = new Point(164, 479);
            buttonSelect.Name = "buttonSelect";
            buttonSelect.Size = new Size(127, 39);
            buttonSelect.TabIndex = 3;
            buttonSelect.Text = "SELECT";
            buttonSelect.UseVisualStyleBackColor = false;
            buttonSelect.Click += buttonSelect_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = Color.IndianRed;
            buttonCancel.FlatAppearance.BorderColor = Color.YellowGreen;
            buttonCancel.FlatAppearance.BorderSize = 2;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Book Antiqua", 13.8F, FontStyle.Bold);
            buttonCancel.Location = new Point(467, 479);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(127, 39);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "CANCEL";
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // MapSelectionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SteelBlue;
            ClientSize = new Size(753, 538);
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
            PerformLayout();
        }

        #endregion

        private ComboBox comboBoxMap;
        private Label label1;
        private PictureBox pictureBoxMap;
        private Button buttonSelect;
        private Button buttonCancel;
    }
}