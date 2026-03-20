namespace BattleGame.Client.Forms
{
    partial class LoginForm
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
            pictureBox1 = new PictureBox();
            pnlLogin = new Panel();
            btnRegister = new Button();
            btnLogin = new Button();
            checkShowpassword = new CheckBox();
            txtPassword = new TextBox();
            pictureBox4 = new PictureBox();
            panel3 = new Panel();
            txtUsername = new TextBox();
            pictureBox5 = new PictureBox();
            panel4 = new Panel();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.backround_login;
            pictureBox1.Location = new Point(-49, -26);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1081, 630);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // pnlLogin
            // 
            pnlLogin.BackColor = Color.Black;
            pnlLogin.Controls.Add(btnRegister);
            pnlLogin.Controls.Add(btnLogin);
            pnlLogin.Controls.Add(checkShowpassword);
            pnlLogin.Controls.Add(txtPassword);
            pnlLogin.Controls.Add(pictureBox4);
            pnlLogin.Controls.Add(panel3);
            pnlLogin.Controls.Add(txtUsername);
            pnlLogin.Controls.Add(pictureBox5);
            pnlLogin.Controls.Add(panel4);
            pnlLogin.Controls.Add(label2);
            pnlLogin.Location = new Point(281, 149);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Size = new Size(395, 290);
            pnlLogin.TabIndex = 18;
            pnlLogin.Paint += pnlLogin_Paint_1;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.Gray;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI Black", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRegister.ForeColor = Color.LavenderBlush;
            btnRegister.Location = new Point(206, 219);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(148, 41);
            btnRegister.TabIndex = 11;
            btnRegister.TabStop = false;
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.Gray;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI Black", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogin.ForeColor = Color.Azure;
            btnLogin.Location = new Point(40, 219);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(148, 41);
            btnLogin.TabIndex = 10;
            btnLogin.TabStop = false;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            btnLogin.MouseEnter += btnLogin_MouseEnter;
            btnLogin.MouseLeave += btnLogin_MouseLeave;
            // 
            // checkShowpassword
            // 
            checkShowpassword.AutoSize = true;
            checkShowpassword.BackColor = Color.Transparent;
            checkShowpassword.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkShowpassword.ForeColor = SystemColors.ControlLightLight;
            checkShowpassword.Location = new Point(227, 176);
            checkShowpassword.Name = "checkShowpassword";
            checkShowpassword.Size = new Size(136, 24);
            checkShowpassword.TabIndex = 9;
            checkShowpassword.Text = "Show password";
            checkShowpassword.UseVisualStyleBackColor = false;
            checkShowpassword.CheckedChanged += checkShowpw_CheckedChanged;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.Bisque;
            txtPassword.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPassword.Location = new Point(70, 136);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Password";
            txtPassword.Size = new Size(282, 26);
            txtPassword.TabIndex = 6;
            txtPassword.TabStop = false;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // pictureBox4
            // 
            pictureBox4.BackColor = Color.Transparent;
            pictureBox4.Image = Properties.Resources.pw_icon;
            pictureBox4.Location = new Point(30, 132);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(34, 29);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.TabIndex = 5;
            pictureBox4.TabStop = false;
            // 
            // panel3
            // 
            panel3.BackColor = Color.SlateBlue;
            panel3.Location = new Point(30, 167);
            panel3.Name = "panel3";
            panel3.Size = new Size(333, 3);
            panel3.TabIndex = 4;
            // 
            // txtUsername
            // 
            txtUsername.BackColor = Color.Bisque;
            txtUsername.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsername.Location = new Point(70, 82);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Username";
            txtUsername.Size = new Size(282, 26);
            txtUsername.TabIndex = 3;
            txtUsername.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.BackColor = Color.Transparent;
            pictureBox5.Image = Properties.Resources._9187532;
            pictureBox5.Location = new Point(30, 78);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(34, 29);
            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.TabIndex = 2;
            pictureBox5.TabStop = false;
            // 
            // panel4
            // 
            panel4.BackColor = Color.SlateBlue;
            panel4.Location = new Point(30, 113);
            panel4.Name = "panel4";
            panel4.Size = new Size(333, 3);
            panel4.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Black;
            label2.Font = new Font("Elephant", 16.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(59, 13);
            label2.Name = "label2";
            label2.Size = new Size(263, 35);
            label2.TabIndex = 0;
            label2.Text = "BATTLE GAME";
            label2.TextAlign = ContentAlignment.TopCenter;
            label2.Click += label2_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(979, 564);
            Controls.Add(pnlLogin);
            Controls.Add(pictureBox1);
            Name = "LoginForm";
            Text = "LoginForm";
            Load += LoginForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlLogin.ResumeLayout(false);
            pnlLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Panel pnlLogin;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private CheckBox checkShowpassword;
        private TextBox txtPassword;
        private Panel panel3;
        private TextBox txtUsername;
        private Panel panel4;
        private Label label2;
        private Button btnLogin;
        private Button btnRegister;
    }
}