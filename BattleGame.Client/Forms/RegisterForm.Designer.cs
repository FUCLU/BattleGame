namespace BattleGame.Client.Forms
{
    partial class RegisterForm
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
            panel1 = new Panel();
            label6 = new Label();
            linkLabel1 = new LinkLabel();
            label5 = new Label();
            textBox4 = new TextBox();
            textBox3 = new TextBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            button1 = new Button();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label6);
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(textBox4);
            panel1.Controls.Add(textBox3);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(370, 48);
            panel1.Name = "panel1";
            panel1.Size = new Size(317, 440);
            panel1.TabIndex = 0;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(63, 13);
            label6.Name = "label6";
            label6.Size = new Size(200, 31);
            label6.TabIndex = 1;
            label6.Text = "CREATE ACCOUNT";
            label6.Click += label6_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = SystemColors.ActiveCaptionText;
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.Red;
            linkLabel1.Location = new Point(244, 393);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(61, 28);
            linkLabel1.TabIndex = 10;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Login";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(15, 395);
            label5.Name = "label5";
            label5.Size = new Size(225, 28);
            label5.TabIndex = 9;
            label5.Text = "Already have an acount?";
            // 
            // textBox4
            // 
            textBox4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox4.Location = new Point(28, 297);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(260, 34);
            textBox4.TabIndex = 8;
            // 
            // textBox3
            // 
            textBox3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox3.Location = new Point(28, 220);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(260, 34);
            textBox3.TabIndex = 7;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.Location = new Point(28, 145);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(260, 34);
            textBox2.TabIndex = 6;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(28, 76);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(260, 34);
            textBox1.TabIndex = 5;
            // 
            // button1
            // 
            button1.BackColor = Color.SandyBrown;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(25, 348);
            button1.Name = "button1";
            button1.Size = new Size(263, 39);
            button1.TabIndex = 4;
            button1.Text = "Register";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(25, 263);
            label4.Name = "label4";
            label4.Size = new Size(168, 28);
            label4.TabIndex = 3;
            label4.Text = "Confirm Password";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(25, 189);
            label3.Name = "label3";
            label3.Size = new Size(93, 28);
            label3.TabIndex = 2;
            label3.Text = "Password";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(25, 114);
            label2.Name = "label2";
            label2.Size = new Size(51, 28);
            label2.TabIndex = 1;
            label2.Text = "User";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(25, 45);
            label1.Name = "label1";
            label1.Size = new Size(59, 28);
            label1.TabIndex = 0;
            label1.Text = "Email";
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.login;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(1057, 581);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "RegisterForm";
            Text = "RegisterForm";
            Load += RegisterForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private TextBox textBox4;
        private TextBox textBox3;
        private TextBox textBox2;
        private TextBox textBox1;
        private Button button1;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private LinkLabel linkLabel1;
        private Label label5;
        private Label label6;
    }
}