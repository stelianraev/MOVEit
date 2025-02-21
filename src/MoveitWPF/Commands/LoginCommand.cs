using MoveitWpf.ViewModels;

namespace MoveitWpf.Commands
{
    public class LoginCommand : AsyncCommandBase
    {
        private LoginViewModel _loginViewModel;
        public LoginCommand(LoginViewModel loginViewModel, CancellationToken cancellationToken)
        {
            _loginViewModel = loginViewModel;   
        }
        public override async Task ExecuteAsync(object parameter)
        {
            await _loginViewModel.LoginAsync();

            return;
        }
    }
}
