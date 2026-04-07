using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace BattleGame.LoadBalancer.Routing
{
    public class Redirect
    {
        public static async Task SendAsync(TcpClient client, ServerEndpoint endpoint)
        {
            try
            {
                using var stream = client.GetStream();
                string message = endpoint.ToPublicString();
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] length = BitConverter.GetBytes(data.Length);

                await stream.WriteAsync(length, 0, 4);
                await stream.WriteAsync(data, 0, data.Length);

                Console.WriteLine($"[LB] Redirect client → {endpoint}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LB] Redirect failed: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
