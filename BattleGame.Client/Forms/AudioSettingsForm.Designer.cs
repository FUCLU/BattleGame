using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    internal sealed partial class AudioSettingsForm
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // ── Controls ─────────────────────────────────────────────
            this.panelTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelMusic = new System.Windows.Forms.Panel();
            this.lblMusic = new System.Windows.Forms.Label();
            this.trackBarMusic = new System.Windows.Forms.TrackBar();
            this.lblMusicValue = new System.Windows.Forms.Label();
            this.panelSfx = new System.Windows.Forms.Panel();
            this.lblSfx = new System.Windows.Forms.Label();
            this.trackBarSfx = new System.Windows.Forms.TrackBar();
            this.lblSfxValue = new System.Windows.Forms.Label();
            this.panelTrack = new System.Windows.Forms.Panel();
            this.lblTrack = new System.Windows.Forms.Label();
            this.comboTrack = new System.Windows.Forms.ComboBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();

            this.panelTitle.SuspendLayout();
            this.panelMusic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.trackBarMusic).BeginInit();
            this.panelSfx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.trackBarSfx).BeginInit();
            this.panelTrack.SuspendLayout();
            this.SuspendLayout();

            // ── Palette (static for Designer visibility) ─────────────
            var bgDeep = System.Drawing.Color.FromArgb(10, 14, 30);
            var bgPanel = System.Drawing.Color.FromArgb(18, 26, 54);
            var bgRow = System.Drawing.Color.FromArgb(22, 32, 64);
            var borderBlue = System.Drawing.Color.FromArgb(60, 140, 210);
            var accentGold = System.Drawing.Color.FromArgb(255, 220, 100);
            var textTitle = System.Drawing.Color.FromArgb(200, 240, 255);
            var textLabel = System.Drawing.Color.FromArgb(180, 200, 240);
            var textValue = System.Drawing.Color.FromArgb(255, 220, 100);
            var trackBg = System.Drawing.Color.FromArgb(28, 40, 76);

            // ── panelTitle ───────────────────────────────────────────
            this.panelTitle.BackColor = bgPanel;
            this.panelTitle.Controls.Add(this.lblTitle);
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Size = new System.Drawing.Size(600, 56);
            this.panelTitle.TabIndex = 0;
            // bottom border painted at runtime via BackgroundImage trick → use Paint in .cs if needed

            // ── lblTitle ─────────────────────────────────────────────
            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.ForeColor = textTitle;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(56, 10);
            this.lblTitle.Size = new System.Drawing.Size(300, 36);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "♪  AUDIO SETTINGS";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── panelMusic ───────────────────────────────────────────
            this.panelMusic.BackColor = bgRow;
            this.panelMusic.Controls.Add(this.lblMusic);
            this.panelMusic.Controls.Add(this.trackBarMusic);
            this.panelMusic.Controls.Add(this.lblMusicValue);
            this.panelMusic.Location = new System.Drawing.Point(24, 72);
            this.panelMusic.Size = new System.Drawing.Size(552, 60);
            this.panelMusic.TabIndex = 1;

            // ── lblMusic ─────────────────────────────────────────────
            this.lblMusic.AutoSize = false;
            this.lblMusic.BackColor = System.Drawing.Color.Transparent;
            this.lblMusic.ForeColor = textLabel;
            this.lblMusic.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMusic.Location = new System.Drawing.Point(16, 18);
            this.lblMusic.Size = new System.Drawing.Size(148, 24);
            this.lblMusic.TabIndex = 0;
            this.lblMusic.Text = "Music Volume";
            this.lblMusic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── trackBarMusic ─────────────────────────────────────────
            this.trackBarMusic.AutoSize = false;
            this.trackBarMusic.BackColor = bgRow;
            this.trackBarMusic.Location = new System.Drawing.Point(170, 15);
            this.trackBarMusic.Maximum = 100;
            this.trackBarMusic.Minimum = 0;
            this.trackBarMusic.Size = new System.Drawing.Size(310, 30);
            this.trackBarMusic.TabIndex = 1;
            this.trackBarMusic.TickFrequency = 10;
            this.trackBarMusic.Value = 70;
            this.trackBarMusic.Scroll += new System.EventHandler(this.trackBarMusic_Scroll);

            // ── lblMusicValue ────────────────────────────────────────
            this.lblMusicValue.AutoSize = false;
            this.lblMusicValue.BackColor = System.Drawing.Color.Transparent;
            this.lblMusicValue.ForeColor = textValue;
            this.lblMusicValue.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold);
            this.lblMusicValue.Location = new System.Drawing.Point(492, 18);
            this.lblMusicValue.Size = new System.Drawing.Size(50, 24);
            this.lblMusicValue.TabIndex = 2;
            this.lblMusicValue.Text = "70%";
            this.lblMusicValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── panelSfx ─────────────────────────────────────────────
            this.panelSfx.BackColor = bgRow;
            this.panelSfx.Controls.Add(this.lblSfx);
            this.panelSfx.Controls.Add(this.trackBarSfx);
            this.panelSfx.Controls.Add(this.lblSfxValue);
            this.panelSfx.Location = new System.Drawing.Point(24, 148);
            this.panelSfx.Size = new System.Drawing.Size(552, 60);
            this.panelSfx.TabIndex = 2;

            // ── lblSfx ───────────────────────────────────────────────
            this.lblSfx.AutoSize = false;
            this.lblSfx.BackColor = System.Drawing.Color.Transparent;
            this.lblSfx.ForeColor = textLabel;
            this.lblSfx.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblSfx.Location = new System.Drawing.Point(16, 18);
            this.lblSfx.Size = new System.Drawing.Size(148, 24);
            this.lblSfx.TabIndex = 0;
            this.lblSfx.Text = "Button Sounds";
            this.lblSfx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── trackBarSfx ───────────────────────────────────────────
            this.trackBarSfx.AutoSize = false;
            this.trackBarSfx.BackColor = bgRow;
            this.trackBarSfx.Location = new System.Drawing.Point(170, 15);
            this.trackBarSfx.Maximum = 100;
            this.trackBarSfx.Minimum = 0;
            this.trackBarSfx.Size = new System.Drawing.Size(310, 30);
            this.trackBarSfx.TabIndex = 1;
            this.trackBarSfx.TickFrequency = 10;
            this.trackBarSfx.Value = 50;
            this.trackBarSfx.Scroll += new System.EventHandler(this.trackBarSfx_Scroll);

            // ── lblSfxValue ──────────────────────────────────────────
            this.lblSfxValue.AutoSize = false;
            this.lblSfxValue.BackColor = System.Drawing.Color.Transparent;
            this.lblSfxValue.ForeColor = textValue;
            this.lblSfxValue.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold);
            this.lblSfxValue.Location = new System.Drawing.Point(492, 18);
            this.lblSfxValue.Size = new System.Drawing.Size(50, 24);
            this.lblSfxValue.TabIndex = 2;
            this.lblSfxValue.Text = "50%";
            this.lblSfxValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── panelTrack ───────────────────────────────────────────
            this.panelTrack.BackColor = bgRow;
            this.panelTrack.Controls.Add(this.lblTrack);
            this.panelTrack.Controls.Add(this.comboTrack);
            this.panelTrack.Location = new System.Drawing.Point(24, 224);
            this.panelTrack.Size = new System.Drawing.Size(552, 60);
            this.panelTrack.TabIndex = 3;

            // ── lblTrack ─────────────────────────────────────────────
            this.lblTrack.AutoSize = false;
            this.lblTrack.BackColor = System.Drawing.Color.Transparent;
            this.lblTrack.ForeColor = textLabel;
            this.lblTrack.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTrack.Location = new System.Drawing.Point(16, 18);
            this.lblTrack.Size = new System.Drawing.Size(148, 24);
            this.lblTrack.TabIndex = 0;
            this.lblTrack.Text = "Background Track";
            this.lblTrack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── comboTrack ───────────────────────────────────────────
            this.comboTrack.BackColor = trackBg;
            this.comboTrack.ForeColor = textLabel;
            this.comboTrack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTrack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboTrack.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.comboTrack.Location = new System.Drawing.Point(170, 17);
            this.comboTrack.Size = new System.Drawing.Size(310, 26);
            this.comboTrack.TabIndex = 1;
            this.comboTrack.SelectedIndexChanged += new System.EventHandler(this.comboTrack_SelectedIndexChanged);

            // ── btnTest ──────────────────────────────────────────────
            this.btnTest.BackColor = System.Drawing.Color.FromArgb(28, 60, 110);
            this.btnTest.ForeColor = accentGold;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnTest.Location = new System.Drawing.Point(170, 320);
            this.btnTest.Size = new System.Drawing.Size(124, 40);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "▶  Test";
            this.btnTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTest.FlatAppearance.BorderColor = borderBlue;
            this.btnTest.FlatAppearance.BorderSize = 1;
            this.btnTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(40, 80, 140);
            this.btnTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(16, 40, 80);
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);

            // ── btnDone ──────────────────────────────────────────────
            this.btnDone.BackColor = System.Drawing.Color.FromArgb(28, 60, 110);
            this.btnDone.ForeColor = accentGold;
            this.btnDone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDone.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDone.Location = new System.Drawing.Point(306, 320);
            this.btnDone.Size = new System.Drawing.Size(124, 40);
            this.btnDone.TabIndex = 5;
            this.btnDone.Text = "✔  Done";
            this.btnDone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDone.FlatAppearance.BorderColor = borderBlue;
            this.btnDone.FlatAppearance.BorderSize = 1;
            this.btnDone.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(40, 80, 140);
            this.btnDone.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(16, 40, 80);
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);

            // ── Form ─────────────────────────────────────────────────
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = bgDeep;
            this.ClientSize = new System.Drawing.Size(600, 380);
            this.Controls.Add(this.panelTitle);
            this.Controls.Add(this.panelMusic);
            this.Controls.Add(this.panelSfx);
            this.Controls.Add(this.panelTrack);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnDone);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AudioSettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audio Settings";

            this.panelTitle.ResumeLayout(false);
            this.panelMusic.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.trackBarMusic).EndInit();
            this.panelSfx.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.trackBarSfx).EndInit();
            this.panelTrack.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // ── Control declarations ──────────────────────────────────────
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelMusic;
        private System.Windows.Forms.Label lblMusic;
        private System.Windows.Forms.TrackBar trackBarMusic;
        private System.Windows.Forms.Label lblMusicValue;
        private System.Windows.Forms.Panel panelSfx;
        private System.Windows.Forms.Label lblSfx;
        private System.Windows.Forms.TrackBar trackBarSfx;
        private System.Windows.Forms.Label lblSfxValue;
        private System.Windows.Forms.Panel panelTrack;
        private System.Windows.Forms.Label lblTrack;
        private System.Windows.Forms.ComboBox comboTrack;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnDone;
    }
}
