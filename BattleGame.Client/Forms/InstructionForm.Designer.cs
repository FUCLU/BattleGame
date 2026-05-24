namespace BattleGame.Client.Forms
{
    partial class InstructionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstructionForm));
            pnlMain = new Panel();
            tabControl = new TabControl();
            tabOffline = new TabPage();
            tabOnline = new TabPage();
            btnBack = new Button();
            pnlMain.SuspendLayout();
            tabControl.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlMain.BackgroundImage = (Image)resources.GetObject("pnlMain.BackgroundImage");
            pnlMain.BackgroundImageLayout = ImageLayout.Stretch;
            pnlMain.Controls.Add(tabControl);
            pnlMain.Controls.Add(btnBack);
            pnlMain.Location = new Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(900, 600);
            pnlMain.TabIndex = 0;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabOffline);
            tabControl.Controls.Add(tabOnline);
            tabControl.Font = new Font("Courier New", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tabControl.ItemSize = new Size(180, 42);
            tabControl.Location = new Point(24, 18);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(852, 500);
            tabControl.TabIndex = 1;
            // 
            // tabOffline
            // 
            tabOffline.Location = new Point(4, 46);
            tabOffline.Name = "tabOffline";
            tabOffline.Padding = new Padding(3);
            tabOffline.Size = new Size(844, 450);
            tabOffline.TabIndex = 0;
            tabOffline.Text = "Offline";
            tabOffline.UseVisualStyleBackColor = true;
            // 
            // tabOnline
            // 
            tabOnline.BackColor = SystemColors.Info;
            tabOnline.Location = new Point(4, 46);
            tabOnline.Name = "tabOnline";
            tabOnline.Padding = new Padding(3);
            tabOnline.Size = new Size(844, 450);
            tabOnline.TabIndex = 1;
            tabOnline.Text = "Online";
            // 
            // btnBack
            // 
            btnBack.BackgroundImage = (Image)resources.GetObject("btnBack.BackgroundImage");
            btnBack.BackgroundImageLayout = ImageLayout.Stretch;
            btnBack.FlatStyle = FlatStyle.Popup;
            btnBack.Font = new Font("Courier New", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBack.Location = new Point(370, 532);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(160, 52);
            btnBack.TabIndex = 0;
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // InstructionForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(900, 600);
            Controls.Add(pnlMain);
            Name = "InstructionForm";
            Text = "InstructionForm";
            pnlMain.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMain;
        private Button btnBack;
        private TabControl tabControl;
        private TabPage tabOffline;
        private TabPage tabOnline;
    }
}
