using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Client.Config;
using BattleGame.Client.Game.Dungeon;

namespace BattleGame.Client.Forms
{
    public partial class DungeonMode : Form
    {
        private readonly Form? _returnFormOnBack;
        private string _selectedCharacterId = string.Empty;

        public DungeonMode(Form? returnFormOnBack = null)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            _returnFormOnBack = returnFormOnBack;
            UpdateSelectedCharacterLabel();
        }

        private void btnStage1_Click(object sender, EventArgs e)
        {
            OpenDungeon(DungeonMapRegistry.Get("map1"));
        }

        private void btnStage2_Click(object sender, EventArgs e)
        {
            OpenDungeon(DungeonMapRegistry.Get("map2"));
        }

        private void OpenDungeon(DungeonMapDefinition dungeonMap)
        {
            if (string.IsNullOrWhiteSpace(_selectedCharacterId))
            {
                MessageBox.Show("Vui lòng chọn nhân vật trước khi vào stage.", "Dungeon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            GameForm gameForm = new GameForm(_selectedCharacterId, dungeonMap.MapId, returnFormOnExit: this);
            Hide();
            gameForm.Show();
        }

        private void btnSelectCharacter_Click(object sender, EventArgs e)
        {
            using CharacterSelection selection = new CharacterSelection();
            if (selection.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(selection.SelectedCharacterId))
                return;

            _selectedCharacterId = selection.SelectedCharacterId;
            UpdateSelectedCharacterLabel();
        }

        private void UpdateSelectedCharacterLabel()
        {
            if (string.IsNullOrWhiteSpace(_selectedCharacterId))
            {
                lblSelectedCharacter.Text = string.Empty;
                return;
            }

            string contentRoot = ClientContentRoot.Resolve(AppDomain.CurrentDomain.BaseDirectory);
            string displayName = CharacterCatalog
                .LoadSelectionItems(contentRoot)
                .FirstOrDefault(x => x.Id.Equals(_selectedCharacterId, StringComparison.OrdinalIgnoreCase))
                ?.DisplayName
                ?? _selectedCharacterId;

            lblSelectedCharacter.Text = $"{displayName}";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (_returnFormOnBack != null && !_returnFormOnBack.IsDisposed)
            {
                _returnFormOnBack.Show();
                Close();
                return;
            }

            OfflineMode offlineMode = new OfflineMode();
            Hide();
            offlineMode.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblSelectedCharacter_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
