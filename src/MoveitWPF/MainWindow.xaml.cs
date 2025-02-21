using MoveitWpf.ViewModels;
using System.Windows;

namespace MoveitWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;
        private readonly LoginViewModel _loginViewModel;

        public MainWindow(MainViewModel mainViewModel, LoginViewModel loginViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            _loginViewModel = loginViewModel;
            DataContext = _mainViewModel;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _loginViewModel.InitializeAsync();
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            _mainViewModel.CurrentViewModel = viewModel;
        }
    }
}