using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;

namespace BattleGame.Client.Forms
{
    public partial class RoomForm : Form
    {
        private const int MaxPlayers = 2;
        private readonly string _roomCode;
        private readonly bool _isHost;
        private readonly int _localPlayerIndex;
        private readonly int _timeLimitMinutes;
        private int _playerCount;
        private bool _isReady;
        private bool _remoteReady;
        private string _selectedCharacterId = "lord";
        private string _selectedMapId = "terrace";
        private CancellationTokenSource? _listenCts;

        public RoomForm(string roomCode, bool isHost, int playerCount, string mapId, int timeLimitMinutes)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _roomCode = roomCode;
            _isHost = isHost;
            _playerCount = Math.Clamp(playerCount, 0, MaxPlayers);
            _localPlayerIndex = _playerCount <= 1 ? 1 : (isHost ? 1 : 2);
            _selectedMapId = string.IsNullOrWhiteSpace(mapId) ? "terrace" : mapId;
            _timeLimitMinutes = Math.Clamp(timeLimitMinutes, 1, 5);
            button1.Click += button1_Click;
            button5.Click += button1_Click;
            button2.Click += button2_Click;
            button4.Click += button4_Click;
            textBox3.Text = _roomCode;
            if (_isHost && _playerCount == 0)
            {
                _playerCount = 1;
            }
        }

        private void RoomForm_Load(object sender, EventArgs e)
        {
            AddMessage("", "Connecting to room...");
            ConfigureOnlineLayout();
            UpdateRoomStatus();
            UpdateReadyState();

            if (NetworkManager.Instance.IsConnected)
            {
                _listenCts = new CancellationTokenSource();
                _ = ListenForPacketsAsync(_listenCts.Token);

                if (_isHost && TryParseRoomId(out int roomId))
                {
                    _ = NetworkManager.Instance.SelectMapAsync(new SelectMapPacket
                    {
                        RoomId = roomId,
                        MapId = MapIdToNetwork(_selectedMapId)
                    });
                }
            }
        }

        private void ConfigureOnlineLayout()
        {
            button1.Visible = true;
            label5.Text = GetMapDisplayName(_selectedMapId);
            label6.Text = FormatTimeLimit(_timeLimitMinutes);
            button4.Visible = _isHost;
            button3.Enabled = _playerCount > 0;
            button4.Enabled = _isHost && _playerCount >= MaxPlayers && _isReady && _remoteReady;
        }

        private void UpdateRoomStatus()
        {
            textBox3.Text = _roomCode;

            bool hasPlayer1 = _playerCount >= 1;
            bool hasPlayer2 = _playerCount >= 2;

            UpdateReadyLabel(lblReady1, hasPlayer1, isReady: false);
            UpdateReadyLabel(lblReady2, hasPlayer2, isReady: false);

            if (!hasPlayer1 || !hasPlayer2)
            {
                AddMessage("", "Đang chờ người chơi...");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string codeToCopy = string.IsNullOrWhiteSpace(textBox3.Text)
                ? _roomCode
                : textBox3.Text;
            codeToCopy = codeToCopy.Trim();

            if (string.IsNullOrWhiteSpace(codeToCopy))
                return;

            try
            {
                Clipboard.SetText(codeToCopy);
            }
            catch (ExternalException)
            {
                Clipboard.SetDataObject(codeToCopy, true);
            }
            AddMessage("", "Đã copy room code.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            JoinRoom joinRoom = new JoinRoom();
            joinRoom.Show();
            Close();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            CharacterSelection selection = new CharacterSelection();
            if (selection.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(selection.SelectedCharacterId))
            {
                _selectedCharacterId = selection.SelectedCharacterId;
                _isReady = true;

                if (NetworkManager.Instance.IsConnected)
                {
                    await NetworkManager.Instance.SelectCharacterAsync(new SelectionCharacterPacket
                    {
                        CharacterId = CharacterIdToNetwork(_selectedCharacterId)
                    });
                }
            }
            else
            {
                _isReady = false;
            }

            UpdateReadyState();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_playerCount < MaxPlayers)
            {
                MessageBox.Show("Chưa đủ người chơi.", "Room", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_isReady || !_remoteReady)
            {
                MessageBox.Show("Chưa đủ người sẵn sàng.", "Room", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (NetworkManager.Instance.IsConnected)
            {
                _ = StartOnlineMatchAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(_selectedCharacterId))
            {
                _selectedCharacterId = "lord";
            }

            GameForm gameForm = new GameForm(_selectedCharacterId, _selectedMapId);
            gameForm.Show();
            Close();
        }

        private void UpdateReadyState()
        {
            bool hasPlayer1 = _playerCount >= 1;
            bool hasPlayer2 = _playerCount >= 2;
            bool canReady = _playerCount > 0;

            button3.Enabled = canReady;
            button4.Enabled = _isHost && _playerCount >= MaxPlayers && _isReady && _remoteReady;

            if (_localPlayerIndex == 1)
            {
                UpdateReadyLabel(lblReady1, hasPlayer1, _isReady);
                UpdateReadyLabel(lblReady2, hasPlayer2, isReady: false);
            }
            else
            {
                UpdateReadyLabel(lblReady1, hasPlayer1, isReady: false);
                UpdateReadyLabel(lblReady2, hasPlayer2, _isReady);
            }
        }

        private static void UpdateReadyLabel(Label label, bool hasPlayer, bool isReady)
        {
            label.Visible = true;

            if (!hasPlayer)
            {
                label.Text = "WAITING..";
                label.ForeColor = Color.White;
                return;
            }

            label.Text = isReady ? "READY" : "NOT READY";
            label.ForeColor = isReady ? Color.LimeGreen : Color.Red;
        }

        private static string GetMapDisplayName(string mapId)
        {
            return mapId switch
            {
                "terrace" => "Battle 1",
                "throneroom" => "Battle 2",
                "castle" => "Battle 3",
                _ => mapId
            };
        }

        private static string FormatTimeLimit(int minutes)
        {
            int safeMinutes = Math.Max(1, minutes);
            return $"{safeMinutes}:00";
        }


        void AddMessage(string sender, string message)
        {
            string time = DateTime.Now.ToString("HH:mm");

            string formatted;

            formatted = $"[{time}] {sender}: {message}\n";
            richtxtBoxMessage.AppendText(formatted);
            richtxtBoxMessage.ScrollToCaret();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtBoxInp.Text.Trim();

            if (string.IsNullOrEmpty(msg)) return;

            AddMessage("tester", msg); // TODO: thay bằng username thật

            txtBoxInp.Clear();
        }

        private void txtBoxInp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }

        private async Task ListenForPacketsAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Packet packet = await NetworkManager.Instance.ReceiveAsync();
                    if (token.IsCancellationRequested)
                        return;

                    BeginInvoke(new Action(() => HandlePacket(packet)));
                }
            }
            catch
            {
            }
        }

        private void HandlePacket(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.JoinRoomResult:
                    var joinResult = (JoinRoomResultPacket)packet;
                    if (joinResult.Success)
                    {
                        _playerCount = MaxPlayers;
                        UpdateRoomStatus();
                        UpdateReadyState();
                    }
                    break;
                case PacketType.Ready:
                    _remoteReady = true;
                    UpdateReadyState();
                    break;
                case PacketType.SelectMap:
                    var mapPacket = (SelectMapPacket)packet;
                    _selectedMapId = MapIdFromNetwork(mapPacket.MapId);
                    label5.Text = GetMapDisplayName(_selectedMapId);
                    break;
                case PacketType.MatchFound:
                    var matchFound = (MatchFoundPacket)packet;
                    OpenMatch(matchFound);
                    break;
            }
        }

        private async Task StartOnlineMatchAsync()
        {
            await NetworkManager.Instance.SendAsync(new MatchRequestPacket());
        }

        private void OpenMatch(MatchFoundPacket matchFound)
        {
            string mapId = MapIdFromNetwork(matchFound.MapId);
            string characterId = _isHost
                ? CharacterIdFromNetwork(matchFound.Player1CharacterId)
                : CharacterIdFromNetwork(matchFound.Player2CharacterId);

            GameForm gameForm = new GameForm(characterId, mapId);
            gameForm.Show();
            Close();
        }

        private bool TryParseRoomId(out int roomId)
        {
            return int.TryParse(_roomCode, out roomId);
        }

        private static int MapIdToNetwork(string mapId)
        {
            return mapId switch
            {
                "terrace" => 0,
                "throneroom" => 1,
                "castle" => 2,
                _ => 0
            };
        }

        private static string MapIdFromNetwork(int mapId)
        {
            return mapId switch
            {
                0 => "terrace",
                1 => "throneroom",
                2 => "castle",
                _ => "terrace"
            };
        }

        private static int CharacterIdToNetwork(string characterId)
        {
            return characterId.ToLowerInvariant() switch
            {
                "lord" => 0,
                "samurai" => 1,
                "kitsune" => 2,
                "wizard" => 3,
                _ => 0
            };
        }

        private static string CharacterIdFromNetwork(int characterId)
        {
            return characterId switch
            {
                0 => "lord",
                1 => "samurai",
                2 => "kitsune",
                3 => "wizard",
                _ => "lord"
            };
        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }
    }
}
