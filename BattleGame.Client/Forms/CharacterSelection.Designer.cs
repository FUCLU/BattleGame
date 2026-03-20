namespace BattleGame.Client.Forms
{
    partial class CharacterSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterSelection));
            panel1 = new Panel();
            txtInfCharac = new RichTextBox();
            characterReviewPic = new PictureBox();
            panel2 = new Panel();
            selectRyu = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            selectKen = new PictureBox();
            label1 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)characterReviewPic).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)selectRyu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)selectKen).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.SteelBlue;
            panel1.Controls.Add(txtInfCharac);
            panel1.Controls.Add(characterReviewPic);
            panel1.Location = new Point(593, 73);
            panel1.Name = "panel1";
            panel1.Size = new Size(360, 495);
            panel1.TabIndex = 0;
            // 
            // txtInfCharac
            // 
            txtInfCharac.Font = new Font("Yu Gothic", 10.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            txtInfCharac.Location = new Point(27, 373);
            txtInfCharac.Name = "txtInfCharac";
            txtInfCharac.ReadOnly = true;
            txtInfCharac.Size = new Size(305, 100);
            txtInfCharac.TabIndex = 1;
            txtInfCharac.TabStop = false;
            txtInfCharac.Text = "Tên: ...\nSức mạnh: ...\nTốc độ: ...\nKỹ năng: ...";
            // 
            // characterReviewPic
            // 
            characterReviewPic.BackColor = Color.LemonChiffon;
            characterReviewPic.ErrorImage = (Image)resources.GetObject("characterReviewPic.ErrorImage");
            characterReviewPic.Image = (Image)resources.GetObject("characterReviewPic.Image");
            characterReviewPic.InitialImage = (Image)resources.GetObject("characterReviewPic.InitialImage");
            characterReviewPic.Location = new Point(27, 16);
            characterReviewPic.Name = "characterReviewPic";
            characterReviewPic.Size = new Size(305, 338);
            characterReviewPic.SizeMode = PictureBoxSizeMode.Zoom;
            characterReviewPic.TabIndex = 0;
            characterReviewPic.TabStop = false;
            // 
            // panel2
            // 
            panel2.BackColor = Color.SteelBlue;
            panel2.Controls.Add(selectRyu);
            panel2.Controls.Add(pictureBox3);
            panel2.Controls.Add(pictureBox2);
            panel2.Controls.Add(selectKen);
            panel2.Location = new Point(56, 73);
            panel2.Name = "panel2";
            panel2.Size = new Size(520, 495);
            panel2.TabIndex = 3;
            // 
            // selectRyu
            // 
            selectRyu.BackColor = Color.Transparent;
            selectRyu.BorderStyle = BorderStyle.Fixed3D;
            selectRyu.Image = (Image)resources.GetObject("selectRyu.Image");
            selectRyu.Location = new Point(26, 17);
            selectRyu.Name = "selectRyu";
            selectRyu.Size = new Size(210, 215);
            selectRyu.SizeMode = PictureBoxSizeMode.StretchImage;
            selectRyu.TabIndex = 13;
            selectRyu.TabStop = false;
            selectRyu.Click += selectRyu_Click;
            selectRyu.MouseEnter += selectRyu_MouseEnter;
            selectRyu.MouseLeave += selectRyu_MouseLeave;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.DimGray;
            pictureBox3.BorderStyle = BorderStyle.Fixed3D;
            pictureBox3.Location = new Point(276, 258);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(213, 215);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 12;
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.DimGray;
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;
            pictureBox2.Location = new Point(26, 258);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(209, 215);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 11;
            pictureBox2.TabStop = false;
            // 
            // selectKen
            // 
            selectKen.BackColor = Color.Transparent;
            selectKen.BorderStyle = BorderStyle.Fixed3D;
            selectKen.Image = Properties.Resources.KenF;
            selectKen.Location = new Point(276, 17);
            selectKen.Name = "selectKen";
            selectKen.Size = new Size(210, 215);
            selectKen.SizeMode = PictureBoxSizeMode.StretchImage;
            selectKen.TabIndex = 10;
            selectKen.TabStop = false;
            selectKen.Click += selectKen_Click_1;
            selectKen.MouseEnter += selectKen_MouseEnter_1;
            selectKen.MouseLeave += selectKen_MouseLeave_1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Showcard Gothic", 19.8000011F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Crimson;
            label1.Location = new Point(379, 9);
            label1.Name = "label1";
            label1.Size = new Size(291, 43);
            label1.TabIndex = 4;
            label1.Text = "Chọn nhân vật";
            // 
            // CharacterSelection
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.image;
            ClientSize = new Size(1026, 592);
            Controls.Add(label1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "CharacterSelection";
            Text = "CharacterSelection";
            Load += CharacterSelection_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)characterReviewPic).EndInit();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)selectRyu).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)selectKen).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private RichTextBox txtInfCharac;
        private PictureBox characterReviewPic;
        private Panel panel2;
        private PictureBox selectKen;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private PictureBox selectRyu;
        private Label label1;
    }
}