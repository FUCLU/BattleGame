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
        private static readonly Color SlotNormalColor = Color.FromArgb(44, 74, 110); // Bình thường
        private static readonly Color SlotHoverColor = Color.FromArgb(55, 95, 140); // Hover (sáng hơn)
        private static readonly Color SlotSelectedColor = Color.FromArgb(63, 110, 165); // Đang chọn

        // Padding cách khung viền của panel2 (tránh lấp lên border vàng)
        private const int PanelPadding = 18;

        // SlotWidth tối đa — clamp theo panel2.Width thực tế
        private const int SlotWidth = 330;
        // SlotHeight & SlotSpacing được tính ĐỘNG trong BindCharacterSlots()
        private const int SlotHeightMax = 78;
        private const int SlotHeightMin = 50;
        private const int SlotSpacingMax = 8;

        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets");

        private static readonly string PortraitRoot = Path.Combine(AssetsRoot, "PotraitPic");
        private static readonly string CharactersRoot = Path.Combine(AssetsRoot, "Characters");

        // Dữ liệu 6 nhân vật (fallback nếu không load được từ config)
        private static readonly List<(string Id, string DisplayName, string Role, int Hp, int Dmg, int Spd)> DefaultCharacters = new()
        {
            ("wizard",       "Wizard",       "🔮 Mage",           100, 20, 200),
            ("samurai",      "Samurai",      "⚔️ Damage Dealer",  100, 20, 200),
            ("lord",         "Lord",         "🛡️ Tank",           130, 26, 190),
            ("kitsune",      "Kitsune",      "⚡ Speedster",       110, 18, 210),
            ("haladin",      "Haladin",      "⚔️ Damage Dealer",  120, 24, 240),
            ("heavycrystal", "HeavyCrystal", "🛡️ Tank",           150, 28, 190),
        };

        public CharacterSelection()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        public string SelectedCharacterId { get; private set; } = string.Empty;
        public string SelectedCharacterName => SelectedCharacterId;

        private void CharacterSelection_Load(object sender, EventArgs e)
        {
            LoadCharacters();
            BindCharacterSlots();
        }

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

        private void BindCharacterSlots()
        {
            _panelCharacterMap.Clear();
            _slotPanels.Clear();

            // Xóa các slot cũ trong panel chứa danh sách (panel2)
            var oldSlots = panel2.Controls.OfType<Panel>().ToList();
            foreach (var old in oldSlots)
                panel2.Controls.Remove(old);

            int count = _availableCharacters.Count;
            if (count == 0) return;

            // Vùng khả dụng sau khi trừ padding hai phía
            int availableWidth = panel2.Width - PanelPadding * 2;
            int availableHeight = panel2.Height - PanelPadding * 2;

            // Clamp SlotWidth không vượt quá vùng khả dụng
            int slotW = Math.Min(SlotWidth, availableWidth);

            // Tính SlotHeight và SlotSpacing ĐỘNG để tất cả slot luôn vừa khít
            int spacing = SlotSpacingMax;
            int slotH = (availableHeight - (count - 1) * spacing) / count;
            if (slotH < SlotHeightMin)
            {
                spacing = 0;
                slotH = availableHeight / count;
            }
            slotH = Math.Clamp(slotH, SlotHeightMin, SlotHeightMax);

            // Tổng chiều cao thực tế
            int totalHeight = count * slotH + (count - 1) * spacing;

            // Căn giữa theo chiều dọc trong vùng khả dụng
            int startY = PanelPadding + Math.Max(0, (availableHeight - totalHeight) / 2);
            int startX = PanelPadding + Math.Max(0, (availableWidth - slotW) / 2);

            for (int i = 0; i < count; i++)
            {
                CharacterSelectionItem character = _availableCharacters[i];
                string role = GetRoleLabel(character.Id);
                int y = startY + i * (slotH + spacing);
                var slot = CreateCharacterSlot(character, role, startX, y, slotW, slotH);

                _panelCharacterMap[slot.Panel] = character;
                _slotPanels.Add(slot.Panel);

                AttachClickRecursive(slot.Panel);
                AttachHoverRecursive(slot.Panel); // ← Thêm hover
            }

            if (_availableCharacters.Count > 0 && _slotPanels.Count > 0)
                SelectByPanel(_slotPanels[0]);
        }

        /// <summary>
        /// Trả về nhãn vai trò cho từng nhân vật.
        /// </summary>
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
            int x, int y, int slotW, int slotH)
        {
            // --- Panel ngoài (slot container) ---
            var panel = new Panel
            {
                BackColor = SlotNormalColor,
                Location = new Point(x, y),
                Size = new Size(slotW, slotH),
                TabIndex = 0,
                Cursor = Cursors.Hand,
            };

            // --- Ảnh nhân vật ---
            int picPad = 6;
            int picSize = slotH - picPad * 2;
            var picture = new PictureBox
            {
                BackColor = Color.Transparent,
                Location = new Point(picPad, picPad),
                Size = new Size(picSize, picSize),
                SizeMode = PictureBoxSizeMode.Zoom,
                TabStop = false,
            };

            string imgPath = GetPortraitPath(character.Id);
            picture.Image = LoadImage(imgPath) ?? LoadImage(character.GetPreviewPath(CharactersRoot));

            // --- Vùng text bên phải ảnh ---
            int textX = picture.Right + 8;
            int textAreaWidth = slotW - textX - 6;

            // ── Phân chia chiều cao: tên 45% | role 30% | stats 25% ──────────
            // Tăng roleH lên 30% (từ 26%) để chữ role không bị cắt
            int nameH = (int)(slotH * 0.45);
            int roleH = (int)(slotH * 0.30);   // ← tăng từ 0.26 lên 0.30
            int statsH = slotH - nameH - roleH;

            // Font size tên co giãn theo slotH
            float nameFontSize = Math.Clamp(slotH * 0.24f, 11f, 18f);

            // Tên nhân vật
            var lblName = new Label
            {
                Text = character.DisplayName,
                Font = new Font("Book Antiqua", nameFontSize, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Color.FromArgb(255, 235, 156),
                AutoSize = false,
                Size = new Size(textAreaWidth, nameH),
                Location = new Point(textX, 0),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // Dòng vai trò — padding trên nhỏ để không bị che
            var lblRole = new Label
            {
                Text = role,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(160, 200, 240),
                AutoSize = false,
                Size = new Size(textAreaWidth, roleH),
                Location = new Point(textX, nameH),
                TextAlign = ContentAlignment.TopLeft, // ← TopLeft thay vì MiddleLeft
                Padding = new Padding(0, 2, 0, 0),  // ← đẩy chữ xuống 2px để tránh đè lên tên
            };

            // Dòng stats: HP | DMG | SPD
            var lblStats = new Label
            {
                Text = $"HP:{character.Hp}  |  DMG:{character.Atk}  |  SPD:{character.Speed}",
                Font = new Font("Consolas", 7.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(120, 160, 200),
                AutoSize = false,
                Size = new Size(textAreaWidth, statsH),
                Location = new Point(textX, nameH + roleH),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            panel.Controls.Add(picture);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblRole);
            panel.Controls.Add(lblStats);
            panel2.Controls.Add(panel);

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
            if (panel != null) SelectByPanel(panel);
        }

        private Panel? FindMappedPanel(Control control)
        {
            Control? current = control;
            while (current != null)
            {
                if (current is Panel p && _panelCharacterMap.ContainsKey(p)) return p;
                current = current.Parent;
            }
            return null;
        }

        private void SelectByPanel(Panel panel)
        {
            if (!_panelCharacterMap.TryGetValue(panel, out CharacterSelectionItem? character)) return;
            _selectedCharacter = character;
            UpdateDisplay(character);
            HighlightSelected(panel);
        }

        // ─── Hover Effect ─────────────────────────────────────────────────────

        /// <summary>
        /// Gắn sự kiện MouseEnter / MouseLeave cho control và toàn bộ control con bên trong slot.
        /// </summary>
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

            // Không đổi màu nếu panel đang được chọn
            if (panel.BackColor != SlotSelectedColor)
                panel.BackColor = SlotHoverColor;
        }

        private void SlotControl_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is not Control control) return;
            var panel = FindMappedPanel(control);
            if (panel == null) return;

            // Khi chuột rời đi, kiểm tra xem chuột có thực sự rời khỏi toàn bộ panel không
            // (tránh trigger khi chuột chuyển sang control con bên trong slot)
            Point cursorPos = panel.PointToClient(Cursor.Position);
            if (!panel.ClientRectangle.Contains(cursorPos))
            {
                if (panel.BackColor != SlotSelectedColor)
                    panel.BackColor = SlotNormalColor;
            }
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

            // Cập nhật thanh fill + chỉ số
            // Thứ tự tham số: (panelBack, panelFill, giá trị, max, labelChỉSố)
            SetStatBar(panelHpBack, panelHpFill, character.Hp, _maxHp, lblHpValue);
            SetStatBar(panelAtkBack, panelAtkFill, character.Atk, _maxAtk, lblAtkValue);
            SetStatBar(panelDefBack, panelDefFill, character.Def, _maxDef, lblDefValue);
            SetStatBar(panelSpdBack, panelSpdFill, character.Speed, _maxSpd, lblSpdValue);
        }

        /// <summary>
        /// Cập nhật độ rộng thanh fill và text chỉ số.
        /// Vị trí của valueLabel giữ nguyên theo Designer — KHÔNG tính lại Location.
        /// </summary>
        private void SetStatBar(
            Panel backPanel,
            Panel fillPanel,
            int value,
            int maxValue,
            Label valueLabel)
        {
            // Cập nhật thanh fill
            fillPanel.Width = GetBarWidth(backPanel, value, maxValue);

            // Cập nhật chỉ số text
            valueLabel.Text = value.ToString();

            // ══════════════════════════════════════════════════════════════════
            // CÀI ĐẶT FONT / SIZE / MÀU CHỮ CHO LABEL CHỈ SỐ THANH STATS
            // Thay đổi tại đây để tuỳ chỉnh giao diện:
            // ────────────────────────────────────────────────────────────────
            valueLabel.Font = new Font("Consolas", 9F, FontStyle.Bold);  // Font & cỡ chữ
            valueLabel.ForeColor = Color.FromArgb(200, 230, 255);              // Màu chữ (R, G, B)
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;                // Căn chữ
            // ══════════════════════════════════════════════════════════════════
        }

        private static int GetBarWidth(Panel backPanel, int value, int maxValue)
        {
            if (maxValue <= 0 || backPanel.Width <= 0) return 0;
            int clampedValue = Math.Clamp(value, 0, maxValue);
            int width = (int)Math.Round(backPanel.Width * (clampedValue / (double)maxValue));
            return Math.Clamp(width, 0, backPanel.Width);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private string GetPortraitPath(string characterId) =>
            Path.Combine(PortraitRoot, $"{characterId.ToLower()}.png");

        private Image? LoadImage(string path)
        {
            try { if (File.Exists(path)) return Image.FromFile(path); }
            catch { }
            return null;
        }

        private void HighlightSelected(Panel selected)
        {
            foreach (var panel in _slotPanels)
                panel.BackColor = SlotNormalColor;
            selected.BackColor = SlotSelectedColor;
        }

        // ─── Button Events ────────────────────────────────────────────────────

        private void btnSellect_Click(object sender, EventArgs e)
        {
            if (_selectedCharacter == null)
            {
                MessageBox.Show("Vui lòng chọn nhân vật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // ─── Paint Events ─────────────────────────────────────────────────────

        private void CharacterPanel_Click(object sender, EventArgs e)
        {
            if (sender is Panel panel) SelectByPanel(panel);
        }

        private void panel4_Paint(object sender, PaintEventArgs e) { }

        private void pnlKitsune_Paint(object sender, PaintEventArgs e) { }

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
    }
}