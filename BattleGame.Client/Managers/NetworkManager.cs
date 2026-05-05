using BattleGame.Client.Config;
using BattleGame.Client.Network;
using BattleGame.Shared.Packets;
using System.Collections.Generic;
using System.Threading;

namespace BattleGame.Client.Managers
{
    public class NetworkManager
    {
        private static NetworkManager? _instance;
        public static NetworkManager Instance => _instance ??= new NetworkManager();

        private readonly ClientSocket _socket;
        private readonly SemaphoreSlim _receiveGate = new(1, 1);
        private readonly List<Packet> _pendingPackets = new();

        private NetworkManager()
        {
            var cfg = new ClientConfig();
            _socket = new ClientSocket(cfg);
        }

        public async Task ConnectAsync()
        {
            await _socket.ConnectAsync();
        }

        public bool IsConnected => _socket.IsConnected();

        public async Task<bool> EnsureConnectedAsync()
        {
            if (IsConnected)
                return true;

            try
            {
                await ConnectAsync();
                return IsConnected;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendAsync(Packet packet)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Chưa kết nối tới server.");

            await _socket.SendPacketAsync(packet);
        }

        public async Task<Packet> ReceiveAsync()
        {
            await _receiveGate.WaitAsync();
            try
            {
                if (TryTakePendingAny(out Packet? pending))
                    return pending;

                return await _socket.ReceivePacketAsync();
            }
            finally
            {
                _receiveGate.Release();
            }
        }

        public async Task<Packet> ReceiveAsync(CancellationToken token)
        {
            await _receiveGate.WaitAsync(token);
            try
            {
                if (TryTakePendingAny(out Packet? pending))
                    return pending;

                return await _socket.ReceivePacketAsync(token);
            }
            finally
            {
                _receiveGate.Release();
            }
        }

        public async Task<Packet?> TryReceiveAsync(int timeoutMs, CancellationToken token, Func<Packet, bool>? acceptPacket = null)
        {
            DateTime deadlineUtc = DateTime.UtcNow.AddMilliseconds(Math.Max(1, timeoutMs));

            while (DateTime.UtcNow < deadlineUtc)
            {
                token.ThrowIfCancellationRequested();
                TimeSpan remaining = deadlineUtc - DateTime.UtcNow;
                TimeSpan waitSlice = remaining > TimeSpan.FromMilliseconds(50)
                    ? TimeSpan.FromMilliseconds(50)
                    : remaining;

                if (waitSlice <= TimeSpan.Zero)
                    break;

                bool entered = await _receiveGate.WaitAsync(waitSlice, token);
                if (!entered)
                    continue;

                try
                {
                    if (acceptPacket == null)
                    {
                        if (TryTakePendingAny(out Packet? pendingAny))
                            return pendingAny;
                    }
                    else
                    {
                        if (TryTakePendingByPredicate(acceptPacket, out Packet? pendingMatch))
                            return pendingMatch;
                    }

                    // Do not block when no data to avoid holding the receive gate for too long.
                    if (!_socket.HasDataAvailable())
                        return null;

                    // Read packet without timeout cancellation to avoid stream desync from partial reads.
                    Packet packet = await _socket.ReceivePacketAsync();
                    if (acceptPacket == null || acceptPacket(packet))
                        return packet;

                    _pendingPackets.Add(packet);
                }
                finally
                {
                    _receiveGate.Release();
                }
            }

            return null;
        }

        public async Task<LoginResultPacket> LoginAsync(LoginPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<LoginResultPacket>(PacketType.LoginResult, 8000);
        }

        public async Task<OtpPacket> RegisterAsync(RegisterPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<OtpPacket>(PacketType.OtpSent, 8000);
        }

        public async Task<OtpPacket> ForgotPasswordAsync(ForgotPasswordPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<OtpPacket>(PacketType.OtpSent, 8000);
        }

        public async Task<OtpPacket> VerifyOtpAsync(OtpVerifyPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<OtpPacket>(PacketType.OtpSent, 8000);
        }

        public async Task<OtpPacket> ResetPasswordAsync(ResetPasswordPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<OtpPacket>(PacketType.OtpSent, 8000);
        }

        public async Task<GetRoomResultPacket> GetRoomAsync(GetRoomPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<GetRoomResultPacket>(PacketType.GetRoomResult, 10000);
        }

        public async Task<JoinRoomResultPacket> JoinRoomAsync(JoinRoomPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<JoinRoomResultPacket>(PacketType.JoinRoomResult, 8000);
        }

        public async Task<CreateRoomResultPacket> CreateRoomAsync(CreateRoomPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<CreateRoomResultPacket>(PacketType.CreateRoomResult, 8000);
        }

        public async Task<GetLeaderboardResultPacket> GetLeaderboardAsync(GetLeaderboardPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<GetLeaderboardResultPacket>(PacketType.GetLeaderboardResult, 8000);
        }

        public async Task<RemoveRoomResultPacket> RemoveRoomAsync(RemoveRoomPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<RemoveRoomResultPacket>(PacketType.RemoveRoomResult, 8000);
        }

        public async Task LeaveRoomAsync(LeaveRoomPacket packet)
        {
            await SendAsync(packet);
        }

        public async Task SelectMapAsync(SelectMapPacket packet)
        {
            await SendAsync(packet);
        }

        public async Task SelectCharacterAsync(SelectionCharacterPacket packet)
        {
            await SendAsync(packet);
        }

        public async Task MoveAsync(MovePacket packet)
        {
            await SendAsync(packet);
        }

        public async Task AttackAsync(AttackPacket packet)
        {
            await SendAsync(packet);
        }

        public async Task DisconnectAsync(DisconnectPacket packet)
        {
            await SendAsync(packet);
        }

        public async Task<MatchFoundPacket> MatchRequestAsync(MatchRequestPacket packet)
        {
            await SendAsync(packet);
            return await ReceiveExpectedAsync<MatchFoundPacket>(PacketType.MatchFound);
        }

        private async Task<TPacket> ReceiveExpectedAsync<TPacket>(PacketType expectedType) where TPacket : Packet
        {
            await _receiveGate.WaitAsync();
            try
            {
                if (TryTakePendingByType(expectedType, out Packet? pendingMatch))
                    return (TPacket)pendingMatch;

                while (true)
                {
                    Packet packet = await _socket.ReceivePacketAsync();
                    if (packet.Type == expectedType)
                        return (TPacket)packet;

                    _pendingPackets.Add(packet);
                }
            }
            finally
            {
                _receiveGate.Release();
            }
        }

        private async Task<TPacket> ReceiveExpectedAsync<TPacket>(PacketType expectedType, int timeoutMs) where TPacket : Packet
        {
            using var cts = new CancellationTokenSource(timeoutMs);

            await _receiveGate.WaitAsync(cts.Token);
            try
            {
                if (TryTakePendingByType(expectedType, out Packet? pendingMatch))
                    return (TPacket)pendingMatch;

                while (true)
                {
                    Packet packet = await _socket.ReceivePacketAsync(cts.Token);
                    if (packet.Type == expectedType)
                        return (TPacket)packet;

                    _pendingPackets.Add(packet);
                }
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Timeout khi chờ packet {expectedType}.");
            }
            finally
            {
                _receiveGate.Release();
            }
        }

        private bool TryTakePendingAny(out Packet? packet)
        {
            if (_pendingPackets.Count > 0)
            {
                packet = _pendingPackets[0];
                _pendingPackets.RemoveAt(0);
                return true;
            }

            packet = null;
            return false;
        }

        private bool TryTakePendingByType(PacketType expectedType, out Packet? packet)
        {
            for (int i = 0; i < _pendingPackets.Count; i++)
            {
                if (_pendingPackets[i].Type != expectedType)
                    continue;

                packet = _pendingPackets[i];
                _pendingPackets.RemoveAt(i);
                return true;
            }

            packet = null;
            return false;
        }

        private bool TryTakePendingByPredicate(Func<Packet, bool> predicate, out Packet? packet)
        {
            for (int i = 0; i < _pendingPackets.Count; i++)
            {
                Packet candidate = _pendingPackets[i];
                if (!predicate(candidate))
                    continue;

                packet = candidate;
                _pendingPackets.RemoveAt(i);
                return true;
            }

            packet = null;
            return false;
        }
    }
}
