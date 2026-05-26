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
        private static readonly Color HeaderBackColor = Color.FromArgb(225, 12, 55, 116);
        private static readonly Color HeaderTextColor = Color.FromArgb(255, 232, 126);
        private static readonly Color RowBackColor = Color.FromArgb(210, 8, 42, 92);
        private static readonly Color AlternateRowBackColor = Color.FromArgb(220, 14, 56, 118);
        private static readonly Color SelectedRowBackColor = Color.FromArgb(245, 224, 118, 34);
        private static readonly Color RowTextColor = Color.FromArgb(232, 248, 255);
        private static readonly Color GridColor = Color.FromArgb(125, 116, 197, 255);
        private static readonly Color RankTextColor = Color.FromArgb(255, 232, 126);

        public LeaderboardForm()
        {
            InitializeComponent();
            NormalizeCloseButton();
            BorderlessFormHelper.Apply(this);
            this.StartPosition = FormStartPosition.CenterScreen;
            StyleLeaderboardTable();
        }

        private void NormalizeCloseButton()
        {
            button1.UseVisualStyleBackColor = false;
            button1.FlatStyle = FlatStyle.Flat;
            button1.BackColor = Color.FromArgb(168, 45, 45);
            button1.FlatAppearance.BorderSize = 2;
            button1.FlatAppearance.BorderColor = Color.White;
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 65, 65);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(120, 28, 28);
        }

        private async void LeaderboardForm_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.Columns.Clear();
            listView1.Items.Clear();
            listView1.Columns.Add("RANK", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("PLAYER", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("WINS", 100, HorizontalAlignment.Center);
            listView1.Columns.Add("LOSSES", 100, HorizontalAlignment.Center);
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
                    item.UseItemStyleForSubItems = false;
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

        private void StyleLeaderboardTable()
        {
            listView1.BorderStyle = BorderStyle.FixedSingle;
            listView1.OwnerDraw = false;
            listView1.FullRowSelect = true;
            listView1.HideSelection = false;
            listView1.GridLines = true;
            listView1.BackColor = Color.FromArgb(232, 248, 255);
            listView1.ForeColor = RowTextColor;
            listView1.Font = new Font("Courier New", 15F, FontStyle.Bold);
            listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.ForeColor = Color.FromArgb(12, 55, 116);
        }



        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ResizeColumns()
        {
            int totalWidth = listView1.ClientSize.Width;

            if (listView1.Columns.Count < 4)
                return;

            listView1.Columns[0].Width = (int)(totalWidth * 0.16);
            listView1.Columns[1].Width = (int)(totalWidth * 0.44);
            listView1.Columns[2].Width = (int)(totalWidth * 0.2);
            listView1.Columns[3].Width = Math.Max(1, totalWidth - listView1.Columns[0].Width - listView1.Columns[1].Width - listView1.Columns[2].Width - 2);
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            ResizeColumns();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JoinRoom joinRoom = new JoinRoom();
            joinRoom.Show();
            this.Close();
        }
    }
}
