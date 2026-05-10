namespace BattleGame.Client.Forms
{
    partial class DungeonMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DungeonMode));
            btnBack = new Button();
            btnStage1 = new Button();
            btnStage2 = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.IndianRed;
            btnBack.BackgroundImage = (Image)resources.GetObject("btnBack.BackgroundImage");
            btnBack.BackgroundImageLayout = ImageLayout.Stretch;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.ForeColor = Color.RosyBrown;
            btnBack.Location = new Point(681, 400);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(120, 48);
            btnBack.TabIndex = 2;
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += btnBack_Click;
            // 
            // btnStage1
            // 
            btnStage1.BackColor = Color.Transparent;
            btnStage1.BackgroundImage = (Image)resources.GetObject("btnStage1.BackgroundImage");
            btnStage1.BackgroundImageLayout = ImageLayout.Stretch;
            btnStage1.FlatAppearance.BorderSize = 0;
            btnStage1.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnStage1.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnStage1.FlatStyle = FlatStyle.Flat;
            btnStage1.Location = new Point(540, 75);
            btnStage1.Name = "btnStage1";
            btnStage1.Size = new Size(219, 108);
            btnStage1.TabIndex = 0;
            btnStage1.UseVisualStyleBackColor = false;
            btnStage1.Click += btnStage1_Click;
            // 
            // btnStage2
            // 
            btnStage2.BackColor = Color.Transparent;
            btnStage2.BackgroundImage = (Image)resources.GetObject("btnStage2.BackgroundImage");
            btnStage2.BackgroundImageLayout = ImageLayout.Stretch;
            btnStage2.FlatAppearance.BorderSize = 0;
            btnStage2.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnStage2.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnStage2.FlatStyle = FlatStyle.Flat;
            btnStage2.Location = new Point(540, 212);
            btnStage2.Name = "btnStage2";
            btnStage2.Size = new Size(219, 107);
            btnStage2.TabIndex = 1;
            btnStage2.UseVisualStyleBackColor = false;
            btnStage2.Click += btnStage2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Tiger Expert", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.HighlightText;
            label1.Location = new Point(84, 180);
            label1.Name = "label1";
            label1.Size = new Size(387, 81);
            label1.TabIndex = 3;
            label1.Text = "Into the shadowed abyss, \r\nI descend—unbroken...\r\n\r\n";
            label1.Click += label1_Click;
            // 
            // DungeonMode
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(btnStage2);
            Controls.Add(btnStage1);
            Controls.Add(btnBack);
            Name = "DungeonMode";
            Text = "DungeonMode";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnStage1;
        private System.Windows.Forms.Button btnStage2;
        private Label label1;
    }
}
