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
            btnSelectCharacter = new Button();
            lblSelectedCharacter = new Label();
            label2 = new Label();
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
            btnBack.Location = new Point(729, 457);
            btnBack.Margin = new Padding(3, 4, 3, 4);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(109, 47);
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
            btnStage1.Location = new Point(653, 49);
            btnStage1.Margin = new Padding(3, 4, 3, 4);
            btnStage1.Name = "btnStage1";
            btnStage1.Size = new Size(250, 144);
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
            btnStage2.Location = new Point(658, 231);
            btnStage2.Margin = new Padding(3, 4, 3, 4);
            btnStage2.Name = "btnStage2";
            btnStage2.Size = new Size(250, 139);
            btnStage2.TabIndex = 1;
            btnStage2.UseVisualStyleBackColor = false;
            btnStage2.Click += btnStage2_Click;
            // 
            // btnSelectCharacter
            // 
            btnSelectCharacter.BackColor = Color.Transparent;
            btnSelectCharacter.BackgroundImage = (Image)resources.GetObject("btnSelectCharacter.BackgroundImage");
            btnSelectCharacter.BackgroundImageLayout = ImageLayout.Stretch;
            btnSelectCharacter.FlatAppearance.BorderSize = 0;
            btnSelectCharacter.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnSelectCharacter.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnSelectCharacter.FlatStyle = FlatStyle.Flat;
            btnSelectCharacter.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnSelectCharacter.ForeColor = Color.Transparent;
            btnSelectCharacter.Location = new Point(84, 430);
            btnSelectCharacter.Margin = new Padding(3, 4, 3, 4);
            btnSelectCharacter.Name = "btnSelectCharacter";
            btnSelectCharacter.Size = new Size(112, 104);
            btnSelectCharacter.TabIndex = 4;
            btnSelectCharacter.Text = "CHOOSE";
            btnSelectCharacter.UseVisualStyleBackColor = false;
            btnSelectCharacter.Click += btnSelectCharacter_Click;
            // 
            // lblSelectedCharacter
            // 
            lblSelectedCharacter.BackColor = Color.Transparent;
            lblSelectedCharacter.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSelectedCharacter.ForeColor = Color.White;
            lblSelectedCharacter.Location = new Point(257, 509);
            lblSelectedCharacter.Name = "lblSelectedCharacter";
            lblSelectedCharacter.Size = new Size(250, 25);
            lblSelectedCharacter.TabIndex = 5;
            lblSelectedCharacter.TextAlign = ContentAlignment.MiddleCenter;
            lblSelectedCharacter.Click += lblSelectedCharacter_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.DarkOrange;
            label2.Location = new Point(53, 127);
            label2.Name = "label2";
            label2.Size = new Size(594, 196);
            label2.TabIndex = 6;
            label2.Text = resources.GetString("label2.Text");
            label2.Click += label2_Click_1;
            // 
            // DungeonMode
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(945, 560);
            Controls.Add(label2);
            Controls.Add(lblSelectedCharacter);
            Controls.Add(btnSelectCharacter);
            Controls.Add(btnStage2);
            Controls.Add(btnStage1);
            Controls.Add(btnBack);
            Margin = new Padding(3, 4, 3, 4);
            Name = "DungeonMode";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnStage1;
        private System.Windows.Forms.Button btnStage2;
        private Button btnSelectCharacter;
        private Label lblSelectedCharacter;
        private Label label2;
    }
}
