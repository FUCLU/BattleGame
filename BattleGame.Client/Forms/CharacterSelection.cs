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
        private sealed class CharacterSlot
        {
            public CharacterSlot(Panel panel, Label nameLabel, PictureBox previewPicture)
            {
                Panel = panel;
                NameLabel = nameLabel;
                PreviewPicture = previewPicture;
            }

            public Panel Panel { get; }
            public Label NameLabel { get; }
            public PictureBox PreviewPicture { get; }
        }

        private readonly Dictionary<Panel, CharacterSelectionItem> _panelCharacterMap = new();
        private readonly List<CharacterSelectionItem> _availableCharacters = new();
        private readonly List<Panel> _slotPanels = new();
        private CharacterSelectionItem? _selectedCharacter;

        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets", "Characters");

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
            string configCharactersDir = Path.Combine(configRoot, "Config", "Characters");

            var configuredIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (Directory.Exists(configCharactersDir))
            {
                foreach (string configPath in Directory.EnumerateFiles(configCharactersDir, "*.json"))
                {
                    configuredIds.Add(Path.GetFileNameWithoutExtension(configPath));
                }
            }

            var catalogItems = CharacterCatalog.LoadSelectionItems(configRoot);
            if (configuredIds.Count > 0)
            {
                _availableCharacters.AddRange(
                    catalogItems.Where(item => configuredIds.Contains(item.Id)));
            }
            else
            {
                _availableCharacters.AddRange(catalogItems);
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

            var slots = new[]
            {
                new CharacterSlot(pnlGirlKnight, label1, pbSelGirlKnight),
                new CharacterSlot(pnlKabold, label8, pbSelKabold),
                new CharacterSlot(pnlWarrior, label3, pictureBox1),
                new CharacterSlot(pnlSoldier, label4, pictureBox2)
            };

            for (int i = 0; i < slots.Length; i++)
            {
                CharacterSlot slot = slots[i];
                _slotPanels.Add(slot.Panel);
                AttachClickRecursive(slot.Panel);

                if (i >= _availableCharacters.Count)
                {
                    ConfigureEmptySlot(slot);
                    continue;
                }

                CharacterSelectionItem character = _availableCharacters[i];
                _panelCharacterMap[slot.Panel] = character;
                slot.NameLabel.Text = character.DisplayName;
                slot.PreviewPicture.Image = LoadImage(character.GetPreviewPath(AssetsRoot));
                slot.Panel.Enabled = true;
                slot.Panel.BackColor = Color.FromArgb(44, 74, 110);
                SetHandCursor(slot.Panel);
            }

            if (_availableCharacters.Count > 0)
            {
                var initialPanel = slots.FirstOrDefault(slot => _panelCharacterMap.ContainsKey(slot.Panel))?.Panel;
                if (initialPanel != null)
                    SelectByPanel(initialPanel);
            }
        }

        private static void ConfigureEmptySlot(CharacterSlot slot)
        {
            slot.NameLabel.Text = "Locked";
            slot.PreviewPicture.Image = null;
            slot.Panel.Enabled = false;
            slot.Panel.Cursor = Cursors.Default;
            slot.Panel.BackColor = Color.FromArgb(36, 58, 94);
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

            pbInfor.Image = LoadImage(character.GetPreviewPath(AssetsRoot));

            label2.Text = character.DisplayName;
            lblHP.Text = $"HP     : {character.Hp}";
            lblATK.Text = $"ATK    : {character.Atk}";
            lblDEF.Text = $"DEF    : {character.Def}";
            lblSPD.Text = $"SPEED  : {character.Speed}";
            lblSkill.Text = $"SKILL  : {character.SkillLabel}";
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
                return;

            SelectedCharacterId = _selectedCharacter.Id;
            DialogResult = DialogResult.OK;
            Close();
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

        private void SetHandCursor(Control ctrl)
        {
            ctrl.Cursor = Cursors.Hand;
            foreach (Control child in ctrl.Controls)
            {
                SetHandCursor(child);
            }
        }
    }
}
