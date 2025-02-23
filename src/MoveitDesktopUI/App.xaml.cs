using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MoveitDesktopUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<MainWindow>();
        }
    }

}
