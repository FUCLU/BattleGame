namespace BattleGame.Client.Forms
{
    partial class LeaderboardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LeaderboardForm));
            pictureBox1 = new PictureBox();
            listView1 = new ListView();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(386, 31);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(273, 69);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // listView1
            // 
            listView1.BackColor = SystemColors.Info;
            listView1.Font = new Font("Book Antiqua", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listView1.Location = new Point(120, 127);
            listView1.Name = "listView1";
            listView1.Size = new Size(818, 364);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            button1.BackColor = Color.Brown;
            button1.Font = new Font("Book Antiqua", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(472, 515);
            button1.Name = "button1";
            button1.Size = new Size(119, 42);
            button1.TabIndex = 2;
            button1.Text = "Close";
            button1.UseVisualStyleBackColor = false;
            // 
            // LeaderboardForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(1057, 581);
            Controls.Add(button1);
            Controls.Add(listView1);
            Controls.Add(pictureBox1);
            Name = "LeaderboardForm";
            Text = "LeaderboardForm";
            Load += LeaderboardForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private ListView listView1;
        private Button button1;
    }
}