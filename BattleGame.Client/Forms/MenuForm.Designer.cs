namespace BattleGame.Client.Forms
{
    partial class MenuForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            button1 = new Button();
            button3 = new Button();
            btnSetting = new GearButton();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = Color.Transparent;
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button1.FlatStyle = FlatStyle.Popup;
            button1.Location = new Point(396, 274);
            button1.Name = "button1";
            button1.Size = new Size(252, 70);
            button1.TabIndex = 4;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.Transparent;
            button3.BackgroundImage = (Image)resources.GetObject("button3.BackgroundImage");
            button3.BackgroundImageLayout = ImageLayout.Stretch;
            button3.FlatStyle = FlatStyle.Popup;
            button3.Location = new Point(398, 390);
            button3.Name = "button3";
            button3.Size = new Size(250, 70);
            button3.TabIndex = 5;
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // btnSetting
            // 
            btnSetting.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSetting.BackColor = Color.Black;
            btnSetting.FlatStyle = FlatStyle.Popup;
            btnSetting.ForeColor = Color.FromArgb(255, 235, 156);
            btnSetting.Location = new Point(985, 511);
            btnSetting.Name = "btnSetting";
            btnSetting.Size = new Size(48, 48);
            btnSetting.TabIndex = 6;
            btnSetting.UseVisualStyleBackColor = false;
            // 
            // MenuForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(1057, 581);
            Controls.Add(btnSetting);
            Controls.Add(button3);
            Controls.Add(button1);
            DoubleBuffered = true;
            Name = "MenuForm";
            Text = "MenuForm";
            Load += MenuForm_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private Button button3;
        private GearButton btnSetting;
    }
}
