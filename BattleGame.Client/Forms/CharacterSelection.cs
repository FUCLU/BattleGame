using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BattleGame.Client.Config;

namespace BattleGame.Client.Forms
{
    public partial class CharacterSelection : Form
    {
        private readonly List<CharacterSelectionItem> _chars = new();
        private readonly Dictionary<Panel, int> _panelToIndex = new();
        private int _selectedIndex = -1;

        private static readonly string ConfigRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Config");

        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets", "Characters");

        public CharacterSelection()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        public string SelectedCharacterName { get; private set; } = string.Empty;

        private void CharacterSelection_Load(object sender, EventArgs e)
        {
            _chars.Clear();
            _chars.AddRange(CharacterCatalog.LoadSelectionItems(ConfigRoot));

            BuildPanelMapping();
            BindPanelTitles();

            if (_chars.Count > 0)
                SelectCharacter(0);
        }

        private void BuildPanelMapping()
        {
            _panelToIndex.Clear();

            Panel[] panels = { pnlGirlKnight, pnlKabold, pnlWarrior, pnlSoldier };
            for (int i = 0; i < panels.Length; i++)
            {
                if (i < _chars.Count)
                {
                    _panelToIndex[panels[i]] = i;
                    panels[i].Enabled = true;
                    panels[i].Visible = true;
                    SetHandCursor(panels[i]);
                }
                else
                {
                    panels[i].Enabled = false;
                    panels[i].Visible = false;
                }
            }
        }

        private void BindPanelTitles()
        {
            if (_chars.Count > 0) label1.Text = _chars[0].DisplayName;
            if (_chars.Count > 1) label8.Text = _chars[1].DisplayName;
            if (_chars.Count > 2) label3.Text = _chars[2].DisplayName;
            if (_chars.Count > 3) label4.Text = _chars[3].DisplayName;
        }

        private void SelectCharacter(int index)
        {
            if (index < 0 || index >= _chars.Count)
                return;

            _selectedIndex = index;
            CharacterSelectionItem item = _chars[index];

            UpdateDisplay(item);
            HighlightSelected(GetPanelByIndex(index));
        }

        private void UpdateDisplay(CharacterSelectionItem character)
        {
            label2.Text = character.DisplayName;
            lblHP.Text = $"HP   : {character.Hp}";
            lblATK.Text = $"ATK  : {character.Atk}";
            lblDEF.Text = $"DEF  : {character.Def}";
            lblSPD.Text = $"SPD  : {character.Speed}";
            lblSkill.Text = $"SKILL: {character.SkillLabel}";

            string previewPath = character.GetPreviewPath(AssetsRoot);
            var newImage = LoadImage(previewPath);

            var oldImage = pbInfor.Image;
            pbInfor.Image = newImage;
            oldImage?.Dispose();
        }

        private static Image? LoadImage(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return null;

                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return Image.FromStream(stream);
            }
            catch
            {
                return null;
            }
        }

        private void HighlightSelected(Panel? selectedPanel)
        {
            if (selectedPanel is null)
                return;

            Panel[] panels = { pnlGirlKnight, pnlKabold, pnlWarrior, pnlSoldier };
            foreach (var panel in panels)
                panel.BackColor = panel == selectedPanel ? Color.FromArgb(78, 125, 181) : Color.FromArgb(44, 74, 110);
        }

        private static void SetHandCursor(Control control)
        {
            control.Cursor = Cursors.Hand;
            foreach (Control child in control.Controls)
                SetHandCursor(child);
        }

        private Panel? GetPanelByIndex(int index)
        {
            foreach (var pair in _panelToIndex)
            {
                if (pair.Value == index)
                    return pair.Key;
            }

            return null;
        }

        private void SelectByPanel(Panel panel)
        {
            if (_panelToIndex.TryGetValue(panel, out int index))
                SelectCharacter(index);
        }

        private void btnSellect_Click(object sender, EventArgs e)
        {
            if (_selectedIndex < 0 || _selectedIndex >= _chars.Count)
                return;

            CharacterSelectionItem selected = _chars[_selectedIndex];
            SelectedCharacterName = selected.Id;

            MessageBox.Show($"Da chon: {selected.DisplayName}");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void pnlGirlKnight_Click(object sender, EventArgs e)
        {
            SelectByPanel(pnlGirlKnight);
        }

        private void pnlKabold_Click(object sender, EventArgs e)
        {
            SelectByPanel(pnlKabold);
        }

        private void pnlWarrior_Click(object sender, EventArgs e)
        {
            SelectByPanel(pnlWarrior);
        }

        private void pnlSoldier_Click(object sender, EventArgs e)
        {
            SelectByPanel(pnlSoldier);
        }
    }
}