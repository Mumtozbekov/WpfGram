using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Td.Api;

using Wpf.Ui;
using Wpf.Ui.Controls;


using WpfGram.Extensions;
using WpfGram.Helpers;
using WpfGram.Pages;
using WpfGram.Services;
using WpfGram.TgHandlers;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;
namespace WpfGram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow, INavigationWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(
            INavigationService navigationService,
            IPageService pageService,
             IServiceProvider serviceProvider,
            IContentDialogService dialogService) : this()
        {
            dialogService.SetContentPresenter(RootContentDialog);
            navigationService.SetNavigationControl(RootNavigation);

            SetPageService(pageService);
            SetServiceProvider(serviceProvider);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            switch (TgClientHelper.GetAuthorizationState())
            {

                case AuthorizationStateWaitTdlibParameters:
                    {
                        SetTdlibParameters request = new SetTdlibParameters();
                        request.DatabaseDirectory = "tdlib";
                        request.UseMessageDatabase = true;
                        request.UseChatInfoDatabase = true;
                        request.UseFileDatabase = true;
                        request.UseSecretChats = true;
                        request.ApiId = Constants.ApiId;
                        request.ApiHash = Constants.ApiHash;
                        request.SystemLanguageCode = "en";
                        request.DeviceModel = "Desktop";
                        request.ApplicationVersion = "1.0";
                        request.EnableStorageOptimizer = true;
                        TgClientHelper.TgClient.Send(request, new AuthorizationRequestHandler());
                        //NavigationService.Navigate(new RegistrationPage());
                        TgClientHelper.AuthorizationStateUpdated += TgClientHelper_AuthorizationStateUpdated;
                    }
                    break;


            }


        }

        private void TgClientHelper_AuthorizationStateUpdated(TdApi.AuthorizationState _authorizationState)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (_authorizationState)
                {
                    case AuthorizationStateWaitPhoneNumber:

                        Navigate(typeof(AuthorizationPage));
                        TgClientHelper.TgClient.Send(new GetMe(), new CurrentUserUpdateHandler());
                        //Application.Current.Dispatcher.Invoke(new Action(() => NavigationService.Navigate(new MainPage())));
                        TgClientHelper.AuthorizationStateUpdated -= TgClientHelper_AuthorizationStateUpdated;
                        break;
                    case AuthorizationStateWaitCode:
                        Navigate(typeof(AuthorizationCodePage));
                        break;
                    case AuthorizationStateWaitPassword:
                        Navigate(typeof(AuthorizationPasswordPage));
                        break;
                    case AuthorizationStateReady:
                        TgClientHelper.TgClient.Send(new GetMe(), new CurrentUserUpdateHandler());
                        Navigate(typeof(MainPage));
                        break;
                }
            });
        }
        #region NavigationWindowMethods
        public void CloseWindow() => Close();

        public Frame GetFrame() => RootFrame;


        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);



        public void SetPageService(IPageService pageService)
        {
            RootNavigation.SetPageService(pageService);
        }

        public void ShowWindow() => Show();

        INavigationView INavigationWindow.GetNavigation() => RootNavigation;

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {

        }
        #endregion
    }
}