using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Client.Game.Rendering;
using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;

namespace BattleGame.Client.Forms
{
    public partial class RoomForm : Form
    {
        private const int MaxPlayers = 2;
        private const int PacketListenPollMs = 250;
        private const int ReadyLabelX = 193;
        private const int ReadyLabelWidth = 128;
        private readonly string _roomCode;
        private readonly bool _isHost;
        private int _timeLimitMinutes;
        private int _playerCount;
        private bool _player1Ready;
        private bool _player2Ready;
        private string _player1Name;
        private string _player2Name;
        private string _selectedCharacterId = "lord";
        private string _selectedMapId = "terrace";
        private CancellationTokenSource? _listenCts;
        private Task? _listenTask;
        private bool _leaveSent;

        public RoomForm(string roomCode, bool isHost, int playerCount, string mapId, int timeLimitMinutes, string? player1Name = null, string? player2Name = null)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _roomCode = roomCode;
            _isHost = isHost;
            _playerCount = Math.Clamp(playerCount, 0, MaxPlayers);
            _selectedMapId = string.IsNullOrWhiteSpace(mapId) ? "terrace" : mapId;
            _timeLimitMinutes = Math.Clamp(timeLimitMinutes, 1, 5);
            _player1Name = string.IsNullOrWhiteSpace(player1Name) ? "Player..." : player1Name.Trim();
            _player2Name = string.IsNullOrWhiteSpace(player2Name) ? "Player..." : player2Name.Trim();
            button1.Click += button1_Click;
            button5.Click += button1_Click;
            button2.Click += button2_Click;
            textBox3.Text = _roomCode;
            if (_isHost && _playerCount == 0)
            {
                _playerCount = 1;
            }

            string localUsername = string.IsNullOrWhiteSpace(PlayerSession.Username) ? "You" : PlayerSession.Username;
            if (_isHost)
            {
                if (string.IsNullOrWhiteSpace(player1Name))
                    _player1Name = localUsername;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(player2Name))
                    _player2Name = localUsername;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _listenCts?.Cancel();
            _listenCts?.Dispose();
            _listenCts = null;
            base.OnFormClosing(e);
        }

        protected override async void OnFormClosed(FormClosedEventArgs e)
        {
            if (TryParseRoomId(out int roomId))
            {
                JoinRoom.MarkOwnedRoomLeft(roomId);
            }

            if (!_leaveSent && NetworkManager.Instance.IsConnected && TryParseRoomId(out roomId))
            {
                try
                {
                    await NetworkManager.Instance.LeaveRoomAsync(new LeaveRoomPacket
                    {
                        RoomId = roomId
                    });
                }
                catch
                {
                }
            }

            base.OnFormClosed(e);
        }

        private async void RoomForm_Load(object sender, EventArgs e)
        {
            AddMessage("", "Connecting to room...");
            ConfigureOnlineLayout();
            UpdateRoomStatus();
            UpdateReadyState();
      

            if (NetworkManager.Instance.IsConnected)
            {
                _listenCts = new CancellationTokenSource();
                _listenTask = ListenForPacketsAsync(_listenCts.Token);

                await RefreshRoomSnapshotAsync();

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
            button3.Location = new Point(721, 12);
            button3.Enabled = _playerCount > 0;
        }

        private void UpdateRoomStatus()
        {
            textBox3.Text = _roomCode;
            bool localFirst = IsLocalPlayer1();
            string displayPlayer1 = localFirst ? _player1Name : _player2Name;
            string displayPlayer2 = localFirst ? _player2Name : _player1Name;
            textBox1.Text = displayPlayer1;
            textBox2.Text = displayPlayer2;

            bool hasPlayer1 = _playerCount >= 1;
            bool hasPlayer2 = _playerCount >= 2;

            bool topReady = localFirst ? _player1Ready : _player2Ready;
            bool bottomReady = localFirst ? _player2Ready : _player1Ready;

            UpdateReadyLabel(lblReady1, hasPlayer1, isReady: topReady);
            UpdateReadyLabel(lblReady2, hasPlayer2, isReady: bottomReady);

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

        private async void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;

            if (_listenCts != null)
            {
                _listenCts.Cancel();
                if (_listenTask != null)
                {
                    try
                    {
                        await _listenTask;
                    }
                    catch
                    {
                    }
                }
            }

            bool hasRoomId = TryParseRoomId(out int roomId);
            if (hasRoomId)
            {
                JoinRoom.MarkOwnedRoomLeft(roomId);
            }

            if (NetworkManager.Instance.IsConnected && hasRoomId)
            {
                try
                {
                    await NetworkManager.Instance.LeaveRoomAsync(new LeaveRoomPacket
                    {
                        RoomId = roomId
                    });
                    _leaveSent = true;
                }
                catch
                {
                }
            }

            JoinRoom joinRoom = new JoinRoom();
            joinRoom.Show();
            if (NetworkManager.Instance.IsConnected)
            {
                try
                {
                    await joinRoom.RefreshRoomsFromServerAsync();
                }
                catch
                {
                }
            }
            Close();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            CharacterSelection selection = new CharacterSelection();
            if (selection.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(selection.SelectedCharacterId))
            {
                _selectedCharacterId = selection.SelectedCharacterId;
                WarmCharacterAssets(_selectedCharacterId);

                if (NetworkManager.Instance.IsConnected)
                {
                    int roomId = 0;
                    TryParseRoomId(out roomId);
                    await NetworkManager.Instance.SelectCharacterAsync(new SelectionCharacterPacket
                    {
                        RoomId = roomId,
                        CharacterId = CharacterIdToNetwork(_selectedCharacterId)
                    });
                }

                if (IsLocalPlayer1())
                    _player1Ready = true;
                else
                    _player2Ready = true;
            }

            UpdateReadyState();
        }

        private void UpdateReadyState()
        {
            bool hasPlayer1 = _playerCount >= 1;
            bool hasPlayer2 = _playerCount >= 2;
            bool canReady = _playerCount > 0;

            button3.Enabled = canReady;

            bool localFirst = IsLocalPlayer1();
            bool displayHasPlayer1 = localFirst ? hasPlayer1 : hasPlayer2;
            bool displayHasPlayer2 = localFirst ? hasPlayer2 : hasPlayer1;
            bool displayReady1 = localFirst ? _player1Ready : _player2Ready;
            bool displayReady2 = localFirst ? _player2Ready : _player1Ready;

            UpdateReadyLabel(lblReady1, displayHasPlayer1, displayReady1);
            UpdateReadyLabel(lblReady2, displayHasPlayer2, displayReady2);
        }

        private static void UpdateReadyLabel(Label label, bool hasPlayer, bool isReady)
        {
            label.Visible = true;
            label.AutoSize = false;
            label.Size = new Size(ReadyLabelWidth, 25);
            label.Location = new Point(ReadyLabelX, label.Name == "lblReady1" ? 29 : 82);
            label.TextAlign = ContentAlignment.MiddleRight;
            label.BackColor = Color.FromArgb(34, 124, 162);
            label.BringToFront();

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
            string displaySender = string.IsNullOrWhiteSpace(sender) ? "SYSTEM" : sender.Trim();
            string formatted = $"[{time}] {displaySender}: {message}\n";
            richtxtBoxMessage.AppendText(formatted);
            richtxtBoxMessage.ScrollToCaret();
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtBoxInp.Text.Trim();
            if (string.IsNullOrEmpty(msg))
                return;

            txtBoxInp.Clear();
            if (!NetworkManager.Instance.IsConnected || !TryParseRoomId(out int roomId))
            {
                AddMessage(string.IsNullOrWhiteSpace(PlayerSession.Username) ? "You" : PlayerSession.Username, msg);
                return;
            }

            btnSend.Enabled = false;
            try
            {
                await NetworkManager.Instance.SendChatAsync(new ChatMessagePacket
                {
                    RoomId = roomId,
                    Message = msg
                });
            }
            catch
            {
                AddMessage("SYSTEM", "Gửi tin nhắn thất bại.");
            }
            finally
            {
                btnSend.Enabled = true;
            }

            if (IsDisposed)
                return;
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
                    Packet? packet = await NetworkManager.Instance.TryReceiveAsync(
                        PacketListenPollMs,
                        token,
                        p => IsRoomRealtimePacket(p.Type));
                    if (packet == null)
                    {
                        await Task.Delay(25, token);
                        continue;
                    }

                    if (token.IsCancellationRequested)
                        return;

                    BeginInvoke(new Action(() => HandlePacket(packet)));
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (IOException)
            {
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
                        if (joinResult.MapId >= 0)
                        {
                            _selectedMapId = MapIdFromNetwork(joinResult.MapId);
                            label5.Text = GetMapDisplayName(_selectedMapId);
                        }
                        if (joinResult.TimeLimitMinutes > 0)
                        {
                            _timeLimitMinutes = Math.Clamp(joinResult.TimeLimitMinutes, 1, 5);
                            label6.Text = FormatTimeLimit(_timeLimitMinutes);
                        }

                        _player1Name = string.IsNullOrWhiteSpace(joinResult.Player1Name) ? _player1Name : joinResult.Player1Name;
                        _player2Name = string.IsNullOrWhiteSpace(joinResult.Player2Name) ? _player2Name : joinResult.Player2Name;
                        _playerCount = MaxPlayers;
                        UpdateRoomStatus();
                        UpdateReadyState();

                        if (!string.IsNullOrWhiteSpace(joinResult.Message))
                            AddMessage("SYSTEM", joinResult.Message);
                    }
                    break;
                case PacketType.Ready:
                    var readyPacket = (ReadyPacket)packet;
                    _player1Ready = readyPacket.Player1Ready;
                    _player2Ready = readyPacket.Player2Ready;
                    UpdateReadyState();
                    break;
                case PacketType.SelectMap:
                    var mapPacket = (SelectMapPacket)packet;
                    _selectedMapId = MapIdFromNetwork(mapPacket.MapId);
                    label5.Text = GetMapDisplayName(_selectedMapId);
                    break;
                case PacketType.MatchFound:
                    var matchFound = (MatchFoundPacket)packet;
                    _player1Name = string.IsNullOrWhiteSpace(matchFound.Player1Name) ? _player1Name : matchFound.Player1Name;
                    _player2Name = string.IsNullOrWhiteSpace(matchFound.Player2Name) ? _player2Name : matchFound.Player2Name;
                    if (matchFound.TimeLimitMinutes > 0)
                        _timeLimitMinutes = Math.Clamp(matchFound.TimeLimitMinutes, 1, 5);
                    OpenMatch(matchFound);
                    break;
                case PacketType.RoomClosed:
                    var closed = (RoomClosedPacket)packet;
                    HandleRoomClosed(closed.Message);
                    break;
                case PacketType.ChatMessage:
                    var chat = (ChatMessagePacket)packet;
                    AddMessage(chat.SenderName, chat.Message);
                    break;
            }
        }

        private async Task RefreshRoomSnapshotAsync()
        {
            if (!NetworkManager.Instance.IsConnected || !TryParseRoomId(out int roomId))
                return;

            try
            {
                var roomResult = await NetworkManager.Instance.GetRoomAsync(new GetRoomPacket());
                RoomInfo? room = roomResult.Rooms.FirstOrDefault(r => r.RoomId == roomId);
                if (room == null)
                    return;

                if (!string.IsNullOrWhiteSpace(room.Player1Name))
                    _player1Name = room.Player1Name;

                if (!string.IsNullOrWhiteSpace(room.Player2Name))
                    _player2Name = room.Player2Name;

                _player1Ready = room.Player1Ready;
                _player2Ready = room.Player2Ready;
                _playerCount = Math.Clamp(room.CurrentPlayers, 0, MaxPlayers);

                if (room.MapId >= 0)
                {
                    _selectedMapId = MapIdFromNetwork(room.MapId);
                    label5.Text = GetMapDisplayName(_selectedMapId);
                }
                if (room.TimeLimitMinutes > 0)
                {
                    _timeLimitMinutes = Math.Clamp(room.TimeLimitMinutes, 1, 5);
                    label6.Text = FormatTimeLimit(_timeLimitMinutes);
                }

                UpdateRoomStatus();
                UpdateReadyState();
            }
            catch
            {
            }
        }

        private static bool IsRoomRealtimePacket(PacketType type)
        {
            return type == PacketType.JoinRoomResult
                || type == PacketType.Ready
                || type == PacketType.SelectMap
                || type == PacketType.MatchFound
                || type == PacketType.RoomClosed
                || type == PacketType.ChatMessage;
        }

        private void HandleRoomClosed(string message)
        {
            _leaveSent = true;
            _listenCts?.Cancel();
            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message, "Room Closed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            var joinRoom = new JoinRoom();
            joinRoom.Show();
            if (NetworkManager.Instance.IsConnected)
            {
                _ = joinRoom.RefreshRoomsFromServerAsync();
            }
            Close();
        }

        private void OpenMatch(MatchFoundPacket matchFound)
        {
            string mapId = MapIdFromNetwork(matchFound.MapId);
            int preferredUserId = NetworkManager.Instance.PreferredUserId;
            int localPlayerId;
            if (preferredUserId > 0 && (matchFound.Player1Id == preferredUserId || matchFound.Player2Id == preferredUserId))
            {
                localPlayerId = preferredUserId;
            }
            else
            {
                string localUsername = (PlayerSession.Username ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(localUsername) &&
                    string.Equals(localUsername, matchFound.Player1Name, StringComparison.OrdinalIgnoreCase))
                {
                    localPlayerId = matchFound.Player1Id;
                }
                else if (!string.IsNullOrWhiteSpace(localUsername) &&
                    string.Equals(localUsername, matchFound.Player2Name, StringComparison.OrdinalIgnoreCase))
                {
                    localPlayerId = matchFound.Player2Id;
                }
                else
                {
                    localPlayerId = IsLocalPlayer1() ? matchFound.Player1Id : matchFound.Player2Id;
                }
            }

            bool localIsP1 = localPlayerId == matchFound.Player1Id;
            int localCharacterNetworkId = localIsP1
                ? matchFound.Player1CharacterId
                : matchFound.Player2CharacterId;
            string localCharacterId = CharacterIdFromNetwork(localCharacterNetworkId);
            string enemyCharacterId = localIsP1
                ? CharacterIdFromNetwork(matchFound.Player2CharacterId)
                : CharacterIdFromNetwork(matchFound.Player1CharacterId);
            string localName = localIsP1 ? _player1Name : _player2Name;
            string enemyName = localIsP1 ? _player2Name : _player1Name;

            _leaveSent = true;
            GameForm gameForm = new GameForm(localCharacterId, mapId, enemyCharacterId, isOnline: true, localPlayerId: localPlayerId, localUsername: localName, enemyUsername: enemyName, roomId: matchFound.RoomId);
            gameForm.Show();
            Close();
        }

        private bool IsLocalPlayer1()
        {
            string localUsername = (PlayerSession.Username ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(localUsername))
            {
                if (string.Equals(localUsername, _player1Name, StringComparison.OrdinalIgnoreCase))
                    return true;

                if (string.Equals(localUsername, _player2Name, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return _isHost;
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
                "haladin" => 4,
                "heavycrystal" => 5,
                "stonegolem" => 6,
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
                4 => "haladin",
                5 => "heavycrystal",
                6 => "stonegolem",
                _ => "lord"
            };
        }

        private static void WarmCharacterAssets(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                return;

            _ = Task.Run(() =>
            {
                try
                {
                    _ = new AnimationLoader("Assets").Load(characterId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[RoomForm] Failed to warm character assets for '{characterId}': {ex}");
                }
            });
        }

    }
}
