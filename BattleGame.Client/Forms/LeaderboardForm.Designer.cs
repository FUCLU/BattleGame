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
            listView1 = new ListView();
            button1 = new Button();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.BackColor = Color.FromArgb(232, 248, 255);
            listView1.BorderStyle = BorderStyle.FixedSingle;
            listView1.Font = new Font("Courier New", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            listView1.ForeColor = Color.FromArgb(12, 55, 116);
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(109, 134);
            listView1.Name = "listView1";
            listView1.OwnerDraw = false;
            listView1.Size = new Size(844, 368);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            listView1.Resize += listView1_Resize;
            // 
            // button1
            // 
            button1.BackColor = Color.Brown;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderColor = Color.White;
            button1.FlatAppearance.BorderSize = 2;
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(120, 28, 28);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 65, 65);
            button1.Font = new Font("Book Antiqua", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.Control;
            button1.Location = new Point(1000, 5);
            button1.Name = "button1";
            button1.Size = new Size(50, 36);
            button1.TabIndex = 2;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // LeaderboardForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Honeydew;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1057, 581);
            Controls.Add(button1);
            Controls.Add(listView1);
            Name = "LeaderboardForm";
            Text = "LeaderboardForm";
            Load += LeaderboardForm_Load;
            ResumeLayout(false);
        }

        #endregion
        private ListView listView1;
        private Button button1;
    }
}
