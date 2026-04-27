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

            // Load all characters from JSON config
            var catalogItems = CharacterCatalog.LoadSelectionItems(configRoot);
            _availableCharacters.AddRange(catalogItems);

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

                // Update label and image
                slot.Label.Text = character.DisplayName;
                slot.Picture.Image = LoadImage(character.GetPreviewPath(AssetsRoot));

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
