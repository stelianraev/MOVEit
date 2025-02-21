using MoveitWpf.Commands;
using MoveitWpf.Models;
using MoveitWpf.MoveitWpf;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MoveitWpf.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cancellationTokenSource;
        private TokenStorage.TokenData? _tokenData;
        private Visibility _loginFieldsVisibility;
        private Visibility _treeViewVisibility;

        public LoginViewModel()
        {
            _httpClient = new HttpClient();
            _cancellationTokenSource = new CancellationTokenSource();

            LoginCommand = new LoginCommand(this, _cancellationTokenSource.Token);

            _tokenData = TokenStorage.GetAccessToken();
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

        public Visibility LoginFieldsVisibility
        {
            get => _loginFieldsVisibility;
            set { _loginFieldsVisibility = value; OnPropertyChanged(nameof(LoginFieldsVisibility)); }
        }

        public Visibility TreeViewVisibility
        {
            get => _treeViewVisibility;
            set { _treeViewVisibility = value; OnPropertyChanged(nameof(TreeViewVisibility)); }
        }

        public async Task InitializeAsync()
        {
            await Task.Delay(500);
            if (_tokenData?.ExpiresDateTime >= DateTime.UtcNow)
            {
                LoginFieldsVisibility = Visibility.Collapsed;
                TreeViewVisibility = Visibility.Visible;
            }
            else
            {
                LoginFieldsVisibility = Visibility.Visible;
                TreeViewVisibility = Visibility.Collapsed;
            }
        }

        public async Task LoginAsync()
        {
            var requestData = new TokenRequest { Username = Username, Password = Password };
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7040/authenticate/token", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                    if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
                    {
                        var expiresDateTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                        TokenStorage.SaveAccessToken(tokenResponse.AccessToken, expiresDateTime);
                        //TODO Log success login
                        // TODO: Redirect or open a new window
                        LoginFieldsVisibility = Visibility.Collapsed;
                        TreeViewVisibility = Visibility.Visible;
                    }
                    else
                    {
                        //TODO Log failed login
                    }
                }
                catch (JsonException ex)
                {
                    //TODO ex lgging
                    MessageBox.Show("Invalid login", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid login", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private async Task RevokeToken(string token)
        //{
        //    //TODO doesnt work properly
        //    if (token != null)
        //    {
        //        var requestData = new RevokeTokenRequest { Token = token };
        //        var json = JsonConvert.SerializeObject(requestData);
        //        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7040/authenticate/revoke")
        //        {
        //            Content = content
        //        };

        //        var result = await _httpClient.SendAsync(requestMessage);
        //        var renewedToken = await result.Content.ReadAsStringAsync();

        //        if (result.IsSuccessStatusCode)
        //        {
        //            //TokenStorage.SaveAccessToken(renewedToken);
        //            //TODO Reddirect or open new widow
        //        }
        //        else
        //        {
        //            //TokenStorage.SaveAccessToken(renewedToken);
        //        }
        //    }
        //}
    }
}
