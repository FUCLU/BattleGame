using System;
using System.Windows.Forms;
using BattleGame.Client.Game.Dungeon;

namespace BattleGame.Client.Forms
{
    public partial class Stage1Form : Form
    {
        public Stage1Form()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Stage1Form_Click(object sender, EventArgs e)
        {
            var dungeonMap = DungeonMapRegistry.Get("map1");
            GameForm gameForm = new GameForm(dungeonMap.DefaultCharacterId, dungeonMap.MapId, returnFormOnExit: this);
            Hide();
            gameForm.Show();
        }
    }
}
