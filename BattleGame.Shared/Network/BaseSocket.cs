using System.Net.Sockets;
using System.Text;
using System.Threading;
using BattleGame.Shared.Security;

namespace BattleGame.Shared.Network
{
    public abstract class BaseSocket
    {
        protected TcpClient? _client;
        protected NetworkStream? _stream;
        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private readonly SemaphoreSlim _receiveLock = new(1, 1);

        public bool IsConnected() => _client != null && _client.Connected;

        // Gửi packet có length-prefix (bất đồng bộ)
        public async Task SendAsync(string json)
        {
            if (_stream == null)
                throw new InvalidOperationException("Stream=null");

            await _sendLock.WaitAsync();
            try
            {
                string encrypted = AesEncryption.Encrypt(json);
                byte[] data = Encoding.UTF8.GetBytes(encrypted);
                byte[] length = BitConverter.GetBytes(data.Length);
                await _stream.WriteAsync(length, 0, 4);
                await _stream.WriteAsync(data, 0, data.Length);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        // Nhận packet có length-prefix (bất đồng bộ)
        public Task<string> ReceiveAsync()
        {
            return ReceiveAsync(CancellationToken.None);
        }

        public async Task<string> ReceiveAsync(CancellationToken token)
        {
            if (_stream == null)
                throw new InvalidOperationException("Stream=null");

            await _receiveLock.WaitAsync(token);
            try
            {
                byte[] lenBuf = new byte[4];
                await ReadExactAsync(lenBuf, 4, token);
                int size = BitConverter.ToInt32(lenBuf, 0);

                Console.WriteLine($"[DEBUG] Receiving packet size: {size}");

                byte[] dataBuf = new byte[size];
                await ReadExactAsync(dataBuf, size, token);

                string encrypted = Encoding.UTF8.GetString(dataBuf);
                return AesEncryption.Decrypt(encrypted);
            }
            finally
            {
                _receiveLock.Release();
            }
        }

        // Đọc đúng số byte yêu cầu
        private Task ReadExactAsync(byte[] buffer, int count)
        {
            return ReadExactAsync(buffer, count, CancellationToken.None);
        }

        private async Task ReadExactAsync(byte[] buffer, int count, CancellationToken token)
        {
            if (_stream == null)
                throw new InvalidOperationException("Stream=null");

            int received = 0;
            while (received < count)
            {
                int n = await _stream.ReadAsync(buffer, received, count - received, token);
                if (n == 0)
                    throw new IOException("Mất kết nối");
                received += n;
            }
        }

        public void Close()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
            }
            catch
            {
            }
        }
    }
}
