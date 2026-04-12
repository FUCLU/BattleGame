namespace BattleGame.Client.Forms
{
    partial class GameOverForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameOverForm));
            panel1 = new Panel();
            label1 = new Label();
            panel2 = new Panel();
            label2 = new Label();
            label3 = new Label();
            panel3 = new Panel();
            button1 = new Button();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.DarkSeaGreen;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(630, 361);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Book Antiqua", 25.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Firebrick;
            label1.Location = new Point(185, 9);
            label1.Name = "label1";
            label1.Size = new Size(253, 52);
            label1.TabIndex = 0;
            label1.Text = "Game Over";
            // 
            // panel2
            // 
            panel2.BackColor = Color.DarkKhaki;
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Location = new Point(58, 62);
            panel2.Name = "panel2";
            panel2.Size = new Size(533, 80);
            panel2.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Book Antiqua", 12F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.MidnightBlue;
            label2.Location = new Point(223, -2);
            label2.Name = "label2";
            label2.Size = new Size(74, 26);
            label2.TabIndex = 0;
            label2.Text = "Victory";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Book Antiqua", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.GreenYellow;
            label3.Location = new Point(191, 26);
            label3.Name = "label3";
            label3.Size = new Size(137, 49);
            label3.TabIndex = 1;
            label3.Text = "Player";
            // 
            // panel3
            // 
            panel3.BackColor = Color.DarkKhaki;
            panel3.Controls.Add(label8);
            panel3.Controls.Add(label7);
            panel3.Controls.Add(label6);
            panel3.Controls.Add(label5);
            panel3.Controls.Add(label4);
            panel3.Location = new Point(58, 148);
            panel3.Name = "panel3";
            panel3.Size = new Size(533, 124);
            panel3.TabIndex = 2;
            // 
            // button1
            // 
            button1.BackColor = Color.DarkCyan;
            button1.Font = new Font("Book Antiqua", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(218, 292);
            button1.Name = "button1";
            button1.Size = new Size(195, 57);
            button1.TabIndex = 3;
            button1.Text = "Back to lobby";
            button1.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Book Antiqua", 12F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.MidnightBlue;
            label4.Location = new Point(205, 0);
            label4.Name = "label4";
            label4.Size = new Size(114, 26);
            label4.TabIndex = 2;
            label4.Text = "Match Score";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Book Antiqua", 12F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.Crimson;
            label5.Location = new Point(106, 37);
            label5.Name = "label5";
            label5.Size = new Size(85, 26);
            label5.TabIndex = 3;
            label5.Text = "Player 1:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Book Antiqua", 12F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.Crimson;
            label6.Location = new Point(106, 78);
            label6.Name = "label6";
            label6.Size = new Size(85, 26);
            label6.TabIndex = 4;
            label6.Text = "Player 2:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Book Antiqua", 12F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.Crimson;
            label7.Location = new Point(223, 38);
            label7.Name = "label7";
            label7.Size = new Size(27, 26);
            label7.TabIndex = 5;
            label7.Text = "...";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Book Antiqua", 12F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label8.ForeColor = Color.Crimson;
            label8.Location = new Point(223, 78);
            label8.Name = "label8";
            label8.Size = new Size(27, 26);
            label8.TabIndex = 6;
            label8.Text = "...";
            // 
            // GameOverForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(654, 385);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "GameOverForm";
            Text = "GameOverForm";
            Load += GameOverForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Label label3;
        private Label label2;
        private Label label1;
        private Button button1;
        private Panel panel3;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
    }
}