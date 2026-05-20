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
            pnlMain.Location = new Point(-2, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(803, 466);
            pnlMain.TabIndex = 0;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabOffline);
            tabControl.Controls.Add(tabOnline);
            tabControl.Font = new Font("Lucida Bright", 12F, FontStyle.Italic, GraphicsUnit.Point, 0);
            tabControl.Location = new Point(28, 14);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(753, 400);
            tabControl.TabIndex = 1;
            // 
            // tabOffline
            // 
            tabOffline.Location = new Point(4, 32);
            tabOffline.Name = "tabOffline";
            tabOffline.Padding = new Padding(3);
            tabOffline.Size = new Size(745, 364);
            tabOffline.TabIndex = 0;
            tabOffline.Text = "Offline";
            tabOffline.UseVisualStyleBackColor = true;
            // 
            // tabOnline
            // 
            tabOnline.BackColor = SystemColors.Info;
            tabOnline.Location = new Point(4, 32);
            tabOnline.Name = "tabOnline";
            tabOnline.Padding = new Padding(3);
            tabOnline.Size = new Size(745, 364);
            tabOnline.TabIndex = 1;
            tabOnline.Text = "Online";
            // 
            // btnBack
            // 
            btnBack.BackgroundImage = (Image)resources.GetObject("btnBack.BackgroundImage");
            btnBack.BackgroundImageLayout = ImageLayout.Stretch;
            btnBack.FlatStyle = FlatStyle.Popup;
            btnBack.Location = new Point(326, 418);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(127, 45);
            btnBack.TabIndex = 0;
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // InstructionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 464);
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