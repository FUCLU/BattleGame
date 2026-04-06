namespace BattleGame.Client.Forms
{
    partial class ResetPasswordForm
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
            linkLabel1 = new LinkLabel();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            button1 = new Button();
            label4 = new Label();
            label3 = new Label();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(372, 140);
            panel1.Name = "panel1";
            panel1.Size = new Size(321, 301);
            panel1.TabIndex = 0;
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = SystemColors.ActiveCaptionText;
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.Red;
            linkLabel1.Location = new Point(92, 257);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(126, 28);
            linkLabel1.TabIndex = 7;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Back to login";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.Location = new Point(26, 160);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(266, 37);
            textBox2.TabIndex = 6;
            textBox2.UseSystemPasswordChar = true;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(27, 89);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(266, 37);
            textBox1.TabIndex = 5;
            textBox1.UseSystemPasswordChar = true;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // button1
            // 
            button1.BackColor = Color.SandyBrown;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(23, 206);
            button1.Name = "button1";
            button1.Size = new Size(274, 48);
            button1.TabIndex = 4;
            button1.Text = "Reset password";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(26, 129);
            label4.Name = "label4";
            label4.Size = new Size(210, 28);
            label4.TabIndex = 3;
            label4.Text = "Confirm new password";
            label4.Click += label4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(26, 56);
            label3.Name = "label3";
            label3.Size = new Size(139, 28);
            label3.TabIndex = 2;
            label3.Text = "New password";
            label3.Click += label3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(63, 12);
            label1.Name = "label1";
            label1.Size = new Size(200, 31);
            label1.TabIndex = 0;
            label1.Text = "RESET PASSWORD";
            label1.Click += label1_Click;
            // 
            // ResetPasswordForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.login;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(1057, 581);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "ResetPasswordForm";
            Text = "ResetPasswordForm";
            Load += ResetPasswordForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        private Panel panel1;
        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private Label label4;
        private Label label3;
        private TextBox textBox2;
        private LinkLabel linkLabel1;
    }
}