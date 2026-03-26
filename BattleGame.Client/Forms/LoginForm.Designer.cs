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
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            pnlLogin = new Panel();
            label15 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            btnRegister = new Button();
            btnLogin = new Button();
            checkShowpassword = new CheckBox();
            txtPassword = new TextBox();
            pictureBox4 = new PictureBox();
            txtUsername = new TextBox();
            pictureBox5 = new PictureBox();
            panel1 = new Panel();
            label17 = new Label();
            label16 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            button1 = new Button();
            textBox4 = new TextBox();
            textBox3 = new TextBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            label4 = new Label();
            panel2 = new Panel();
            label11 = new Label();
            button4 = new Button();
            button3 = new Button();
            label10 = new Label();
            label9 = new Label();
            textBox6 = new TextBox();
            button2 = new Button();
            textBox5 = new TextBox();
            panel3 = new Panel();
            label12 = new Label();
            button5 = new Button();
            button6 = new Button();
            label13 = new Label();
            label14 = new Label();
            textBox7 = new TextBox();
            textBox8 = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.background;
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
            pnlLogin.BackgroundImage = Properties.Resources.dangnhap;
            pnlLogin.Controls.Add(label15);
            pnlLogin.Controls.Add(label3);
            pnlLogin.Controls.Add(label2);
            pnlLogin.Controls.Add(label1);
            pnlLogin.Controls.Add(linkLabel1);
            pnlLogin.Controls.Add(btnRegister);
            pnlLogin.Controls.Add(btnLogin);
            pnlLogin.Controls.Add(checkShowpassword);
            pnlLogin.Controls.Add(txtPassword);
            pnlLogin.Controls.Add(pictureBox4);
            pnlLogin.Controls.Add(txtUsername);
            pnlLogin.Controls.Add(pictureBox5);
            pnlLogin.Location = new Point(285, 193);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Size = new Size(403, 291);
            pnlLogin.TabIndex = 18;
            pnlLogin.Paint += pnlLogin_Paint_1;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.BackColor = Color.Transparent;
            label15.Font = new Font("Century", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label15.ForeColor = Color.Salmon;
            label15.Location = new Point(128, 25);
            label15.Name = "label15";
            label15.Size = new Size(144, 23);
            label15.TabIndex = 20;
            label15.Text = "ĐĂNG NHẬP";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.ForeColor = Color.Red;
            label3.Location = new Point(70, 141);
            label3.Name = "label3";
            label3.Size = new Size(230, 20);
            label3.TabIndex = 19;
            label3.Text = "Sai mật khẩu hoặc tên đăng nhập";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.ForeColor = Color.Red;
            label2.Location = new Point(70, 89);
            label2.Name = "label2";
            label2.Size = new Size(205, 20);
            label2.TabIndex = 14;
            label2.Text = "Vui lòng nhập tên đăng nhập!";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.ForeColor = Color.Red;
            label1.Location = new Point(70, 140);
            label1.Name = "label1";
            label1.Size = new Size(170, 20);
            label1.TabIndex = 13;
            label1.Text = "Vui lòng nhập mật khẩu!";
            label1.Click += label1_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.BackColor = Color.Transparent;
            linkLabel1.Cursor = Cursors.Hand;
            linkLabel1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.White;
            linkLabel1.Location = new Point(40, 157);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(133, 23);
            linkLabel1.TabIndex = 12;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Quên mật khẩu";
            linkLabel1.VisitedLinkColor = Color.White;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.Gray;
            btnRegister.Cursor = Cursors.Hand;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI Black", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRegister.ForeColor = Color.LavenderBlush;
            btnRegister.Location = new Point(215, 207);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(148, 41);
            btnRegister.TabIndex = 11;
            btnRegister.TabStop = false;
            btnRegister.Text = "Đăng ký";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.Gray;
            btnLogin.BackgroundImageLayout = ImageLayout.None;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI Black", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogin.ForeColor = Color.Azure;
            btnLogin.Location = new Point(40, 207);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(148, 41);
            btnLogin.TabIndex = 10;
            btnLogin.TabStop = false;
            btnLogin.Text = "Đăng nhập";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // checkShowpassword
            // 
            checkShowpassword.AutoSize = true;
            checkShowpassword.BackColor = Color.Transparent;
            checkShowpassword.Cursor = Cursors.Hand;
            checkShowpassword.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkShowpassword.ForeColor = SystemColors.ControlLightLight;
            checkShowpassword.Location = new Point(235, 157);
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
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPassword.Location = new Point(72, 112);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Mật khẩu";
            txtPassword.Size = new Size(282, 26);
            txtPassword.TabIndex = 6;
            txtPassword.TabStop = false;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // pictureBox4
            // 
            pictureBox4.BackColor = Color.Transparent;
            pictureBox4.Image = Properties.Resources.pw_icon;
            pictureBox4.Location = new Point(30, 109);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(34, 29);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.TabIndex = 5;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // txtUsername
            // 
            txtUsername.BackColor = Color.Bisque;
            txtUsername.Cursor = Cursors.IBeam;
            txtUsername.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsername.Location = new Point(72, 60);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Tên đăng nhập";
            txtUsername.Size = new Size(282, 26);
            txtUsername.TabIndex = 3;
            txtUsername.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.BackColor = Color.Transparent;
            pictureBox5.Image = Properties.Resources._9187532;
            pictureBox5.Location = new Point(30, 57);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(34, 29);
            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.TabIndex = 2;
            pictureBox5.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackgroundImage = Properties.Resources.dangki;
            panel1.Controls.Add(label17);
            panel1.Controls.Add(label16);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(textBox4);
            panel1.Controls.Add(textBox3);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(label4);
            panel1.Location = new Point(304, 143);
            panel1.Name = "panel1";
            panel1.Size = new Size(364, 376);
            panel1.TabIndex = 19;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.BackColor = Color.Transparent;
            label17.ForeColor = Color.Red;
            label17.Location = new Point(42, 268);
            label17.Name = "label17";
            label17.Size = new Size(166, 20);
            label17.TabIndex = 20;
            label17.Text = "Email đã được sử dụng!";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.BackColor = Color.Transparent;
            label16.ForeColor = Color.Red;
            label16.Location = new Point(42, 116);
            label16.Name = "label16";
            label16.Size = new Size(175, 20);
            label16.TabIndex = 19;
            label16.Text = "Tên đăng nhập đã tồn tại";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.Transparent;
            label8.ForeColor = Color.Red;
            label8.Location = new Point(42, 268);
            label8.Name = "label8";
            label8.Size = new Size(222, 20);
            label8.TabIndex = 18;
            label8.Text = "Vui lòng nhập email đúng dạng!";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.ForeColor = Color.Red;
            label7.Location = new Point(42, 216);
            label7.Name = "label7";
            label7.Size = new Size(156, 20);
            label7.TabIndex = 17;
            label7.Text = "Mật khẩu không khớp!";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.ForeColor = Color.Red;
            label6.Location = new Point(42, 168);
            label6.Name = "label6";
            label6.Size = new Size(170, 20);
            label6.TabIndex = 16;
            label6.Text = "Vui lòng nhập mật khẩu!";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.ForeColor = Color.Red;
            label5.Location = new Point(42, 116);
            label5.Name = "label5";
            label5.Size = new Size(205, 20);
            label5.TabIndex = 15;
            label5.Text = "Vui lòng nhập tên đăng nhập!";
            // 
            // button1
            // 
            button1.BackColor = Color.Gray;
            button1.Cursor = Cursors.Hand;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI Black", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.LavenderBlush;
            button1.Location = new Point(71, 297);
            button1.Name = "button1";
            button1.Size = new Size(210, 41);
            button1.TabIndex = 12;
            button1.TabStop = false;
            button1.Text = "ĐĂNG KÝ";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.Bisque;
            textBox4.Cursor = Cursors.IBeam;
            textBox4.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox4.Location = new Point(42, 239);
            textBox4.Name = "textBox4";
            textBox4.PlaceholderText = "Email";
            textBox4.Size = new Size(275, 26);
            textBox4.TabIndex = 7;
            textBox4.TabStop = false;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.Bisque;
            textBox3.Cursor = Cursors.IBeam;
            textBox3.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox3.Location = new Point(42, 189);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "Xác nhận mật khẩu";
            textBox3.Size = new Size(275, 26);
            textBox3.TabIndex = 6;
            textBox3.TabStop = false;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.Bisque;
            textBox2.Cursor = Cursors.IBeam;
            textBox2.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.Location = new Point(42, 139);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Mật khẩu";
            textBox2.Size = new Size(275, 26);
            textBox2.TabIndex = 5;
            textBox2.TabStop = false;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.Bisque;
            textBox1.Cursor = Cursors.IBeam;
            textBox1.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(42, 89);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Tên đăng nhập";
            textBox1.Size = new Size(275, 26);
            textBox1.TabIndex = 4;
            textBox1.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Century", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.Salmon;
            label4.Location = new Point(53, 38);
            label4.Name = "label4";
            label4.Size = new Size(248, 23);
            label4.TabIndex = 0;
            label4.Text = "ĐĂNG KÝ TÀI KHOẢN";
            label4.Click += label4_Click;
            // 
            // panel2
            // 
            panel2.BackgroundImage = Properties.Resources.quenmk;
            panel2.Controls.Add(label11);
            panel2.Controls.Add(button4);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(label9);
            panel2.Controls.Add(textBox6);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(textBox5);
            panel2.Location = new Point(305, 200);
            panel2.Name = "panel2";
            panel2.Size = new Size(360, 216);
            panel2.TabIndex = 20;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = Color.Transparent;
            label11.Font = new Font("Century", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.ForeColor = Color.Salmon;
            label11.Location = new Point(90, 28);
            label11.Name = "label11";
            label11.Size = new Size(165, 23);
            label11.TabIndex = 23;
            label11.Text = "Quên mật khẩu";
            // 
            // button4
            // 
            button4.BackColor = Color.Gray;
            button4.Cursor = Cursors.Hand;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Times New Roman", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.LavenderBlush;
            button4.Location = new Point(193, 169);
            button4.Name = "button4";
            button4.Size = new Size(135, 26);
            button4.TabIndex = 22;
            button4.TabStop = false;
            button4.Text = "XÁC NHẬN";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.Gray;
            button3.Cursor = Cursors.Hand;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Times New Roman", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.LavenderBlush;
            button3.Location = new Point(33, 169);
            button3.Name = "button3";
            button3.Size = new Size(138, 26);
            button3.TabIndex = 21;
            button3.TabStop = false;
            button3.Text = "TRỞ LẠI";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = Color.Transparent;
            label10.ForeColor = Color.Red;
            label10.Location = new Point(30, 146);
            label10.Name = "label10";
            label10.Size = new Size(206, 20);
            label10.TabIndex = 20;
            label10.Text = "Mã xác nhận không chính xác!";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.ForeColor = Color.Red;
            label9.Location = new Point(30, 92);
            label9.Name = "label9";
            label9.Size = new Size(222, 20);
            label9.TabIndex = 19;
            label9.Text = "Vui lòng nhập email đúng dạng!";
            // 
            // textBox6
            // 
            textBox6.BackColor = Color.Bisque;
            textBox6.Cursor = Cursors.IBeam;
            textBox6.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox6.Location = new Point(33, 117);
            textBox6.Name = "textBox6";
            textBox6.PlaceholderText = "Mã xác nhận";
            textBox6.Size = new Size(189, 26);
            textBox6.TabIndex = 14;
            textBox6.TabStop = false;
            // 
            // button2
            // 
            button2.BackColor = Color.Gray;
            button2.Cursor = Cursors.Hand;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Times New Roman", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.LavenderBlush;
            button2.Location = new Point(241, 63);
            button2.Name = "button2";
            button2.Size = new Size(96, 26);
            button2.TabIndex = 13;
            button2.TabStop = false;
            button2.Text = "GỬI";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // textBox5
            // 
            textBox5.BackColor = Color.Bisque;
            textBox5.Cursor = Cursors.IBeam;
            textBox5.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox5.Location = new Point(33, 63);
            textBox5.Name = "textBox5";
            textBox5.PlaceholderText = "Email";
            textBox5.Size = new Size(189, 26);
            textBox5.TabIndex = 5;
            textBox5.TabStop = false;
            textBox5.TextChanged += textBox5_TextChanged;
            // 
            // panel3
            // 
            panel3.BackgroundImage = Properties.Resources.quenmk;
            panel3.Controls.Add(label12);
            panel3.Controls.Add(button5);
            panel3.Controls.Add(button6);
            panel3.Controls.Add(label13);
            panel3.Controls.Add(label14);
            panel3.Controls.Add(textBox7);
            panel3.Controls.Add(textBox8);
            panel3.Location = new Point(305, 200);
            panel3.Name = "panel3";
            panel3.Size = new Size(360, 216);
            panel3.TabIndex = 21;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.BackColor = Color.Transparent;
            label12.Font = new Font("Century", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label12.ForeColor = Color.Salmon;
            label12.Location = new Point(77, 30);
            label12.Name = "label12";
            label12.Size = new Size(219, 23);
            label12.TabIndex = 23;
            label12.Text = "ĐẶT LẠI MẬT KHẨU";
            label12.Click += label12_Click;
            // 
            // button5
            // 
            button5.BackColor = Color.Gray;
            button5.Cursor = Cursors.Hand;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Times New Roman", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button5.ForeColor = Color.LavenderBlush;
            button5.Location = new Point(193, 169);
            button5.Name = "button5";
            button5.Size = new Size(135, 26);
            button5.TabIndex = 22;
            button5.TabStop = false;
            button5.Text = "XÁC NHẬN";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.BackColor = Color.Gray;
            button6.Cursor = Cursors.Hand;
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("Times New Roman", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button6.ForeColor = Color.LavenderBlush;
            button6.Location = new Point(33, 169);
            button6.Name = "button6";
            button6.Size = new Size(138, 26);
            button6.TabIndex = 21;
            button6.TabStop = false;
            button6.Text = "TRỞ LẠI";
            button6.UseVisualStyleBackColor = false;
            button6.Click += button6_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.BackColor = Color.Transparent;
            label13.ForeColor = Color.Red;
            label13.Location = new Point(33, 146);
            label13.Name = "label13";
            label13.Size = new Size(195, 20);
            label13.TabIndex = 20;
            label13.Text = "Mật khẩu không trùng khớp!";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.BackColor = Color.Transparent;
            label14.ForeColor = Color.Red;
            label14.Location = new Point(33, 91);
            label14.Name = "label14";
            label14.Size = new Size(170, 20);
            label14.TabIndex = 19;
            label14.Text = "Vui lòng nhập mật khẩu!";
            // 
            // textBox7
            // 
            textBox7.BackColor = Color.Bisque;
            textBox7.Cursor = Cursors.IBeam;
            textBox7.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox7.Location = new Point(33, 117);
            textBox7.Name = "textBox7";
            textBox7.PlaceholderText = "Xác nhận mật khẩu";
            textBox7.Size = new Size(286, 26);
            textBox7.TabIndex = 14;
            textBox7.TabStop = false;
            // 
            // textBox8
            // 
            textBox8.BackColor = Color.Bisque;
            textBox8.Cursor = Cursors.IBeam;
            textBox8.Font = new Font("Century", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox8.Location = new Point(33, 63);
            textBox8.Name = "textBox8";
            textBox8.PlaceholderText = "Mật khẩu mới";
            textBox8.Size = new Size(286, 26);
            textBox8.TabIndex = 5;
            textBox8.TabStop = false;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(979, 564);
            Controls.Add(panel3);
            Controls.Add(pnlLogin);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Name = "LoginForm";
            Text = "LoginForm";
            Load += LoginForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlLogin.ResumeLayout(false);
            pnlLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Panel pnlLogin;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private CheckBox checkShowpassword;
        private TextBox txtPassword;
        private TextBox txtUsername;
        private Button btnLogin;
        private Button btnRegister;
        private LinkLabel linkLabel1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Panel panel1;
        private Label label4;
        private TextBox textBox4;
        private TextBox textBox3;
        private TextBox textBox2;
        private TextBox textBox1;
        private Button button1;
        private Label label6;
        private Label label5;
        private Label label8;
        private Label label7;
        private Panel panel2;
        private TextBox textBox5;
        private Label label10;
        private Label label9;
        private TextBox textBox6;
        private Button button2;
        private Label label11;
        private Button button4;
        private Button button3;
        private Panel panel3;
        private Label label12;
        private Button button5;
        private Button button6;
        private Label label13;
        private Label label14;
        private TextBox textBox7;
        private TextBox textBox8;
        private System.Windows.Forms.Timer timer1;
        private Label label15;
        private Label label17;
        private Label label16;
    }
}