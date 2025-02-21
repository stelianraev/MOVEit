using Microsoft.Extensions.DependencyInjection;
using MoveitDesktopUI.Services;
using System;
using System.Configuration;
using System.Data;
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
            services.AddSingleton<MainWindow>();
        }
    }

}
