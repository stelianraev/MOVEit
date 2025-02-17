namespace DesktopUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            CurrentViewModel = new LoginViewModel();
        }

        public ViewModelBase CurrentViewModel { get; }
    }
}
