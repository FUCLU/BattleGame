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

        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets");

        private static readonly string PortraitRoot = Path.Combine(AssetsRoot, "PotraitPic");
        private static readonly string CharactersRoot = Path.Combine(AssetsRoot, "Characters");

        public CharacterSelection()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        public string SelectedCharacterId { get; private set; } = string.Empty;

        // Keep this property for compatibility with existing callers.
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

            // Load all characters from JSON config
            var catalogItems = CharacterCatalog.LoadSelectionItems(configRoot);
            _availableCharacters.AddRange(catalogItems);

            if (_availableCharacters.Count > 0)
            {
                _maxHp = Math.Max(1, _availableCharacters.Max(character => character.Hp));
                _maxAtk = Math.Max(1, _availableCharacters.Max(character => character.Atk));
                _maxDef = Math.Max(1, _availableCharacters.Max(character => character.Def));
                _maxSpd = Math.Max(1, _availableCharacters.Max(character => character.Speed));
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

            // Define hardcoded slots for 4 characters
            var slots = new[]
            {
                new CharacterSlot(pnlWizard, lblWizardName, pbWizard),
                new CharacterSlot(pnlSamurai, lblSamuraiName, pbSamurai),
                new CharacterSlot(pnlKitsune, lblKitsuneName, pbKitsune),
                new CharacterSlot(pnlLord, lblLordName, pbLord)
            };

            // Bind characters to slots
            for (int i = 0; i < slots.Length && i < _availableCharacters.Count; i++)
            {
                CharacterSelectionItem character = _availableCharacters[i];
                CharacterSlot slot = slots[i];

                // Update label and image - use portrait from PotraitPic
                slot.Label.Text = character.DisplayName;
                slot.Picture.Image = LoadImage(GetPortraitPath(character.Id));

                // Store mapping
                _panelCharacterMap[slot.Panel] = character;
                _slotPanels.Add(slot.Panel);

                // Attach click event
                AttachClickRecursive(slot.Panel);
            }

            // Select first character if available
            if (_availableCharacters.Count > 0 && _slotPanels.Count > 0)
            {
                SelectByPanel(_slotPanels[0]);
            }
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

        private void AttachClickRecursive(Control control)
        {
            control.Click += CharacterSlot_Click;

            foreach (Control child in control.Controls)
            {
                AttachClickRecursive(child);
            }
        }

        private void CharacterSlot_Click(object? sender, EventArgs e)
        {
            if (sender is not Control control)
                return;

            var panel = FindMappedPanel(control);
            if (panel != null)
                SelectByPanel(panel);
        }

        private Panel? FindMappedPanel(Control control)
        {
            Control? current = control;

            while (current != null)
            {
                if (current is Panel panel && _panelCharacterMap.ContainsKey(panel))
                    return panel;

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

        private void UpdateDisplay(CharacterSelectionItem character)
        {
            if (character == null)
                return;

            pbInfor.Image = LoadImage(GetPortraitPath(character.Id));

            label2.Text = character.DisplayName;
            lblHP.Text = "HP";
            lblATK.Text = "ATK";
            lblDEF.Text = "DEF";
            lblSPD.Text = "SPD";
            lblSkill.Text = $"SKILL  : {character.SkillLabel}";

            panelHpFill.Width = GetBarWidth(panelHpBack, character.Hp, _maxHp);
            panelAtkFill.Width = GetBarWidth(panelAtkBack, character.Atk, _maxAtk);
            panelDefFill.Width = GetBarWidth(panelDefBack, character.Def, _maxDef);
            panelSpdFill.Width = GetBarWidth(panelSpdBack, character.Speed, _maxSpd);
        }

        private static int GetBarWidth(Panel backPanel, int value, int maxValue)
        {
            if (maxValue <= 0 || backPanel.Width <= 0)
                return 0;

            int clampedValue = Math.Clamp(value, 0, maxValue);
            double ratio = clampedValue / (double)maxValue;
            int width = (int)Math.Round(backPanel.Width * ratio);
            return Math.Clamp(width, 0, backPanel.Width);
        }

        private string GetPortraitPath(string characterId)
        {
            // Map character IDs to portrait filenames
            string portraitFileName = characterId.ToLower() switch
            {
                "wizard" => "wizard.png",
                "samurai" => "samurai.png",
                "kitsune" => "kitsune.png",
                "lord" => "lord.png",
                _ => $"{characterId.ToLower()}.png"
            };

            return Path.Combine(PortraitRoot, portraitFileName);
        }

        private Image? LoadImage(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return Image.FromFile(path);
                }
            }
            catch
            {
            }

            return null;
        }

        private void btnSellect_Click(object sender, EventArgs e)
        {
            if (_selectedCharacter == null)
            {
                MessageBox.Show("Vui lòng chọn nhân vật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedCharacterId = _selectedCharacter.Id;
            
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CharacterPanel_Click(object sender, EventArgs e)
        {
            if (sender is Panel panel)
                SelectByPanel(panel);
        }

        private void HighlightSelected(Panel selected)
        {
            foreach (var panel in _slotPanels)
            {
                panel.BackColor = Color.FromArgb(44, 74, 110);
            }

            selected.BackColor = Color.FromArgb(63, 110, 165);
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlKitsune_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
