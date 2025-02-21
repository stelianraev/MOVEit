﻿using Microsoft.Extensions.DependencyInjection;
using MoveitWpf.ViewModels;
using System.Windows;

namespace MoveitWpf
{
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
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            Task.Run(() => loginViewModel.InitializeAsync());

            base.OnStartup(e);
        }
    }
}
