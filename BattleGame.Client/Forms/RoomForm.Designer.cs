namespace BattleGame.Client.Forms
{
    partial class RoomForm
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
            panel3 = new Panel();
            button5 = new Button();
            textBox3 = new TextBox();
            label3 = new Label();
            richTextBox1 = new RichTextBox();
            panel2 = new Panel();
            label2 = new Label();
            label1 = new Label();
            button1 = new Button();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            panel1 = new Panel();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel3
            // 
            panel3.BackColor = Color.OliveDrab;
            panel3.BorderStyle = BorderStyle.Fixed3D;
            panel3.Controls.Add(button5);
            panel3.Controls.Add(textBox3);
            panel3.Controls.Add(label3);
            panel3.Controls.Add(richTextBox1);
            panel3.Location = new Point(420, 28);
            panel3.Name = "panel3";
            panel3.Size = new Size(552, 439);
            panel3.TabIndex = 3;
            // 
            // button5
            // 
            button5.BackColor = Color.Olive;
            button5.FlatAppearance.BorderColor = Color.Orange;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Book Antiqua", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button5.Location = new Point(448, 390);
            button5.Name = "button5";
            button5.Size = new Size(79, 29);
            button5.TabIndex = 3;
            button5.Text = "SEND";
            button5.UseVisualStyleBackColor = false;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.DarkOliveGreen;
            textBox3.Font = new Font("Book Antiqua", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox3.ForeColor = Color.Gold;
            textBox3.Location = new Point(20, 390);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "Nhap tin nhan ...";
            textBox3.Size = new Size(409, 29);
            textBox3.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Book Antiqua", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.PeachPuff;
            label3.Location = new Point(11, 0);
            label3.Name = "label3";
            label3.Size = new Size(61, 28);
            label3.TabIndex = 1;
            label3.Text = "Chat";
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.DarkOliveGreen;
            richTextBox1.ForeColor = Color.Gold;
            richTextBox1.Location = new Point(20, 33);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(507, 343);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // panel2
            // 
            panel2.BackColor = Color.OliveDrab;
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(textBox2);
            panel2.Controls.Add(textBox1);
            panel2.Location = new Point(24, 28);
            panel2.Name = "panel2";
            panel2.Size = new Size(370, 439);
            panel2.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold | FontStyle.Italic);
            label2.ForeColor = SystemColors.Info;
            label2.Location = new Point(20, 336);
            label2.Name = "label2";
            label2.Size = new Size(94, 25);
            label2.TabIndex = 6;
            label2.Text = "Time: 3:00";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold | FontStyle.Italic);
            label1.ForeColor = SystemColors.Info;
            label1.Location = new Point(20, 286);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 5;
            label1.Text = "Map 1";
            // 
            // button1
            // 
            button1.BackColor = Color.Gray;
            button1.FlatAppearance.BorderColor = Color.DarkGreen;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Constantia", 10.8F, FontStyle.Italic, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.Black;
            button1.Location = new Point(20, 229);
            button1.Name = "button1";
            button1.Size = new Size(161, 43);
            button1.TabIndex = 4;
            button1.Text = "Choose Map";
            button1.UseVisualStyleBackColor = false;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.NavajoWhite;
            textBox2.Font = new Font("Book Antiqua", 13.8F);
            textBox2.ForeColor = Color.DarkOrange;
            textBox2.Location = new Point(20, 114);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(118, 36);
            textBox2.TabIndex = 3;
            textBox2.Text = "Player 2";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.NavajoWhite;
            textBox1.Font = new Font("Book Antiqua", 13.8F);
            textBox1.ForeColor = Color.DarkOrange;
            textBox1.Location = new Point(20, 33);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(118, 36);
            textBox1.TabIndex = 2;
            textBox1.Text = "Player 1";
            // 
            // panel1
            // 
            panel1.BackColor = Color.OliveDrab;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Location = new Point(24, 483);
            panel1.Name = "panel1";
            panel1.Size = new Size(948, 68);
            panel1.TabIndex = 3;
            // 
            // button4
            // 
            button4.BackColor = Color.DarkSlateGray;
            button4.FlatAppearance.BorderColor = Color.DimGray;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Showcard Gothic", 10.2F, FontStyle.Italic, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.MediumTurquoise;
            button4.Location = new Point(734, 12);
            button4.Name = "button4";
            button4.Size = new Size(161, 43);
            button4.TabIndex = 9;
            button4.Text = "START GAME";
            button4.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.MediumSlateBlue;
            button3.FlatAppearance.BorderColor = Color.DimGray;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Showcard Gothic", 10.2F, FontStyle.Italic, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.MediumTurquoise;
            button3.Location = new Point(407, 12);
            button3.Name = "button3";
            button3.Size = new Size(161, 43);
            button3.TabIndex = 8;
            button3.Text = "READY";
            button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackColor = Color.Sienna;
            button2.FlatAppearance.BorderColor = Color.DimGray;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Showcard Gothic", 10.2F, FontStyle.Italic, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.LightGreen;
            button2.Location = new Point(20, 12);
            button2.Name = "button2";
            button2.Size = new Size(161, 43);
            button2.TabIndex = 7;
            button2.Text = "BACK";
            button2.UseVisualStyleBackColor = false;
            // 
            // RoomForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(1015, 581);
            Controls.Add(panel1);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Name = "RoomForm";
            Text = "RoomForm";
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel3;
        private Panel panel2;
        private TextBox textBox2;
        private TextBox textBox1;
        private Panel panel1;
        private Label label2;
        private Label label1;
        private Button button1;
        private Button button4;
        private Button button3;
        private Button button2;
        private Button button5;
        private TextBox textBox3;
        private Label label3;
        private RichTextBox richTextBox1;
    }
}