namespace BattleGame.Client.Forms
{
    partial class RoundStartForm
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
            panelRound = new Panel();
            label1 = new Label();
            panelRound.SuspendLayout();
            SuspendLayout();
            // 
            // panelRound
            // 
            panelRound.Controls.Add(label1);
            panelRound.Location = new Point(300, 180);
            panelRound.Name = "panelRound";
            panelRound.Size = new Size(482, 255);
            panelRound.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Book Antiqua", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Tomato;
            label1.Location = new Point(82, 77);
            label1.Name = "label1";
            label1.Size = new Size(317, 73);
            label1.TabIndex = 0;
            label1.Text = "ROUND 1";
            // 
            SuspendLayout();
            // 
            // RoundStartForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1057, 581);
            Controls.Add(panelRound);
            DoubleBuffered = true;
            Name = "RoundStartForm";
            Text = "RoundStartForm";
            Load += RoundStartForm_Load;
            panelRound.ResumeLayout(false);
            panelRound.PerformLayout();
            ClientSize = new Size(1057, 581);
            Name = "RoundStartForm";
            Text = "RoundStartForm";
            ResumeLayout(false);
        }

        #endregion
        private Panel panelRound;
        private Label label1;
    }
}