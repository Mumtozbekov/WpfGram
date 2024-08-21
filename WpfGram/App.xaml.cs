using System.Threading;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui;

using WpfGram.Helpers;
using WpfGram.Pages;
using WpfGram.Services;
using WpfGram.ViewModels;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace WpfGram
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ServiceProvider serviceProvider;
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();

            Td.Client.Execute(new TdApi.SetLogVerbosityLevel(0));
            if (Td.Client.Execute(new TdApi.SetLogStream(new TdApi.LogStreamFile("tdlib.log", 1 << 27, false))) is TdApi.Error)
            {
                throw new System.IO.IOException("Write access to the current directory is required");
            }
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Td.Client.Run();
            }).Start();

            // create Td.Client
            TgClientHelper.TgClient = TgClientHelper.CreateTdClient();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainPage>();
            services.AddSingleton<MainViewModel>();

            services.AddSingleton<AuthorizationPage>();
            services.AddSingleton<AuthorizationCodePage>();
            services.AddSingleton<AuthorizationPasswordPage>();

            services.AddSingleton<AuthorizationViewModel>();
            services.AddSingleton<AuthorizationCodeViewModel>();
            services.AddSingleton<AuthorizationPasswordViewModel>();
        }


        public static T GetRequiredService<T>()
     where T : class
        {
            return serviceProvider.GetRequiredService<T>();
        }
    }

}
