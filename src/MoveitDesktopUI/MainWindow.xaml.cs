using Microsoft.AspNetCore.SignalR.Client;
using MoveitDesktopUI.Models;
using MoveitDesktopUI.Services;
using MoveitWpf.MoveitDesktopUI.Services;
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
        private readonly CancellationTokenSource _cancellationTokenSource;
        private GetRemoteFiles _remoteFiles;
        private HubConnection _hubConnection;

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

            Task.Run(ConnectToSignalRAsync);
            Task.Run(() => RevokeTokenChecker(_cancellationTokenSource.Token));
        }

        private async Task ConnectToSignalRAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7040/file-change/notify")
                .WithAutomaticReconnect()
                .Build();

            //Get notification when a file is uploaded
            _hubConnection.On<string>("FileUploaded", async (message) =>
            {
                var accesstoken = TokenStorage.GetAccessToken()?.AccessToken;

                if (String.IsNullOrEmpty(accesstoken))
                {
                    return;
                }

                var remoteFiles = await _remoteFiles.GetRemoteFilesAsync(1, 1000, accesstoken);
                _localFilesObserver.LoadSpecificFolder(_baseFolder);
                await _remoteFileObserver.RemoteResponseObserverAsync(remoteFiles);
            });

            //Get notification when a file is uploaded
            _hubConnection.On<string>("FileDeleted", async (message) =>
            {
                var accesstoken = TokenStorage.GetAccessToken()?.AccessToken;

                if (String.IsNullOrEmpty(accesstoken))
                {
                    return;
                }

                var remoteFiles = await _remoteFiles.GetRemoteFilesAsync(1, 1000, accesstoken);
                await Dispatcher.InvokeAsync(() => RemoteFileTree.Items.Clear());
                _localFilesObserver.LoadSpecificFolder(_baseFolder);
                await _remoteFileObserver.RemoteResponseObserverAsync(remoteFiles);
            });

            await _hubConnection.StartAsync();
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
                            TokenStorage.RemoveAccessToken();
                            TokenStorage.SaveAccessToken(tokenResponse.AccessToken, tokenResponse.ExpiresIn, tokenResponse.RefreshToken, DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));

                            var remoteFiles = await _remoteFiles.GetRemoteFilesAsync(1, 1000, tokenResponse.AccessToken);
                            _localFilesObserver.LoadSpecificFolder(_baseFolder);
                            await _remoteFileObserver.RemoteResponseObserverAsync(remoteFiles);

                            UsernameInput.Visibility = Visibility.Hidden;
                            PasswordInput.Visibility = Visibility.Hidden;
                            UsernameLabel.Visibility = Visibility.Hidden;
                            PasswordLabel.Visibility = Visibility.Hidden;
                            LoginBtn.Visibility = Visibility.Hidden;
                            LocalFileTree.Visibility = Visibility.Visible;
                            RemoteFileTree.Visibility = Visibility.Visible;

                            Console.WriteLine("Login success");
                        }
                        else
                        {
                            UsernameInput.Visibility = Visibility.Visible;
                            PasswordInput.Visibility = Visibility.Visible;
                            UsernameLabel.Visibility = Visibility.Visible;
                            PasswordLabel.Visibility = Visibility.Visible;
                            LoginBtn.Visibility = Visibility.Visible;

                            LocalFileTree.Visibility = Visibility.Hidden;
                            RemoteFileTree.Visibility = Visibility.Hidden;

                            Console.WriteLine("Login unsuccessful");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
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
                Console.WriteLine($"Error: {ex.Message}");
                MessageBox.Show("Invalid login", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CheckIsTokenValid()
        {
            var accessToken = IsTokenValid();

            if (accessToken.IsTokenValid)
            {
                var remoteFiles = await _remoteFiles.GetRemoteFilesAsync(1, 1000, accessToken!.Token!.AccessToken);
                _localFilesObserver.LoadSpecificFolder(_baseFolder);
                await _remoteFileObserver.RemoteResponseObserverAsync(remoteFiles);

                UsernameInput.Visibility = Visibility.Hidden;
                PasswordInput.Visibility = Visibility.Hidden;
                UsernameLabel.Visibility = Visibility.Hidden;
                PasswordLabel.Visibility = Visibility.Hidden;
                LoginBtn.Visibility = Visibility.Hidden;
                LocalFileTree.Visibility = Visibility.Visible;
                RemoteFileTree.Visibility = Visibility.Visible;
            }
            else
            {
                UsernameInput.Visibility = Visibility.Visible;
                PasswordInput.Visibility = Visibility.Visible;
                UsernameLabel.Visibility = Visibility.Visible;
                PasswordLabel.Visibility = Visibility.Visible;
                LoginBtn.Visibility = Visibility.Visible;

                LocalFileTree.Visibility = Visibility.Hidden;
                RemoteFileTree.Visibility = Visibility.Hidden;
            }
        }

        private async Task RevokeTokenChecker(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var accessToken = IsTokenValid();

                    if (!accessToken.IsTokenValid)
                    {
                        if (accessToken.Token == null)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(20));
                            continue;
                        }
                        else
                        {
                            //var revokeTokenResponse = await RevokeTokenAsync(accessToken!.Token!.AccessToken);
                            //var responseContent = await revokeTokenResponse.Content.ReadAsStringAsync();
                            //var isTokenRevoked = JsonConvert.DeserializeObject<RevokeTokenResponse>(responseContent);

                            //if (isTokenRevoked != null && isTokenRevoked.StatusCode == "200")
                            //{
                            //    var revokeToken = TokenStorage.GetAccessToken();
                            //    TokenStorage.SaveAccessToken(revokeToken.AccessToken, revokeToken.TokenExpireSeconds, revokeToken.RefreshToken, DateTime.UtcNow.AddSeconds(revokeToken.TokenExpireSeconds));
                            //}

                            UsernameInput.Visibility = Visibility.Visible;
                            PasswordInput.Visibility = Visibility.Visible;
                            UsernameLabel.Visibility = Visibility.Visible;
                            PasswordLabel.Visibility = Visibility.Visible;
                            LoginBtn.Visibility = Visibility.Visible;

                            LocalFileTree.Visibility = Visibility.Hidden;
                            RemoteFileTree.Visibility = Visibility.Hidden;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token renewal error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }


        //private async Task<HttpResponseMessage> RevokeTokenAsync(string token)
        //{
        //    var requestData = new Dictionary<string, string>
        //        {
        //              { "token", token }
        //        };

        //    var json = JsonConvert.SerializeObject(requestData);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    return await _httpClient.PostAsync("https://localhost:7040/authenticate/revoke", content);
        //}

        private (bool IsTokenValid, TokenStorage.TokenData? Token) IsTokenValid()
        {
            var token = TokenStorage.GetAccessToken();

            if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            {
                var expiresDateTime = token.ExpiresDateTime - DateTime.UtcNow;

                if (expiresDateTime.Minutes >= 1)
                {
                    return (true, token);
                }
                else
                {
                    return (false, token);
                }
            }

            return (false, null);
        }
    }
}