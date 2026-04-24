namespace BattleGame.Client.Forms
{
    partial class ForgotPasswordForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ForgotPasswordForm));
            panel1 = new Panel();
            btnBackLogin = new Button();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            textBox1 = new TextBox();
            label3 = new Label();
            button1 = new Button();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            button2 = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.SteelBlue;
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(btnBackLogin);
            panel1.Controls.Add(pictureBox3);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(316, 85);
            panel1.Name = "panel1";
            panel1.Size = new Size(427, 357);
            panel1.TabIndex = 0;
            // 
            // btnBackLogin
            // 
            btnBackLogin.BackColor = Color.SandyBrown;
            btnBackLogin.FlatAppearance.BorderColor = Color.Teal;
            btnBackLogin.FlatAppearance.BorderSize = 3;
            btnBackLogin.FlatStyle = FlatStyle.Popup;
            btnBackLogin.Font = new Font("Georgia", 9F);
            btnBackLogin.Location = new Point(247, 258);
            btnBackLogin.Name = "btnBackLogin";
            btnBackLogin.Size = new Size(163, 43);
            btnBackLogin.TabIndex = 10;
            btnBackLogin.Text = "BACK TO LOGIN";
            btnBackLogin.UseVisualStyleBackColor = false;
            btnBackLogin.Click += btnBackLogin_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(25, 29);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(47, 47);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 9;
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(363, 29);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(47, 47);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 8;
            pictureBox2.TabStop = false;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(25, 190);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(385, 38);
            textBox1.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.Window;
            label3.Location = new Point(22, 154);
            label3.Name = "label3";
            label3.Size = new Size(75, 31);
            label3.TabIndex = 5;
            label3.Text = "Email";
            // 
            // button1
            // 
            button1.BackColor = Color.SandyBrown;
            button1.FlatAppearance.BorderColor = Color.Teal;
            button1.FlatAppearance.BorderSize = 3;
            button1.FlatStyle = FlatStyle.Popup;
            button1.Font = new Font("Georgia", 9F);
            button1.Location = new Point(19, 258);
            button1.Name = "button1";
            button1.Size = new Size(163, 43);
            button1.TabIndex = 3;
            button1.Text = "SEND RESET CODE";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic Medium", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.DarkSalmon;
            label2.Location = new Point(25, 96);
            label2.Name = "label2";
            label2.Size = new Size(279, 44);
            label2.TabIndex = 2;
            label2.Text = "Enter your registered email to \r\nreceive reset code.\r\n";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(73, 24);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(289, 57);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // button2
            // 
            button2.BackgroundImage = Properties.Resources.loa;
            button2.BackgroundImageLayout = ImageLayout.Zoom;
            button2.Location = new Point(999, 521);
            button2.Name = "button2";
            button2.Size = new Size(35, 33);
            button2.TabIndex = 2;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // ForgotPasswordForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.login;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(1057, 581);
            Controls.Add(button2);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "ForgotPasswordForm";
            Text = "ForgotPasswordForm";
            Load += ForgotPasswordForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label2;
        private Button button1;
        private TextBox textBox1;
        private Label label3;
        private Button button2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private Button btnBackLogin;
    }
}