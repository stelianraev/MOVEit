using MoveitDesktopUI.Models;
using MoveitDesktopUI.Services;
using MoveitWpf.MoveitDesktopUI;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace MoveitDesktopUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly LocalFilesObserver _localFilesObserver;
        private readonly string _baseFolder = @"C:\MOVEit";

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _localFilesObserver = new LocalFilesObserver(FileTree);
            CheckIsTokenValid();
        }

        private void CheckIsTokenValid()
        {
            var accessToken = TokenStorage.GetAccessToken();

            if (accessToken != null)
            {
                if (accessToken.ExpiresDateTime > DateTime.UtcNow)
                {
                    UsernameInput.Visibility = Visibility.Hidden;
                    PasswordInput.Visibility = Visibility.Hidden;
                    LoginBtn.Visibility = Visibility.Hidden;
                    _localFilesObserver.LoadSpecificFolder(_baseFolder);
                    FileTree.Visibility = Visibility.Visible;
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

                            FileTree.Visibility = Visibility.Visible;

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
    }
}