using MoveitDesktopUI.Models;
using MoveitDesktopUI.Services;
using MoveitWpf.MoveitDesktopUI;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace MoveitDesktopUI
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly LocalFilesObserver _localFilesObserver;
        private readonly RemoteFileObserver _remoteFileObserver;
        private readonly GetRemoteFiles _remoteFiles;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly string _baseFolder = @"C:\MOVEit";

        public MainWindow()
        {
            InitializeComponent();
            _cancellationTokenSource = new CancellationTokenSource();
            _httpClient = new HttpClient();
            _remoteFiles = new GetRemoteFiles(_httpClient, RemoteFileTree, _cancellationTokenSource.Token);
            _localFilesObserver = new LocalFilesObserver(LocalFileTree);
            _remoteFileObserver = new RemoteFileObserver(RemoteFileTree);

            //Preventing deadlock
            Loaded += async (s, e) => await CheckIsTokenValid();
        }

        private async Task CheckIsTokenValid()
        {
            var accessToken = TokenStorage.GetAccessToken();

            //TODO : I revoke token succesfully but is declined by endpoint return 401. I need to check this. At the moment I will work with Expires Time
            //var revokeTokenResponse = await RevokeTokenAsync(accessToken.AccessToken)/*.ConfigureAwait(false)*/;

            //if(revokeTokenResponse.IsSuccessStatusCode)
            //{
            //    TokenStorage.RemoveAccessToken();
            //    TokenStorage.SaveAccessToken(accessToken.AccessToken, DateTime.UtcNow);
            //}

            if (accessToken != null && !string.IsNullOrEmpty(accessToken.AccessToken))
            {
                if (accessToken.ExpiresDateTime > DateTime.UtcNow)
                {
                    await _remoteFiles.GetRemoteFilesAsync(1, 1000, accessToken.AccessToken);
                    _localFilesObserver.LoadSpecificFolder(_baseFolder);
                    await _remoteFileObserver.RemoteResponseObserverAsync(new GetFilesResponse());

                    UsernameInput.Visibility = Visibility.Hidden;
                    PasswordInput.Visibility = Visibility.Hidden;
                    UsernameLabel.Visibility = Visibility.Hidden;
                    PasswordLabel.Visibility = Visibility.Hidden;
                    LoginBtn.Visibility = Visibility.Hidden;
                    LocalFileTree.Visibility = Visibility.Visible;
                    RemoteFileTree.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameInput.Text) || string.IsNullOrEmpty(PasswordInput.Password))
            {
                MessageBox.Show("Please enter username and password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var requestData = new TokenRequest { Username = UsernameInput.Text, Password = PasswordInput.Password };

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

                            UsernameInput.Visibility = Visibility.Hidden;
                            PasswordInput.Visibility = Visibility.Hidden;
                            LoginBtn.Visibility = Visibility.Hidden;

                            _localFilesObserver.LoadSpecificFolder(_baseFolder);
                            _remoteFileObserver.GetRemoteFilesAsync(1, 1000, tokenResponse.AccessToken);

                            LocalFileTree.Visibility = Visibility.Visible;

                            //TODO Log success login
                        }
                        else
                        {
                            //TODO Log failed login
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO ex lgging
                        throw;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid login", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                //TODO ex lgging
                MessageBox.Show("Invalid login", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<HttpResponseMessage> RevokeTokenAsync(string refreshToken)
        {
            var requestData = new RevokeTokenRequest { Token = refreshToken };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync("https://localhost:7040/authenticate/revoke", content);
        }
    }
}