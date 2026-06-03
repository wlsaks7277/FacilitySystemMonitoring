using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;


namespace FactorySimulator
{
    class Program
    {
        private static TcpListener _server;
        private static StreamWriter _writer;
        private static Timer _dataTimer;
        private static int _currentProduction = 0;
        private static readonly Random _random = new Random();

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 스마트 팩토리 가상 설비 시뮬레이터 시작===");

            IPAddress ipAny = IPAddress.Any;
            int port = 9000;

            _server = new TcpListener(ipAny, port);
            _server.Start();

            Console.WriteLine($"TCP 서버가 열렸습니다. IP : {ipAny} PORT : {port}");
            Console.WriteLine("클라이언트의 연결 대기 중....");

            try
            {
                using (TcpClient client = await _server.AcceptTcpClientAsync())
                {
                    Console.WriteLine($"클라이언트 연결 성공 ! (접속처: {client.Client.RemoteEndPoint})");

                    using (NetworkStream stream = client.GetStream())
                    {
                        _writer = new StreamWriter(stream, Encoding.UTF8)
                        {
                            AutoFlush = true
                        };

                        _dataTimer = new Timer(500);
                        _dataTimer.Elapsed += OnDataTimerElapsed;
                        _dataTimer.Start();

                        Console.WriteLine("실시간 데이터 전송 중... (종료하려면 엔터를 누르세요)");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"통신 중 오류 발생 : {ex.Message}");
            }
            finally
            {
                _dataTimer?.Stop();
                _server?.Stop();
                Console.WriteLine("시뮬레이터가 종료되었습니다.");
            }
        }

        private static void OnDataTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var data = GenerateMockData();

                string jsonString = JsonSerializer.Serialize(data);

                _writer?.WriteLine(jsonString);

                Console.WriteLine($"[{ data.Timestamp:HH: mm: ss.fff}] 데이터 전송 완료->상태: { data.Status}, 온도: { data.Temperature:F1}°C");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"데이터 전송 실패 (클라이언트가 종료되었을 수 있음): {ex.Message}");
                _dataTimer.Stop();
            }
        }

        private static EquipmentData GenerateMockData()
        {
            _currentProduction += _random.Next(1, 3);

            double temp = 50.0 + (_random.NextDouble() * 20.0);
            double pressure = 4.0 + (_random.NextDouble() * 2.0);
            string status = "RUN";

            //에러 상황을 인위적으로 생성
            int errorDice = _random.Next(1, 101);
            if(errorDice > 95)
            {
                temp = 85.5 + (_random.NextDouble() * 10.0);//과열 상태
                status = "ERROR";
            }
            else if(errorDice >90)
            {
                pressure = 7.5 + (_random.NextDouble() * 1.5);//과압 상태
                status = "WARN";
            }

            return new EquipmentData
            {
                EquipmentId = "EQ-001",
                Temperature = Math.Round(temp, 1),
                Pressure = Math.Round(pressure, 1),
                ProductionCount = _currentProduction,
                Status = status,
                Timestamp = DateTime.Now
            };
        }
    }
}
