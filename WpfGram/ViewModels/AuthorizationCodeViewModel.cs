using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Telegram.Td.Api;

using Wpf.Ui;
using Wpf.Ui.Controls;

using WpfGram.Helpers;
using WpfGram.Pages;
using WpfGram.TgHandlers;

namespace WpfGram.ViewModels
{
    public partial class AuthorizationPasswordViewModel : ObservableObject, INavigationAware
    {
        INavigationService _navigationService;
        [ObservableProperty]
        private string _animationSource;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmPasswordCommand))]
        private string _password="";
        public AuthorizationPasswordViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            AnimationSource = $@"{AppDomain.CurrentDomain.BaseDirectory}Assets\AuthorizationStateWaitPassword.json";
        }


        [RelayCommand(CanExecute = nameof(CanConfirmPassword))]
        private void ConfirmPassword()
        {
            TgClientHelper.TgClient.Send(new CheckAuthenticationPassword(Password), new AuthorizationRequestHandler());


        }

        private bool CanConfirmPassword()
        {
            return !string.IsNullOrEmpty(Password);
        }
        private void OnAuthorizationStateUpdated(AuthorizationState state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                switch (state)
                {
                    case AuthorizationStateWaitPassword:
                        _navigationService.Navigate(typeof(AuthorizationPasswordPage));
                        break;
                    case AuthorizationStateReady:
                        _navigationService.Navigate(typeof(MainPage));
                        break;
                }
            });
        }
        public void OnNavigatedTo()
        {
            TgClientHelper.AuthorizationStateUpdated += OnAuthorizationStateUpdated;
        }



        public void OnNavigatedFrom()
        {
        }
    }
}
