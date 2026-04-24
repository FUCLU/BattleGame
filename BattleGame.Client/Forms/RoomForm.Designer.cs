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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoomForm));
            panel3 = new Panel();
            btnSend = new Button();
            txtBoxInp = new TextBox();
            label3 = new Label();
            richtxtBoxMessage = new RichTextBox();
            panel2 = new Panel();
            label4 = new Label();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label1 = new Label();
            button1 = new Button();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            panel1 = new Panel();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            lblReady1 = new Label();
            lblReady2 = new Label();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel3
            // 
            panel3.BackColor = Color.Transparent;
            panel3.BackgroundImage = (Image)resources.GetObject("panel3.BackgroundImage");
            panel3.BackgroundImageLayout = ImageLayout.Stretch;
            panel3.Controls.Add(btnSend);
            panel3.Controls.Add(txtBoxInp);
            panel3.Controls.Add(label3);
            panel3.Controls.Add(richtxtBoxMessage);
            panel3.Location = new Point(447, 28);
            panel3.Name = "panel3";
            panel3.Size = new Size(528, 435);
            panel3.TabIndex = 3;
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.DarkSlateGray;
            btnSend.BackgroundImage = (Image)resources.GetObject("btnSend.BackgroundImage");
            btnSend.BackgroundImageLayout = ImageLayout.Stretch;
            btnSend.FlatAppearance.BorderColor = Color.LightBlue;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Font = new Font("Book Antiqua", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSend.Location = new Point(398, 383);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(89, 38);
            btnSend.TabIndex = 3;
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            // 
            // txtBoxInp
            // 
            txtBoxInp.BackColor = Color.FromArgb(18, 3, 69);
            txtBoxInp.BorderStyle = BorderStyle.FixedSingle;
            txtBoxInp.Font = new Font("Book Antiqua", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtBoxInp.ForeColor = Color.Gold;
            txtBoxInp.Location = new Point(20, 390);
            txtBoxInp.Name = "txtBoxInp";
            txtBoxInp.PlaceholderText = "Nhap tin nhan ...";
            txtBoxInp.Size = new Size(372, 29);
            txtBoxInp.TabIndex = 2;
            txtBoxInp.KeyDown += txtBoxInp_KeyDown;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Bookman Old Style", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.PeachPuff;
            label3.Location = new Point(20, 5);
            label3.Name = "label3";
            label3.Size = new Size(81, 28);
            label3.TabIndex = 1;
            label3.Text = "CHAT";
            // 
            // richtxtBoxMessage
            // 
            richtxtBoxMessage.BackColor = Color.FromArgb(18, 3, 69);
            richtxtBoxMessage.BorderStyle = BorderStyle.None;
            richtxtBoxMessage.ForeColor = Color.Gold;
            richtxtBoxMessage.Location = new Point(20, 33);
            richtxtBoxMessage.Name = "richtxtBoxMessage";
            richtxtBoxMessage.ReadOnly = true;
            richtxtBoxMessage.Size = new Size(467, 342);
            richtxtBoxMessage.TabIndex = 0;
            richtxtBoxMessage.Text = "";
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.BackgroundImage = (Image)resources.GetObject("panel2.BackgroundImage");
            panel2.BackgroundImageLayout = ImageLayout.Stretch;
            panel2.Controls.Add(lblReady2);
            panel2.Controls.Add(lblReady1);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(pictureBox1);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(textBox2);
            panel2.Controls.Add(textBox1);
            panel2.Location = new Point(71, 28);
            panel2.Name = "panel2";
            panel2.Size = new Size(351, 435);
            panel2.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Bookman Old Style", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ButtonHighlight;
            label4.Location = new Point(93, 198);
            label4.Name = "label4";
            label4.Size = new Size(140, 32);
            label4.TabIndex = 8;
            label4.Text = "1010101";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(20, 145);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(297, 109);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold | FontStyle.Italic);
            label2.ForeColor = SystemColors.Info;
            label2.Location = new Point(20, 382);
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
            label1.Location = new Point(20, 346);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 5;
            label1.Text = "Map 1";
            // 
            // button1
            // 
            button1.BackColor = Color.Transparent;
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button1.FlatAppearance.BorderColor = Color.DarkGreen;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Constantia", 10.8F, FontStyle.Italic, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.Black;
            button1.Location = new Point(20, 300);
            button1.Name = "button1";
            button1.Size = new Size(161, 43);
            button1.TabIndex = 4;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(34, 124, 162);
            textBox2.Font = new Font("Book Antiqua", 13.8F, FontStyle.Bold);
            textBox2.ForeColor = Color.Gold;
            textBox2.Location = new Point(20, 67);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(290, 35);
            textBox2.TabIndex = 3;
            textBox2.TabStop = false;
            textBox2.Text = "PLAYER 2";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(34, 124, 162);
            textBox1.Font = new Font("Book Antiqua", 13.8F, FontStyle.Bold);
            textBox1.ForeColor = Color.Gold;
            textBox1.Location = new Point(20, 20);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(290, 35);
            textBox1.TabIndex = 2;
            textBox1.TabStop = false;
            textBox1.Text = "PLAYER 1";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Location = new Point(65, 483);
            panel1.Name = "panel1";
            panel1.Size = new Size(910, 68);
            panel1.TabIndex = 3;
            // 
            // button4
            // 
            button4.BackColor = Color.DarkSlateGray;
            button4.BackgroundImage = (Image)resources.GetObject("button4.BackgroundImage");
            button4.BackgroundImageLayout = ImageLayout.Stretch;
            button4.FlatAppearance.BorderColor = Color.DimGray;
            button4.FlatStyle = FlatStyle.Popup;
            button4.Font = new Font("Showcard Gothic", 10.2F, FontStyle.Italic, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.MediumTurquoise;
            button4.Location = new Point(721, 12);
            button4.Name = "button4";
            button4.Size = new Size(161, 43);
            button4.TabIndex = 9;
            button4.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.MediumSlateBlue;
            button3.BackgroundImage = (Image)resources.GetObject("button3.BackgroundImage");
            button3.BackgroundImageLayout = ImageLayout.Stretch;
            button3.FlatAppearance.BorderColor = Color.DimGray;
            button3.FlatStyle = FlatStyle.Popup;
            button3.Font = new Font("Showcard Gothic", 10.2F, FontStyle.Italic, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.MediumTurquoise;
            button3.Location = new Point(382, 12);
            button3.Name = "button3";
            button3.Size = new Size(161, 43);
            button3.TabIndex = 8;
            button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackColor = Color.Sienna;
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.FlatAppearance.BorderColor = Color.DimGray;
            button2.FlatStyle = FlatStyle.Popup;
            button2.Font = new Font("Showcard Gothic", 10.2F, FontStyle.Italic, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.LightGreen;
            button2.Location = new Point(26, 12);
            button2.Name = "button2";
            button2.Size = new Size(161, 43);
            button2.TabIndex = 7;
            button2.UseVisualStyleBackColor = false;
            // 
            // lblReady1
            // 
            lblReady1.AutoSize = true;
            lblReady1.BackColor = Color.FromArgb(34, 124, 162);
            lblReady1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblReady1.ForeColor = Color.SpringGreen;
            lblReady1.Location = new Point(237, 27);
            lblReady1.Name = "lblReady1";
            lblReady1.Size = new Size(58, 20);
            lblReady1.TabIndex = 9;
            lblReady1.Text = "READY";
            // 
            // lblReady2
            // 
            lblReady2.AutoSize = true;
            lblReady2.BackColor = Color.FromArgb(34, 124, 162);
            lblReady2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblReady2.ForeColor = Color.SpringGreen;
            lblReady2.Location = new Point(237, 74);
            lblReady2.Name = "lblReady2";
            lblReady2.Size = new Size(58, 20);
            lblReady2.TabIndex = 10;
            lblReady2.Text = "READY";
            // 
            // RoomForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.CornflowerBlue;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1057, 581);
            Controls.Add(panel1);
            Controls.Add(panel3);
            Controls.Add(panel2);
            DoubleBuffered = true;
            Name = "RoomForm";
            Text = "RoomForm";
            Load += RoomForm_Load;
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
        private Button btnSend;
        private TextBox txtBoxInp;
        private Label label3;
        private RichTextBox richtxtBoxMessage;
        private Label label4;
        private PictureBox pictureBox1;
        private Label lblReady2;
        private Label lblReady1;
    }
}