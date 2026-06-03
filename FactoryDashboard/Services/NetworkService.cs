

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FactoryDashboard.Services
{
    public class NetworkService
    {
        private TcpClient _client;
        private StreamReader _reader;

        public bool IsConnected
        {
            get
            {
                return _client != null && _client.Connected;
            }
        }

        public event Action<string> OnDataReceived;
        public event Action<string> OnStatusChanged;

        public async Task ConnectToServerAsync(string ip, int port)
        {
            try
            {
                _client = new TcpClient();
                OnStatusChanged?.Invoke("서버 연결 중 ....");

                System.Net.IPAddress ipAddress;

                System.Net.IPAddress.TryParse(ip, out ipAddress);

                await _client.ConnectAsync(ipAddress, port);
                OnStatusChanged?.Invoke("서버 연결 성공");

                NetworkStream stream = _client.GetStream();
                _reader = new StreamReader(stream, Encoding.UTF8);

                _ = Task.Run(() => ReceiveLoopAsync());
            }
            catch(Exception ex)
            {
                OnStatusChanged?.Invoke($"연결 실패 : {ex.Message}");
                Disconnect();
            }
        }

        public void Disconnect()
        {
            _reader.Close();
            _client?.Close();
            _client = null;
            OnStatusChanged?.Invoke("연결이 종료되었습니다.");
        }

        private async void ReceiveLoopAsync()
        {
            try
            {
                while(_client != null && _client.Connected)
                {
                    string rawData = await _reader.ReadLineAsync();
                    if (rawData == null) break;

                    OnDataReceived?.Invoke(rawData);
                }
            }
            catch(Exception ex)
            {
                OnStatusChanged?.Invoke("서버와의 연결이 유실되었습니다.");
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
