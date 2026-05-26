using System;
using System.Windows.Forms;
using BattleGame.Client.Managers;

namespace BattleGame.Client.Forms
{
    internal sealed partial class AudioSettingsForm : Form
    {
        public AudioSettingsForm()
        {
            InitializeComponent();
            BorderlessFormHelper.Apply(this);
            LoadValues();
            SoundManager.SettingsChanged += SoundManager_SettingsChanged;
        }

        // ── Load current values from SoundManager ───────────────────
        private void LoadValues()
        {
            bool design = System.ComponentModel.LicenseManager.UsageMode ==
                          System.ComponentModel.LicenseUsageMode.Designtime;
            if (design) return;

            trackBarMusic.Value = Math.Clamp((int)Math.Round(SoundManager.MusicVolume * 100), 0, 100);
            trackBarSfx.Value = Math.Clamp((int)Math.Round(SoundManager.SfxVolume * 100), 0, 100);

            comboTrack.Items.Clear();
            foreach (string bgm in SoundManager.GetAvailableBgmFiles())
                comboTrack.Items.Add(bgm);

            if (comboTrack.Items.Count > 0)
                comboTrack.SelectedIndex = Math.Max(0,
                    comboTrack.Items.IndexOf(SoundManager.PreferredBgm));

            UpdateMusicLabel(trackBarMusic.Value);
            UpdateSfxLabel(trackBarSfx.Value);
        }

        // ── Events ───────────────────────────────────────────────────
        private void trackBarMusic_Scroll(object sender, EventArgs e)
        {
            float v = trackBarMusic.Value / 100f;
            SoundManager.SetMusicVolume(v);
            UpdateMusicLabel(trackBarMusic.Value);
        }

        private void trackBarSfx_Scroll(object sender, EventArgs e)
        {
            float v = trackBarSfx.Value / 100f;
            SoundManager.SetSfxVolume(v);
            UpdateSfxLabel(trackBarSfx.Value);
        }

        private void comboTrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboTrack.SelectedItem is string bgm)
                SoundManager.SetPreferredBgm(bgm);
        }

        private void btnTest_Click(object sender, EventArgs e)
            => SoundManager.PlayButtonClick();

        private void btnDone_Click(object sender, EventArgs e)
            => Close();

        // ── Helpers ──────────────────────────────────────────────────
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SoundManager.SettingsChanged -= SoundManager_SettingsChanged;
            base.OnFormClosed(e);
        }

        private void SoundManager_SettingsChanged(object? sender, EventArgs e)
        {
            if (IsDisposed)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SoundManager_SettingsChanged(sender, e)));
                return;
            }

            trackBarMusic.Value = Math.Clamp((int)Math.Round(SoundManager.MusicVolume * 100), 0, 100);
            trackBarSfx.Value = Math.Clamp((int)Math.Round(SoundManager.SfxVolume * 100), 0, 100);
            UpdateMusicLabel(trackBarMusic.Value);
            UpdateSfxLabel(trackBarSfx.Value);
        }

        private void UpdateMusicLabel(int value) => lblMusicValue.Text = $"{value}%";
        private void UpdateSfxLabel(int value) => lblSfxValue.Text = $"{value}%";
    }
}
