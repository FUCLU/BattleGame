using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BattleGame.Client.Managers;

namespace BattleGame.Client.Forms
{
    internal sealed class AudioSettingsForm : Form
    {
        private readonly VolumeSlider _musicSlider = new();
        private readonly VolumeSlider _sfxSlider = new();
        private readonly ComboBox _musicCombo = new();
        private readonly Label _musicValue = new();
        private readonly Label _sfxValue = new();

        public AudioSettingsForm()
        {
            InitializeSettingsForm();
        }

        private void InitializeSettingsForm()
        {
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(600, 392);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(8, 14, 31);
            DoubleBuffered = true;
            Padding = new Padding(24);

            var titlePanel = new Panel
            {
                BackColor = Color.FromArgb(19, 42, 78),
                Location = new Point(72, 18),
                Size = new Size(456, 58)
            };
            titlePanel.Paint += (_, e) =>
            {
                using var top = new Pen(Color.FromArgb(151, 240, 255), 2);
                using var bottom = new Pen(Color.FromArgb(255, 235, 156), 1);
                using var edge = new Pen(Color.FromArgb(39, 101, 158), 2);
                e.Graphics.DrawRectangle(edge, 0, 0, titlePanel.Width - 1, titlePanel.Height - 1);
                e.Graphics.DrawLine(top, 8, 6, titlePanel.Width - 9, 6);
                e.Graphics.DrawLine(bottom, 8, titlePanel.Height - 7, titlePanel.Width - 9, titlePanel.Height - 7);
            };

            var title = new Label
            {
                Text = "AUDIO SETTINGS",
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(178, 255, 250),
                Font = new Font("Book Antiqua", 21F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 7),
                Size = new Size(456, 42)
            };
            titlePanel.Controls.Add(title);

            var musicRow = CreateRowPanel(40, 98);
            var sfxRow = CreateRowPanel(40, 168);
            var trackRow = CreateRowPanel(40, 238);

            musicRow.Controls.Add(CreateLabel("MUSIC", 18));
            sfxRow.Controls.Add(CreateLabel("BUTTON", 18));
            trackRow.Controls.Add(CreateLabel("TRACK", 18));

            ConfigureSlider(_musicSlider, SoundManager.MusicVolume);
            ConfigureSlider(_sfxSlider, SoundManager.SfxVolume);
            ConfigureValue(_musicValue, SoundManager.MusicVolume);
            ConfigureValue(_sfxValue, SoundManager.SfxVolume);

            _musicSlider.Location = new Point(168, 17);
            _sfxSlider.Location = new Point(168, 17);
            _musicValue.Location = new Point(436, 16);
            _sfxValue.Location = new Point(436, 16);

            _musicSlider.ValueChanged += (_, _) =>
            {
                float volume = _musicSlider.Value / 100f;
                SoundManager.SetMusicVolume(volume);
                UpdateValue(_musicValue, volume);
            };

            _sfxSlider.ValueChanged += (_, _) =>
            {
                float volume = _sfxSlider.Value / 100f;
                SoundManager.SetSfxVolume(volume);
                UpdateValue(_sfxValue, volume);
            };

            musicRow.Controls.Add(_musicSlider);
            musicRow.Controls.Add(_musicValue);
            sfxRow.Controls.Add(_sfxSlider);
            sfxRow.Controls.Add(_sfxValue);

            ConfigureMusicCombo();
            _musicCombo.Location = new Point(168, 16);
            trackRow.Controls.Add(_musicCombo);

            var testButton = CreateButton("TEST", new Point(168, 322), DialogResult.None);
            testButton.Click += (_, _) => SoundManager.PlayButtonClick();

            var closeButton = CreateButton("DONE", new Point(312, 322), DialogResult.OK);

            Controls.Add(titlePanel);
            Controls.Add(musicRow);
            Controls.Add(sfxRow);
            Controls.Add(trackRow);
            Controls.Add(testButton);
            Controls.Add(closeButton);

            AcceptButton = closeButton;
            CancelButton = closeButton;
        }

        private static Panel CreateRowPanel(int x, int y)
        {
            var panel = new Panel
            {
                BackColor = Color.FromArgb(18, 31, 62),
                Location = new Point(x, y),
                Size = new Size(520, 56)
            };
            panel.Paint += (_, e) =>
            {
                using var shadow = new Pen(Color.FromArgb(2, 7, 19), 3);
                using var border = new Pen(Color.FromArgb(63, 143, 198), 2);
                using var highlight = new Pen(Color.FromArgb(151, 240, 255), 1);
                using var glow = new SolidBrush(Color.FromArgb(20, 58, 98));
                e.Graphics.FillRectangle(glow, 4, 4, panel.Width - 8, panel.Height - 8);
                e.Graphics.DrawRectangle(shadow, 1, 1, panel.Width - 3, panel.Height - 3);
                e.Graphics.DrawRectangle(border, 0, 0, panel.Width - 1, panel.Height - 1);
                e.Graphics.DrawLine(highlight, 8, 6, panel.Width - 9, 6);
            };
            return panel;
        }

        private static Label CreateLabel(string text, int x)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(255, 235, 156),
                Font = new Font("Bookman Old Style", 12.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(x, 14),
                Size = new Size(120, 28)
            };
        }

        private static void ConfigureSlider(VolumeSlider slider, float value)
        {
            slider.Size = new Size(230, 26);
            slider.Value = Math.Clamp((int)Math.Round(value * 100), 0, 100);
        }

        private static void ConfigureValue(Label label, float value)
        {
            label.AutoSize = false;
            label.BackColor = Color.Transparent;
            label.ForeColor = Color.WhiteSmoke;
            label.Font = new Font("Consolas", 12F, FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleRight;
            label.Size = new Size(62, 28);
            UpdateValue(label, value);
        }

        private void ConfigureMusicCombo()
        {
            _musicCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _musicCombo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _musicCombo.Size = new Size(300, 28);
            _musicCombo.BackColor = Color.FromArgb(238, 245, 255);
            _musicCombo.ForeColor = Color.FromArgb(16, 24, 48);

            foreach (string bgm in SoundManager.GetAvailableBgmFiles())
                _musicCombo.Items.Add(bgm);

            if (_musicCombo.Items.Count > 0)
            {
                int selectedIndex = Math.Max(0, _musicCombo.Items.IndexOf(SoundManager.PreferredBgm));
                _musicCombo.SelectedIndex = selectedIndex;
            }

            _musicCombo.SelectedIndexChanged += (_, _) =>
            {
                if (_musicCombo.SelectedItem is string bgm)
                    SoundManager.SetPreferredBgm(bgm);
            };
        }

        private static void UpdateValue(Label label, float value)
        {
            label.Text = $"{Math.Clamp((int)Math.Round(value * 100), 0, 100)}%";
        }

        private static Button CreateButton(string text, Point location, DialogResult result)
        {
            var button = new Button
            {
                Text = text,
                DialogResult = result,
                BackColor = Color.FromArgb(39, 92, 139),
                ForeColor = Color.FromArgb(255, 235, 156),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Courier New", 13F, FontStyle.Bold),
                Location = location,
                Size = new Size(120, 44),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderColor = Color.PaleTurquoise;
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(54, 126, 182);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(20, 55, 96);
            return button;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.None;

            using var fill = new SolidBrush(Color.FromArgb(11, 20, 45));
            using var outer = new Pen(Color.FromArgb(2, 7, 19), 8);
            using var inner = new Pen(Color.FromArgb(137, 230, 245), 3);
            using var accent = new Pen(Color.FromArgb(255, 235, 156), 1);
            using var corner = new Pen(Color.FromArgb(255, 235, 156), 3);
            var bounds = new Rectangle(4, 4, ClientSize.Width - 9, ClientSize.Height - 9);
            e.Graphics.FillRectangle(fill, bounds);
            e.Graphics.DrawRectangle(outer, bounds);
            bounds.Inflate(-10, -10);
            e.Graphics.DrawRectangle(inner, bounds);
            bounds.Inflate(-7, -7);
            e.Graphics.DrawRectangle(accent, bounds);

            DrawCorner(e.Graphics, corner, bounds.Left, bounds.Top, 22, 1, 1);
            DrawCorner(e.Graphics, corner, bounds.Right, bounds.Top, 22, -1, 1);
            DrawCorner(e.Graphics, corner, bounds.Left, bounds.Bottom, 22, 1, -1);
            DrawCorner(e.Graphics, corner, bounds.Right, bounds.Bottom, 22, -1, -1);
        }

        private static void DrawCorner(Graphics g, Pen pen, int x, int y, int length, int sx, int sy)
        {
            g.DrawLine(pen, x, y, x + sx * length, y);
            g.DrawLine(pen, x, y, x, y + sy * length);
        }

        private sealed class VolumeSlider : Control
        {
            private int _value;
            private bool _dragging;

            public event EventHandler? ValueChanged;

            public int Value
            {
                get => _value;
                set
                {
                    int next = Math.Clamp(value, 0, 100);
                    if (_value == next)
                        return;

                    _value = next;
                    Invalidate();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public VolumeSlider()
            {
                DoubleBuffered = true;
                Cursor = Cursors.Hand;
                TabStop = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.None;

                var track = GetTrackRect();
                using var empty = new SolidBrush(Color.FromArgb(48, 60, 82));
                using var fill = new SolidBrush(Color.FromArgb(28, 156, 238));
                using var border = new Pen(Color.FromArgb(210, 226, 245), 1);
                using var tickPen = new Pen(Color.FromArgb(92, 122, 156), 1);

                e.Graphics.FillRectangle(empty, track);
                int fillWidth = Math.Max(0, (int)Math.Round(track.Width * Value / 100f));
                if (fillWidth > 0)
                    e.Graphics.FillRectangle(fill, new Rectangle(track.X, track.Y, fillWidth, track.Height));
                e.Graphics.DrawRectangle(border, track);

                for (int i = 1; i < 10; i++)
                {
                    int x = track.X + i * track.Width / 10;
                    e.Graphics.DrawLine(tickPen, x, track.Bottom + 5, x, track.Bottom + 8);
                }

                int knobX = track.X + fillWidth;
                var knob = new Rectangle(knobX - 6, track.Y - 5, 12, track.Height + 10);
                using var knobFill = new SolidBrush(Color.FromArgb(255, 235, 156));
                using var knobEdge = new Pen(Color.FromArgb(7, 12, 30), 2);
                e.Graphics.FillRectangle(knobFill, knob);
                e.Graphics.DrawRectangle(knobEdge, knob);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                _dragging = true;
                Capture = true;
                Focus();
                SetValueFromMouse(e.X);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (_dragging)
                    SetValueFromMouse(e.X);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                _dragging = false;
                Capture = false;
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Down)
                    Value -= 5;
                else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Up)
                    Value += 5;
            }

            private Rectangle GetTrackRect()
                => new(6, 7, Width - 12, 9);

            private void SetValueFromMouse(int mouseX)
            {
                var track = GetTrackRect();
                Value = (int)Math.Round((mouseX - track.X) * 100f / Math.Max(1, track.Width));
            }
        }
    }
}
