using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BattleGame.Client.Config;

namespace BattleGame.Client.Forms
{
    public partial class CharacterSelection : Form
    {
        private readonly Dictionary<Panel, CharacterSelectionItem> _panelCharacterMap = new();
        private readonly List<CharacterSelectionItem> _availableCharacters = new();
        private readonly List<Panel> _slotPanels = new();

        private CharacterSelectionItem? _selectedCharacter;

        private int _maxHp = 1;
        private int _maxAtk = 1;
        private int _maxDef = 1;
        private int _maxSpd = 1;

        // ─── Màu nền slot ────────────────────────────────────────────────────
        private static readonly Color SlotNormalColor = Color.FromArgb(44, 74, 110);
        private static readonly Color SlotHoverColor = Color.FromArgb(55, 95, 140);
        private static readonly Color SlotSelectedColor = Color.FromArgb(63, 110, 165);

        // ─── Giao diện slot trong FlowLayoutPanel ────────────────────────────
        private const int PanelPadding = 14;
        private const int SlotHeight = 92;
        private const int SlotSpacing = 10;

        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets");

        private static readonly string PortraitRoot = Path.Combine(AssetsRoot, "PotraitPic");
        private static readonly string CharactersRoot = Path.Combine(AssetsRoot, "Characters");

        public CharacterSelection()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            // Khi FlowLayoutPanel đổi kích thước thì slot tự giãn theo
            flpnlSelChar.Resize += flpnlSelChar_Resize;

        }

        public string SelectedCharacterId { get; private set; } = string.Empty;
        public string SelectedCharacterName => SelectedCharacterId;

        private void CharacterSelection_Load(object sender, EventArgs e)
        {

            SetupCharacterListFlowPanel();

            LoadCharacters();

            // Đợi Form layout xong rồi mới bind slot
            BeginInvoke(new Action(() =>
            {
                BindCharacterSlots();
                ApplySlotWidthToAll();
            }));
        }

        // ─── Setup FlowLayoutPanel ────────────────────────────────────────────

        private void SetupCharacterListFlowPanel()
        {
            flpnlSelChar.AutoScroll = true;
            flpnlSelChar.FlowDirection = FlowDirection.TopDown;
            flpnlSelChar.WrapContents = false;
            flpnlSelChar.Padding = new Padding(PanelPadding);
        }

        private void flpnlSelChar_Resize(object? sender, EventArgs e)
        {
            if (_slotPanels.Count == 0) return;

            ApplySlotWidthToAll();
        }
        private int GetSlotWidth()
        {
            int scrollBarWidth = SystemInformation.VerticalScrollBarWidth;

            int slotW = flpnlSelChar.ClientSize.Width
                        - flpnlSelChar.Padding.Left
                        - flpnlSelChar.Padding.Right
                        - scrollBarWidth
                        - 4;

            return Math.Max(350, slotW);
        }



        private void ApplySlotWidthToAll()
        {
            int slotW = GetSlotWidth();

            foreach (var slot in _slotPanels)
            {
                slot.Width = slotW;
                slot.Height = SlotHeight;
                slot.Margin = new Padding(0, 0, 0, SlotSpacing);

                ResizeSlotChildren(slot);
            }
        }

        private void ResizeSlotChildren(Panel slot)
        {
            if (slot.Controls.Count == 0) return;

            PictureBox? picture = slot.Controls.OfType<PictureBox>().FirstOrDefault();
            if (picture == null) return;

            int textX = picture.Right + 10;
            int textAreaWidth = slot.Width - textX - 10;

            foreach (Control child in slot.Controls)
            {
                if (child is Label lbl)
                {
                    lbl.Width = textAreaWidth;
                }
            }
        }

        // ─── Load dữ liệu nhân vật ────────────────────────────────────────────

        private void LoadCharacters()
        {
            _availableCharacters.Clear();

            string configRoot = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..");

            var catalogItems = CharacterCatalog.LoadSelectionItems(configRoot);
            _availableCharacters.AddRange(catalogItems);

            if (_availableCharacters.Count > 0)
            {
                _maxHp = Math.Max(1, _availableCharacters.Max(c => c.Hp));
                _maxAtk = Math.Max(1, _availableCharacters.Max(c => c.Atk));
                _maxDef = Math.Max(1, _availableCharacters.Max(c => c.Def));
                _maxSpd = Math.Max(1, _availableCharacters.Max(c => c.Speed));
            }

            if (_availableCharacters.Count == 0)
            {
                MessageBox.Show(
                    "No character configuration found.",
                    "Character Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ─── Bind danh sách nhân vật vào FlowLayoutPanel ─────────────────────

        private void BindCharacterSlots()
        {
            _panelCharacterMap.Clear();
            _slotPanels.Clear();

            flpnlSelChar.Controls.Clear();

            int count = _availableCharacters.Count;
            if (count == 0) return;

            int slotW = GetSlotWidth();

            for (int i = 0; i < count; i++)
            {
                CharacterSelectionItem character = _availableCharacters[i];
                string role = GetRoleLabel(character.Id);

                var slot = CreateCharacterSlot(
                    character,
                    role,
                    slotW,
                    SlotHeight
                );

                _panelCharacterMap[slot.Panel] = character;
                _slotPanels.Add(slot.Panel);

                AttachClickRecursive(slot.Panel);
                AttachHoverRecursive(slot.Panel);
            }

            if (_slotPanels.Count > 0)
                SelectByPanel(_slotPanels[0]);
        }

        private static string GetRoleLabel(string characterId)
        {
            return characterId.ToLower() switch
            {
                "wizard" => "🔮 Mage",
                "samurai" => "⚔️ Damage Dealer",
                "lord" => "🛡️ Tank",
                "kitsune" => "⚡ Speedster",
                "haladin" => "⚔️ Damage Dealer",
                "heavycrystal" => "🛡️ Tank",
                _ => "🗡️ Fighter",
            };
        }

        private CharacterSlot CreateCharacterSlot(
            CharacterSelectionItem character,
            string role,
            int slotW,
            int slotH)
        {
            var panel = new Panel
            {
                BackColor = SlotNormalColor,
                Size = new Size(slotW, slotH),
                TabIndex = 0,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, SlotSpacing)
            };

            // Ảnh nhân vật
            int picPad = 8;
            int picSize = slotH - picPad * 2;

            var picture = new PictureBox
            {
                BackColor = Color.Transparent,
                Location = new Point(picPad, picPad),
                Size = new Size(picSize, picSize),
                SizeMode = PictureBoxSizeMode.Zoom,
                TabStop = false,
                Cursor = Cursors.Hand
            };

            string imgPath = GetPortraitPath(character.Id);
            picture.Image = LoadImage(imgPath)
                         ?? LoadImage(character.GetPreviewPath(CharactersRoot));

            // Vùng text bên phải ảnh
            int textX = picture.Right + 10;
            int textAreaWidth = slotW - textX - 10;

            int nameH = 34;
            int roleH = 24;
            int statsH = slotH - nameH - roleH - 4;

            var lblName = new Label
            {
                Text = character.DisplayName,
                Font = new Font("Book Antiqua", 15F, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Color.FromArgb(255, 235, 156),
                AutoSize = false,
                Size = new Size(textAreaWidth, nameH),
                Location = new Point(textX, 4),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };

            var lblRole = new Label
            {
                Text = role,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(160, 200, 240),
                AutoSize = false,
                Size = new Size(textAreaWidth, roleH),
                Location = new Point(textX, lblName.Bottom),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };

            var lblStats = new Label
            {
                Text = $"HP:{character.Hp}  |  DMG:{character.Atk}  |  SPD:{character.Speed}",
                Font = new Font("Consolas", 8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(120, 160, 200),
                AutoSize = false,
                Size = new Size(textAreaWidth, statsH),
                Location = new Point(textX, lblRole.Bottom),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };

            panel.Controls.Add(picture);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblRole);
            panel.Controls.Add(lblStats);

            // Quan trọng: add vào FlowLayoutPanel
            flpnlSelChar.Controls.Add(panel);

            return new CharacterSlot(panel, lblName, picture);
        }

        private struct CharacterSlot
        {
            public Panel Panel { get; }
            public Label Label { get; }
            public PictureBox Picture { get; }

            public CharacterSlot(Panel panel, Label label, PictureBox picture)
            {
                Panel = panel;
                Label = label;
                Picture = picture;
            }
        }

        // ─── Click & Selection ────────────────────────────────────────────────

        private void AttachClickRecursive(Control control)
        {
            control.Click += CharacterSlot_Click;

            foreach (Control child in control.Controls)
                AttachClickRecursive(child);
        }

        private void CharacterSlot_Click(object? sender, EventArgs e)
        {
            if (sender is not Control control) return;

            var panel = FindMappedPanel(control);

            if (panel != null)
                SelectByPanel(panel);
        }

        private Panel? FindMappedPanel(Control control)
        {
            Control? current = control;

            while (current != null)
            {
                if (current is Panel p && _panelCharacterMap.ContainsKey(p))
                    return p;

                current = current.Parent;
            }

            return null;
        }

        private void SelectByPanel(Panel panel)
        {
            if (!_panelCharacterMap.TryGetValue(panel, out CharacterSelectionItem? character))
                return;

            _selectedCharacter = character;

            UpdateDisplay(character);
            HighlightSelected(panel);
        }

        // ─── Hover Effect ─────────────────────────────────────────────────────

        private void AttachHoverRecursive(Control control)
        {
            control.MouseEnter += SlotControl_MouseEnter;
            control.MouseLeave += SlotControl_MouseLeave;

            foreach (Control child in control.Controls)
                AttachHoverRecursive(child);
        }

        private void SlotControl_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is not Control control) return;

            var panel = FindMappedPanel(control);
            if (panel == null) return;

            if (panel.BackColor != SlotSelectedColor)
                panel.BackColor = SlotHoverColor;
        }

        private void SlotControl_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is not Control control) return;

            var panel = FindMappedPanel(control);
            if (panel == null) return;

            Point cursorPos = panel.PointToClient(Cursor.Position);

            if (!panel.ClientRectangle.Contains(cursorPos))
            {
                if (panel.BackColor != SlotSelectedColor)
                    panel.BackColor = SlotNormalColor;
            }
        }

        private void HighlightSelected(Panel selected)
        {
            foreach (var panel in _slotPanels)
                panel.BackColor = SlotNormalColor;

            selected.BackColor = SlotSelectedColor;
        }

        // ─── Info Panel bên phải ─────────────────────────────────────────────

        private void UpdateDisplay(CharacterSelectionItem character)
        {
            if (character == null) return;

            pbInfor.Image = LoadImage(GetPortraitPath(character.Id))
                         ?? LoadImage(character.GetPreviewPath(CharactersRoot));

            label2.Text = character.DisplayName;

            lblHP.Text = "❤️ HP";
            lblATK.Text = "⚔️ ATK";
            lblDEF.Text = "🛡️ DEF";
            lblSPD.Text = "⚡ SPD";
            lblSkill.Text = $"✨ SKILL  : {character.SkillLabel}";

            SetStatBar(panelHpBack, panelHpFill, character.Hp, _maxHp, lblHpValue);
            SetStatBar(panelAtkBack, panelAtkFill, character.Atk, _maxAtk, lblAtkValue);
            SetStatBar(panelDefBack, panelDefFill, character.Def, _maxDef, lblDefValue);
            SetStatBar(panelSpdBack, panelSpdFill, character.Speed, _maxSpd, lblSpdValue);
        }

        private void SetStatBar(
            Panel backPanel,
            Panel fillPanel,
            int value,
            int maxValue,
            Label valueLabel)
        {
            fillPanel.Width = GetBarWidth(backPanel, value, maxValue);

            valueLabel.Text = value.ToString();
            valueLabel.Font = new Font("Consolas", 9F, FontStyle.Bold);
            valueLabel.ForeColor = Color.FromArgb(200, 230, 255);
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;
        }

        private static int GetBarWidth(Panel backPanel, int value, int maxValue)
        {
            if (maxValue <= 0 || backPanel.Width <= 0)
                return 0;

            int clampedValue = Math.Clamp(value, 0, maxValue);

            int width = (int)Math.Round(
                backPanel.Width * (clampedValue / (double)maxValue)
            );

            return Math.Clamp(width, 0, backPanel.Width);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private string GetPortraitPath(string characterId)
        {
            return Path.Combine(PortraitRoot, $"{characterId.ToLower()}.png");
        }

        private Image? LoadImage(string path)
        {
            try
            {
                if (File.Exists(path))
                    return Image.FromFile(path);
            }
            catch
            {
            }

            return null;
        }

        // ─── Button Events ────────────────────────────────────────────────────

        private void btnSellect_Click(object sender, EventArgs e)
        {
            if (_selectedCharacter == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn nhân vật!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            SelectedCharacterId = _selectedCharacter.Id;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ─── Paint Events cũ giữ lại nếu Designer đang gắn event ─────────────

        private void CharacterPanel_Click(object sender, EventArgs e)
        {
            if (sender is Panel panel)
                SelectByPanel(panel);
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
        }

        private void pnlKitsune_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel4_Paint_1(object sender, PaintEventArgs e)
        {
            if (sender is not Panel panel) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            Rectangle rect = panel.ClientRectangle;

            using (var brush = new SolidBrush(Color.FromArgb(71, 129, 179)))
                g.FillRectangle(brush, rect);

            using (var penOuter = new Pen(Color.Black, 4))
                g.DrawRectangle(penOuter, 0, 0, panel.Width - 4, panel.Height - 4);

            using (var penInner = new Pen(Color.FromArgb(100, 255, 255, 255), 2))
                g.DrawRectangle(penInner, 4, 4, panel.Width - 12, panel.Height - 12);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}