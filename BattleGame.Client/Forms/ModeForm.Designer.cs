namespace BattleGame.Client.Forms
{
    partial class ModeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModeForm));
            button2 = new Button();
            pictureBox1 = new PictureBox();
            button3 = new Button();
            pictureBox2 = new PictureBox();
            button4 = new Button();
            pictureBox3 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // button2
            // 
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.Location = new Point(579, 374);
            button2.Name = "button2";
            button2.Size = new Size(381, 65);
            button2.TabIndex = 1;
            button2.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(332, 23);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(372, 60);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // button3
            // 
            button3.BackgroundImage = (Image)resources.GetObject("button3.BackgroundImage");
            button3.BackgroundImageLayout = ImageLayout.Stretch;
            button3.Location = new Point(12, 519);
            button3.Name = "button3";
            button3.Size = new Size(112, 50);
            button3.TabIndex = 3;
            button3.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.Location = new Point(87, 151);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(381, 236);
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // button4
            // 
            button4.BackgroundImage = (Image)resources.GetObject("button4.BackgroundImage");
            button4.BackgroundImageLayout = ImageLayout.Stretch;
            button4.Location = new Point(87, 374);
            button4.Name = "button4";
            button4.Size = new Size(381, 65);
            button4.TabIndex = 5;
            button4.UseVisualStyleBackColor = true;
            // 
            // pictureBox3
            // 
            pictureBox3.BackgroundImage = (Image)resources.GetObject("pictureBox3.BackgroundImage");
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Location = new Point(579, 151);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(381, 236);
            pictureBox3.TabIndex = 6;
            pictureBox3.TabStop = false;
            // 
            // ModeForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1057, 581);
            Controls.Add(button2);
            Controls.Add(pictureBox3);
            Controls.Add(button4);
            Controls.Add(pictureBox2);
            Controls.Add(button3);
            Controls.Add(pictureBox1);
            Name = "ModeForm";
            Text = "ModeForm";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button button2;
        private PictureBox pictureBox1;
        private Button button3;
        private PictureBox pictureBox2;
        private Button button4;
        private PictureBox pictureBox3;
    }
}