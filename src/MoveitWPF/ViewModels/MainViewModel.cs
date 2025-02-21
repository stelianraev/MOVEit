using MoveitWpf.MoveitWpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MoveitWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public MainViewModel(LoginViewModel loginViewModel)
        {
            CurrentViewModel = loginViewModel;
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
