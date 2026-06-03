using FactoryDashboard.Helpers;
using FactoryDashboard.Models;
using FactoryDashboard.Services;
using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace FactoryDashboard.ViewModels
{
    public partial class MainViewModel :ViewModelBase
    {
        private readonly NetworkService _networkService;
        private EquipmentData _equipment;
        private string _connectionStatus = "연결 끊김";

        public EquipmentData Equipment
        {
            get => _equipment;
            set { _equipment = value; OnPropertyChanged(); }
        }
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set { _connectionStatus = value; OnPropertyChanged(); }
        }

        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }

        public MainViewModel()
        {
            _networkService = new NetworkService();

            _networkService.OnStatusChanged += (status) => ConnectionStatus = status;
            _networkService.OnDataReceived += ProcessIncomingData;

            ConnectCommand = new RelayCommand(
                async (obj) => await _networkService.ConnectToServerAsync("127.0.0.1", 9000),
                (obj) => !_networkService.IsConnected);


            DisconnectCommand = new RelayCommand(
                (obj) => _networkService.Disconnect(), (obj) => _networkService.IsConnected);
        }

        private void ProcessIncomingData(string rawJson)
        {
            try
            {
                EquipmentData freshData = JsonSerializer.Deserialize<EquipmentData>(rawJson);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Equipment = freshData;
                });
            }
            catch (Exception)
            {
                // 파싱 오류 예외 처리
            }
        }
    }
}
