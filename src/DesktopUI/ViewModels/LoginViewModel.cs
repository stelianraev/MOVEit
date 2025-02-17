using DesktopUI.Commands;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace DesktopUI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cancellationTokenSource;

        public LoginViewModel()
        {
            _httpClient = new HttpClient();
            _cancellationTokenSource = new CancellationTokenSource();

            LoginCommand = new LoginCommand(this, _cancellationTokenSource.Token);
        }

        public ICommand LoginCommand { get; }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Password
        {
            private get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public async Task LoginAsync()
        {
            //TODO Call my API 
            var requestData = new { username = Username, password = Password };
            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7040/authenticate/token", content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show($"Welcome, {Username}!", "Login Successful", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Invalid login", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
