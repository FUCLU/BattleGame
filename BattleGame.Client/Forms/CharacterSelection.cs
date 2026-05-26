using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private bool _isInitialized;

        private int _maxHp = 1;
        private int _maxAtk = 1;
        private int _maxDef = 1;
        private int _maxSpd = 1;
        private Panel? _selectedSlotPanel;

        // ─── Màu nền slot ────────────────────────────────────────────────────
        private Color SlotNormalColor => pnlCharacterSlotTemplate.BackColor == Color.Empty
            ? Color.FromArgb(44, 74, 110)
            : pnlCharacterSlotTemplate.BackColor;

        private Color SlotHoverColor => Color.FromArgb(57, 100, 150);
        private Color SlotSelectedColor => Color.FromArgb(32, 82, 140);

        // ─── Giao diện slot trong FlowLayoutPanel ────────────────────────────
        private const int PanelPadding = 8;
        private const int SlotHeight = 92;
        private const int SlotSpacing = 8;

        private string _assetsRoot = string.Empty;
        private string _portraitRoot = string.Empty;
        private string _charactersRoot = string.Empty;

        private bool IsInDesignMode()
        {
            string processName = Process.GetCurrentProcess().ProcessName;

            return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                || (Site?.DesignMode ?? false)
                || DesignMode
                || LicenseManager.UsageMode != LicenseUsageMode.Runtime
                || processName.Contains("devenv", StringComparison.OrdinalIgnoreCase)
                || processName.Contains("designtoolsserver", StringComparison.OrdinalIgnoreCase)
                || processName.Contains("xdesproc", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyFlatStatBarStyle()
        {
            panelHpBack.BorderStyle = BorderStyle.FixedSingle;
            panelAtkBack.BorderStyle = BorderStyle.FixedSingle;
            panelDefBack.BorderStyle = BorderStyle.FixedSingle;
            panelSpdBack.BorderStyle = BorderStyle.FixedSingle;

            panelHpFill.BackColor = Color.FromArgb(218, 73, 79);
            panelAtkFill.BackColor = Color.FromArgb(238, 179, 73);
            panelDefFill.BackColor = Color.FromArgb(102, 159, 229);
            panelSpdFill.BackColor = Color.FromArgb(106, 207, 124);

            panelHpBack.BackColor = Color.FromArgb(62, 47, 37);
            panelAtkBack.BackColor = Color.FromArgb(62, 47, 37);
            panelDefBack.BackColor = Color.FromArgb(62, 47, 37);
            panelSpdBack.BackColor = Color.FromArgb(62, 47, 37);

            panelHpBack.Paint -= PixelStatBack_Paint;
            panelAtkBack.Paint -= PixelStatBack_Paint;
            panelDefBack.Paint -= PixelStatBack_Paint;
            panelSpdBack.Paint -= PixelStatBack_Paint;
            panelHpFill.Paint -= PixelStatFill_Paint;
            panelAtkFill.Paint -= PixelStatFill_Paint;
            panelDefFill.Paint -= PixelStatFill_Paint;
            panelSpdFill.Paint -= PixelStatFill_Paint;
        }

        private void ApplySynchronizedLabelStyles()
        {
            SyncLabelStyle(lblHpIcon, lblAtkIcon, lblDefIcon, lblSpdIcon, lblSkillIcon);
            SyncLabelStyle(lblHP, lblATK, lblDEF, lblSPD, lblSkill);
            SyncLabelStyle(lblHpValue, lblAtkValue, lblDefValue, lblSpdValue);
        }

        private static void SyncLabelStyle(Label source, params Label[] targets)
        {
            foreach (var target in targets)
            {
                target.Font = source.Font;
                target.ForeColor = source.ForeColor;
                target.BackColor = source.BackColor;
                target.TextAlign = source.TextAlign;
            }
        }

        private void InitializeAssetPaths()
        {
            string configRoot = ResolveConfigRoot();

            _assetsRoot = Path.Combine(configRoot, "Assets");
            _portraitRoot = Path.Combine(_assetsRoot, "PotraitPic");
            _charactersRoot = Path.Combine(_assetsRoot, "Characters");
        }

        private static string ResolveConfigRoot([CallerFilePath] string sourceFilePath = "")
        {
            foreach (string startPath in GetSearchRoots(sourceFilePath))
            {
                if (TryFindConfigRoot(startPath, out var root))
                    return root;
            }

            foreach (string startPath in GetSearchRoots(sourceFilePath))
            {
                if (TryFindProjectRoot(startPath, out var root))
                    return root;
            }

            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..");
        }

        private static IEnumerable<string> GetSearchRoots(string sourceFilePath)
        {
            var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void AddRoot(string? path)
            {
                if (!string.IsNullOrWhiteSpace(path))
                    roots.Add(path);
            }

            AddRoot(AppDomain.CurrentDomain.BaseDirectory);
            AddRoot(AppContext.BaseDirectory);
            AddRoot(Directory.GetCurrentDirectory());
            AddRoot(Environment.CurrentDirectory);

            if (!string.IsNullOrWhiteSpace(sourceFilePath))
                AddRoot(Path.GetDirectoryName(sourceFilePath));

            string assemblyLocation = typeof(CharacterSelection).Assembly.Location;
            if (!string.IsNullOrWhiteSpace(assemblyLocation))
                AddRoot(Path.GetDirectoryName(assemblyLocation));

            return roots;
        }

        private static bool TryFindConfigRoot(string startPath, out string root)
        {
            var current = new DirectoryInfo(startPath);

            while (current != null)
            {
                if (TryResolveFromDirectory(current.FullName, out root))
                    return true;

                current = current.Parent;
            }

            root = string.Empty;
            return false;
        }

        private static bool TryFindProjectRoot(string startPath, out string root)
        {
            var current = new DirectoryInfo(startPath);

            while (current != null)
            {
                if (TryResolveProjectFromDirectory(current.FullName, out root))
                    return true;

                current = current.Parent;
            }

            root = string.Empty;
            return false;
        }

        private static bool TryResolveFromDirectory(string directoryPath, out string root)
        {
            string directConfigDir = Path.Combine(directoryPath, "Config", "Characters");
            if (Directory.Exists(directConfigDir))
            {
                root = directoryPath;
                return true;
            }

            string clientConfigDir = Path.Combine(directoryPath, "BattleGame.Client", "Config", "Characters");
            if (Directory.Exists(clientConfigDir))
            {
                root = Path.Combine(directoryPath, "BattleGame.Client");
                return true;
            }

            root = string.Empty;
            return false;
        }

        private static bool TryResolveProjectFromDirectory(string directoryPath, out string root)
        {
            string directProject = Path.Combine(directoryPath, "BattleGame.Client.csproj");
            if (File.Exists(directProject))
            {
                root = directoryPath;
                return true;
            }

            string nestedProject = Path.Combine(directoryPath, "BattleGame.Client", "BattleGame.Client.csproj");
            if (File.Exists(nestedProject))
            {
                root = Path.Combine(directoryPath, "BattleGame.Client");
                return true;
            }

            root = string.Empty;
            return false;
        }

        public CharacterSelection()
        {
            InitializeComponent();
            BorderlessFormHelper.Apply(this);
            StartPosition = FormStartPosition.CenterScreen;

            ApplyFlatStatBarStyle();
            ApplySynchronizedLabelStyles();

            // Khi FlowLayoutPanel đổi kích thước thì slot tự giãn theo
            flpnlSelChar.Resize += flpnlSelChar_Resize;
            btnInstruction.Click += btnInstruction_Click;

            if (IsInDesignMode())
                return;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (_isInitialized)
                return;

            if (IsInDesignMode())
                return;
        }

        public string SelectedCharacterId { get; private set; } = string.Empty;
        public string SelectedCharacterName => SelectedCharacterId;

        private void CharacterSelection_Load(object sender, EventArgs e)
        {
            if (IsInDesignMode())
                return;

            InitializeCharacterSelection(isDesignTime: false);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (_isInitialized)
                return;

            if (IsInDesignMode())
                return;
        }

        private void InitializeCharacterSelection(bool isDesignTime)
        {
            _isInitialized = true;

            SetupCharacterListFlowPanel();
            InitializeAssetPaths();
            LoadCharacters(isDesignTime);

            bool bindInline = isDesignTime || !Application.MessageLoop;
            if (bindInline)
            {
                BindCharacterSlots();
                ApplySlotWidthToAll();
                return;
            }

            // Đợi Form layout xong rồi mới bind slot
            BeginInvoke(new Action(() =>
            {
                BindCharacterSlots();
                ApplySlotWidthToAll();
            }));
        }

        private void BuildDesignTimePreview()
        {
            try
            {
                if (_slotPanels.Count > 0 && flpnlSelChar.Controls.OfType<Panel>().Any())
                    return;

                SetupCharacterListFlowPanel();
                InitializeAssetPaths();

                _panelCharacterMap.Clear();
                _slotPanels.Clear();
                _availableCharacters.Clear();

                LoadDesignTimeCharacters();
                BindCharacterSlots();
                ApplySlotWidthToAll();
            }
            catch
            {
            }
        }

        // ─── Setup FlowLayoutPanel ────────────────────────────────────────────

        private void SetupCharacterListFlowPanel()
        {
            flpnlSelChar.AutoScroll = true;
            flpnlSelChar.FlowDirection = FlowDirection.TopDown;
            flpnlSelChar.WrapContents = false;
            flpnlSelChar.Padding = new Padding(PanelPadding, PanelPadding, 2, PanelPadding);
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
                        - 2;

            return Math.Max(280, slotW);
        }



        private void ApplySlotWidthToAll()
        {
            int slotW = GetSlotWidth();
            int slotH = GetSlotHeight();

            foreach (var slot in _slotPanels)
            {
                slot.Width = slotW;
                slot.Height = slotH;
                slot.Margin = new Padding(0, 0, 0, SlotSpacing);

                ResizeSlotChildren(slot);
            }
        }

        private int GetSlotHeight()
        {
            return pnlCharacterSlotTemplate.Height > 0
                ? pnlCharacterSlotTemplate.Height
                : SlotHeight;
        }

        private void ResizeSlotChildren(Panel slot)
        {
            if (slot.Controls.Count == 0) return;

            PictureBox? picture = slot.Controls.OfType<PictureBox>().FirstOrDefault();
            if (picture == null) return;

            int textX = lblSlotNameTemplate.Left > 0
                ? lblSlotNameTemplate.Left
                : picture.Right + 10;
            int textAreaWidth = Math.Max(120, slot.Width - textX - 10);

            foreach (Control child in slot.Controls)
            {
                if (child is Label lbl && (child.Name == "lblSlotName" || child.Name == "lblSlotRole"))
                {
                    lbl.Left = textX;
                    lbl.Width = textAreaWidth;
                }
            }
        }

        // ─── Load dữ liệu nhân vật ────────────────────────────────────────────

        private void LoadCharacters(bool isDesignTime)
        {
            _availableCharacters.Clear();

            string configRoot = ResolveConfigRoot();

            var catalogItems = CharacterCatalog.LoadSelectionItems(configRoot);
            _availableCharacters.AddRange(catalogItems);

            if (_availableCharacters.Count > 0)
            {
                _maxHp = Math.Max(1, _availableCharacters.Max(c => c.Hp));
                _maxAtk = Math.Max(1, _availableCharacters.Max(c => c.Atk));
                _maxDef = Math.Max(1, _availableCharacters.Max(c => c.Def));
                _maxSpd = Math.Max(1, _availableCharacters.Max(c => c.Speed));
            }

            if (_availableCharacters.Count == 0 && !isDesignTime)
            {
                MessageBox.Show(
                    "No character configuration found.",
                    "Character Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            if (_availableCharacters.Count == 0 && isDesignTime)
                LoadDesignTimeCharacters();
        }

        private void LoadDesignTimeCharacters()
        {
            _availableCharacters.AddRange(new[]
            {
                new CharacterSelectionItem("stonegolem", "Golem", "Idle.png", 165, 22, 20, 165, "Flying Obelisk / Laser Beam", "StoneGolem"),
                new CharacterSelectionItem("haladin", "Haladin", "Idle.png", 135, 22, 14, 230, "Judgement Slash / Holy Wrath", "Haladin"),
                new CharacterSelectionItem("heavycrystal", "HeavyCrystal", "Idle.png", 200, 30, 20, 175, "Crystal Crush / Crystal Burst", "HeavyCrystal"),
                new CharacterSelectionItem("kitsune", "Kitsune", "Idle.png", 105, 16, 7, 245, "Barrier / Fire Burst", "Kitsune"),
                new CharacterSelectionItem("lord", "Lord", "Idle.png", 125, 22, 12, 235, "Fireball / Lightning", "Lord"),
                new CharacterSelectionItem("samurai", "Samurai", "Idle.png", 105, 18, 8, 300, "Multi Slash / Blade Wave", "Samurai"),
                new CharacterSelectionItem("wizard", "Wizard", "Idle.png", 95, 16, 7, 215, "Light Charge / Light Ball", "Wizard")
            });

            _maxHp = Math.Max(1, _availableCharacters.Max(c => c.Hp));
            _maxAtk = Math.Max(1, _availableCharacters.Max(c => c.Atk));
            _maxDef = Math.Max(1, _availableCharacters.Max(c => c.Def));
            _maxSpd = Math.Max(1, _availableCharacters.Max(c => c.Speed));
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
                    GetSlotHeight()
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
            panel.Paint += CharacterSlot_Paint;

            // Ảnh nhân vật
            var picture = new PictureBox
            {
                BackColor = pbSlotTemplate.BackColor,
                Location = pbSlotTemplate.Location,
                Size = pbSlotTemplate.Size,
                SizeMode = pbSlotTemplate.SizeMode,
                TabStop = false,
                Cursor = Cursors.Hand
            };

            string imgPath = GetPortraitPath(character);
            picture.Image = LoadImage(imgPath)
                         ?? LoadImage(character.GetPreviewPath(_charactersRoot));

            // Vùng text bên phải ảnh
            int textX = lblSlotNameTemplate.Left;
            int textAreaWidth = Math.Max(120, slotW - textX - 10);

            var lblName = new Label
            {
                Name = "lblSlotName",
                Text = character.DisplayName,
                Font = lblSlotNameTemplate.Font,
                ForeColor = lblSlotNameTemplate.ForeColor,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(textAreaWidth, lblSlotNameTemplate.Height),
                Location = new Point(textX, lblSlotNameTemplate.Top),
                TextAlign = lblSlotNameTemplate.TextAlign,
                Cursor = Cursors.Hand
            };

            var lblRole = new Label
            {
                Name = "lblSlotRole",
                Text = role,
                Font = lblSlotRoleTemplate.Font,
                ForeColor = lblSlotRoleTemplate.ForeColor,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(textAreaWidth, lblSlotRoleTemplate.Height),
                Location = new Point(textX, lblSlotRoleTemplate.Top),
                TextAlign = lblSlotRoleTemplate.TextAlign,
                Cursor = Cursors.Hand
            };

            var lblHp = CreateSlotStatLabel("lblSlotHp", lblSlotHpTemplate, $"HP:{character.Hp}");
            var lblSep1 = CreateSlotStatLabel("lblSlotSep1", lblSlotSep1Template, "|");
            var lblDmg = CreateSlotStatLabel("lblSlotDmg", lblSlotDmgTemplate, $"DMG:{character.Atk}");
            var lblSep2 = CreateSlotStatLabel("lblSlotSep2", lblSlotSep2Template, "|");
            var lblSpd = CreateSlotStatLabel("lblSlotSpd", lblSlotSpdTemplate, $"SPD:{character.Speed}");

            panel.Controls.Add(picture);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblRole);
            panel.Controls.Add(lblHp);
            panel.Controls.Add(lblSep1);
            panel.Controls.Add(lblDmg);
            panel.Controls.Add(lblSep2);
            panel.Controls.Add(lblSpd);

            // Quan trọng: add vào FlowLayoutPanel
            flpnlSelChar.Controls.Add(panel);

            return new CharacterSlot(panel, lblName, picture);
        }

        private static Label CreateSlotStatLabel(string name, Label template, string text)
        {
            return new Label
            {
                Name = name,
                Text = text,
                Font = template.Font,
                ForeColor = template.ForeColor,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = template.Size,
                Location = template.Location,
                TextAlign = template.TextAlign,
                Cursor = Cursors.Hand
            };
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
            {
                panel.BackColor = SlotNormalColor;
                panel.Invalidate();
            }

            _selectedSlotPanel = selected;
            selected.BackColor = SlotSelectedColor;
            selected.Invalidate();
        }

        private void CharacterSlot_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel)
                return;

            bool isSelected = ReferenceEquals(panel, _selectedSlotPanel);
            Color borderColor = isSelected
                ? Color.FromArgb(255, 235, 156)
                : Color.FromArgb(27, 48, 82);

            using var borderPen = new Pen(borderColor, isSelected ? 2 : 1);
            e.Graphics.DrawRectangle(borderPen, 0, 0, panel.Width - 1, panel.Height - 1);

            if (!isSelected)
                return;

            using var accentBrush = new SolidBrush(Color.FromArgb(255, 235, 156));
            e.Graphics.FillRectangle(accentBrush, 0, 0, 5, panel.Height);
        }

        // ─── Info Panel bên phải ─────────────────────────────────────────────

        private void UpdateDisplay(CharacterSelectionItem character)
        {
            if (character == null) return;

            pbInfor.Image = LoadImage(GetPortraitPath(character))
                         ?? LoadImage(character.GetPreviewPath(_charactersRoot));

            label2.Text = character.DisplayName;

            lblHpIcon.Text = "♥";
            lblAtkIcon.Text = "⚔";
            lblDefIcon.Text = "♦";
            lblSpdIcon.Text = "✦";
            lblSkillIcon.Text = "✥";

            lblHP.Text = "HP:";
            lblATK.Text = "ATK:";
            lblDEF.Text = "DEF:";
            lblSPD.Text = "SPD:";
            lblSkill.Text = $"SKILL : {character.SkillLabel}";

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
            fillPanel.Height = backPanel.ClientSize.Height;
            fillPanel.Invalidate();
            backPanel.Invalidate();

            valueLabel.Text = value.ToString();
            SyncLabelStyle(lblHpValue, valueLabel);
        }

        private static int GetBarWidth(Panel backPanel, int value, int maxValue)
        {
            if (maxValue <= 0 || backPanel.ClientSize.Width <= 0)
                return 0;

            int clampedValue = Math.Clamp(value, 0, maxValue);

            int width = (int)Math.Round(
                backPanel.ClientSize.Width * (clampedValue / (double)maxValue)
            );

            return Math.Clamp(width, 0, backPanel.ClientSize.Width);
        }

        private void PixelStatBack_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel backPanel)
                return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            Panel? fillPanel = backPanel.Controls.OfType<Panel>().FirstOrDefault();
            int fillWidth = fillPanel?.Width ?? 0;
            int emptyX = Math.Clamp(fillWidth, 0, backPanel.ClientSize.Width);

            Rectangle wholeRect = backPanel.ClientRectangle;
            Rectangle emptyRect = new Rectangle(
                emptyX,
                0,
                Math.Max(0, backPanel.ClientSize.Width - emptyX),
                backPanel.ClientSize.Height);

            using var baseBrush = new SolidBrush(Color.FromArgb(39, 26, 18));
            using var emptyBrush = new SolidBrush(Color.FromArgb(75, 52, 32));
            using var pixelBrush = new SolidBrush(Color.FromArgb(123, 88, 52));
            using var darkPixelBrush = new SolidBrush(Color.FromArgb(28, 20, 15));
            using var edgePen = new Pen(Color.FromArgb(24, 17, 13));
            using var lightPen = new Pen(Color.FromArgb(140, 104, 69));

            e.Graphics.FillRectangle(baseBrush, wholeRect);
            e.Graphics.FillRectangle(emptyBrush, emptyRect);

            DrawDitherPixels(e.Graphics, emptyRect, pixelBrush, darkPixelBrush);

            e.Graphics.DrawLine(lightPen, 0, 0, backPanel.ClientSize.Width - 1, 0);
            e.Graphics.DrawLine(edgePen, 0, backPanel.ClientSize.Height - 1, backPanel.ClientSize.Width - 1, backPanel.ClientSize.Height - 1);
            e.Graphics.DrawRectangle(edgePen, 0, 0, backPanel.ClientSize.Width - 1, backPanel.ClientSize.Height - 1);
        }

        private void PixelStatFill_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel fillPanel)
                return;

            Rectangle rect = fillPanel.ClientRectangle;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            Color baseColor = fillPanel.BackColor;
            Color highlightColor = ControlPaint.Light(baseColor, 0.45f);
            Color gridColor = ControlPaint.Dark(baseColor, 0.12f);
            Color shadeColor = ControlPaint.Dark(baseColor, 0.35f);
            Color edgeColor = ControlPaint.Dark(baseColor, 0.65f);

            using var fillBrush = new SolidBrush(baseColor);
            using var highlightBrush = new SolidBrush(highlightColor);
            using var gridPen = new Pen(gridColor);
            using var shadePen = new Pen(shadeColor);
            using var edgePen = new Pen(edgeColor);

            e.Graphics.FillRectangle(fillBrush, rect);

            for (int x = 4; x < rect.Right - 1; x += 4)
                e.Graphics.DrawLine(gridPen, x, rect.Top + 1, x, rect.Bottom - 2);

            for (int y = rect.Top + 4; y < rect.Bottom - 1; y += 4)
                e.Graphics.DrawLine(gridPen, rect.Left + 1, y, rect.Right - 2, y);

            for (int x = 1; x < rect.Right - 1; x += 4)
            {
                for (int y = rect.Top + 1; y < rect.Bottom - 1; y += 4)
                    e.Graphics.FillRectangle(highlightBrush, x, y, 2, 1);
            }

            e.Graphics.DrawLine(edgePen, rect.Left, rect.Top, rect.Right - 1, rect.Top);
            e.Graphics.DrawLine(shadePen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 1);
            e.Graphics.DrawLine(edgePen, rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
        }

        private static void DrawDitherPixels(Graphics graphics, Rectangle rect, Brush lightBrush, Brush darkBrush)
        {
            const int pixel = 2;
            const int step = 4;

            for (int y = rect.Top + 1; y < rect.Bottom - 1; y += step)
            {
                for (int x = rect.Left + 1; x < rect.Right - 1; x += step)
                {
                    Brush brush = ((x + y) / step) % 2 == 0 ? lightBrush : darkBrush;
                    int width = Math.Min(pixel, rect.Right - x - 1);
                    int height = Math.Min(pixel, rect.Bottom - y - 1);
                    if (width > 0 && height > 0)
                        graphics.FillRectangle(brush, x, y, width, height);
                }
            }
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private string GetPortraitPath(CharacterSelectionItem character)
        {
            foreach (string candidateName in GetPortraitCandidateNames(character))
            {
                string directPath = Path.Combine(_portraitRoot, candidateName);
                if (File.Exists(directPath))
                    return directPath;

                string? matchedPath = FindFileIgnoreCase(_portraitRoot, candidateName);
                if (!string.IsNullOrWhiteSpace(matchedPath))
                    return matchedPath;
            }

            return Path.Combine(_portraitRoot, $"{character.Id}.png");
        }

        private static IEnumerable<string> GetPortraitCandidateNames(CharacterSelectionItem character)
        {
            string previewFile = Path.GetFileName(character.PreviewImage);
            if (!string.IsNullOrWhiteSpace(previewFile) &&
                !previewFile.Equals("Idle.png", StringComparison.OrdinalIgnoreCase))
            {
                yield return previewFile;
            }

            if (!string.IsNullOrWhiteSpace(character.AssetFolder))
                yield return $"{character.AssetFolder}.png";

            if (!string.IsNullOrWhiteSpace(character.DisplayName))
                yield return $"{character.DisplayName}.png";

            yield return $"{character.Id}.png";
            yield return $"{character.Id.ToLowerInvariant()}.png";
        }

        private static string? FindFileIgnoreCase(string directoryPath, string fileName)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return null;

                return Directory.EnumerateFiles(directoryPath, fileName, SearchOption.TopDirectoryOnly)
                    .FirstOrDefault()
                    ?? Directory.EnumerateFiles(directoryPath, "*.png", SearchOption.TopDirectoryOnly)
                        .FirstOrDefault(path => string.Equals(Path.GetFileName(path), fileName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return null;
            }
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

        private void btnInstruction_Click(object? sender, EventArgs e)
        {
            using var instructionForm = new InstructionForm();
            instructionForm.ShowDialog(this);
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

        private void flpnlSelChar_Paint(object sender, PaintEventArgs e)
        {
        }

        private void lblHpValue_Click(object sender, EventArgs e)
        {

        }

        private void panelHpFill_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelHpFill_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}
