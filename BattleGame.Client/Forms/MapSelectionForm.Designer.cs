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
            comboBoxMap = new ComboBox();
            label1 = new Label();
            pictureBoxMap = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).BeginInit();
            SuspendLayout();
            // 
            // comboBoxMap
            // 
            comboBoxMap.FormattingEnabled = true;
            comboBoxMap.Items.AddRange(new object[] { "Map 1", "Map 2", "Map 3", "" });
            comboBoxMap.Location = new Point(43, 53);
            comboBoxMap.Name = "comboBoxMap";
            comboBoxMap.Size = new Size(653, 28);
            comboBoxMap.TabIndex = 0;
            comboBoxMap.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Goudy Stout", 13.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.CornflowerBlue;
            label1.Location = new Point(162, 9);
            label1.Name = "label1";
            label1.Size = new Size(462, 31);
            label1.TabIndex = 1;
            label1.Text = "Choose Backround";
            // 
            // pictureBoxMap
            // 
            pictureBoxMap.BackColor = Color.CadetBlue;
            pictureBoxMap.Location = new Point(75, 89);
            pictureBoxMap.Name = "pictureBoxMap";
            pictureBoxMap.Size = new Size(565, 271);
            pictureBoxMap.TabIndex = 2;
            pictureBoxMap.TabStop = false;
            pictureBoxMap.Click += pictureBoxMap_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.BlanchedAlmond;
            button1.FlatAppearance.BorderColor = Color.YellowGreen;
            button1.FlatAppearance.BorderSize = 2;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Book Antiqua", 13.8F, FontStyle.Bold);
            button1.Location = new Point(140, 372);
            button1.Name = "button1";
            button1.Size = new Size(127, 39);
            button1.TabIndex = 3;
            button1.Text = "SELECT";
            button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackColor = Color.IndianRed;
            button2.FlatAppearance.BorderColor = Color.YellowGreen;
            button2.FlatAppearance.BorderSize = 2;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Book Antiqua", 13.8F, FontStyle.Bold);
            button2.Location = new Point(423, 372);
            button2.Name = "button2";
            button2.Size = new Size(127, 39);
            button2.TabIndex = 4;
            button2.Text = "CANCEL";
            button2.UseVisualStyleBackColor = false;
            // 
            // MapSelectionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkSlateGray;
            ClientSize = new Size(732, 423);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(pictureBoxMap);
            Controls.Add(label1);
            Controls.Add(comboBoxMap);
            Name = "MapSelectionForm";
            Text = "MapSelectionForm";
            Load += MapSelectionForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxMap).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBoxMap;
        private Label label1;
        private PictureBox pictureBoxMap;
        private Button button1;
        private Button button2;
    }
}