using DesktopUI.Commands;
using MoveitWpf;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
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
        private readonly string _token;

        public LoginViewModel()
        {
            _httpClient = new HttpClient();
            _cancellationTokenSource = new CancellationTokenSource();

            LoginCommand = new LoginCommand(this, _cancellationTokenSource.Token);

            var _token = TokenStorage.GetToken();

            if (_token != null)
            {
                RevokeToken(_token).Wait();
            }

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
            var requestData = new TokenRequest(Username, Password);
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7040/authenticate/token", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                if (tokenResponse != null)
                {
                    TokenStorage.SaveToken(tokenResponse.Token);
                    //TODO Reddirect or open new widow
                }
            }
            else
            {
                MessageBox.Show("Invalid login", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RevokeToken(string token)
        {
            if (token != null)
            {
                var requestData = new Dictionary<string, string>
                {
                    { "token", token }
                };

                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://localhost:7040/authenticate/revoke", content);

                if (response.IsSuccessStatusCode)
                {
                    TokenStorage.RemoveToken();
                    //TODO Reddirect or open new widow
                }
            }
        }

        private record TokenRequest(string Username, string Password);
        private record TokenResponse(string Token, int ExpiresIn, string RefreshToken);
    }
}
