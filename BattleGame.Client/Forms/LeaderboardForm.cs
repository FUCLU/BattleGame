using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;

namespace BattleGame.Client.Forms
{
    public partial class LeaderboardForm : Form
    {
        public LeaderboardForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private async void LeaderboardForm_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.Columns.Clear();
            listView1.Items.Clear();
            listView1.Columns.Add("Rank", 50);
            listView1.Columns.Add("Name", 200);
            listView1.Columns.Add("Win", 100);
            listView1.Columns.Add("Lost", 100);
            ResizeColumns();

            await LoadLeaderboardAsync();
        }

        private async Task LoadLeaderboardAsync()
        {
            if (!NetworkManager.Instance.IsConnected)
            {
                AddStatusRow("Not connected");
                return;
            }

            try
            {
                var result = await NetworkManager.Instance.GetLeaderboardAsync(new GetLeaderboardPacket());
                listView1.Items.Clear();

                if (result.Entries.Count == 0)
                {
                    AddStatusRow("No matches yet");
                    return;
                }

                foreach (var entry in result.Entries)
                {
                    var item = new ListViewItem(entry.Rank.ToString());
                    item.SubItems.Add(entry.Username ?? string.Empty);
                    item.SubItems.Add(entry.Wins.ToString());
                    item.SubItems.Add(entry.Losses.ToString());
                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                listView1.Items.Clear();
                AddStatusRow($"Load failed: {ex.Message}");
            }
        }

        private void AddStatusRow(string message)
        {
            var item = new ListViewItem("-");
            item.SubItems.Add(message);
            item.SubItems.Add("-");
            item.SubItems.Add("-");
            listView1.Items.Add(item);
        }



        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ResizeColumns()
        {
            int totalWidth = listView1.ClientSize.Width;

            listView1.Columns[0].Width = (int)(totalWidth * 0.2); // LEVEL
            listView1.Columns[1].Width = (int)(totalWidth * 0.4); // NAME
            listView1.Columns[2].Width = (int)(totalWidth * 0.2); // XP
            listView1.Columns[3].Width = (int)(totalWidth * 0.2);
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            ResizeColumns();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
