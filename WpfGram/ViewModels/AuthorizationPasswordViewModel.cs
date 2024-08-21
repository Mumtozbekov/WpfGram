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
    public partial class AuthorizationCodeViewModel : ObservableObject, INavigationAware
    {
        INavigationService _navigationService;
        [ObservableProperty]
        private string _animationSource;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmCodeCommand))]
        private string _confirmationCode;
        public AuthorizationCodeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            AnimationSource = $@"{AppDomain.CurrentDomain.BaseDirectory}\Assets\AuthorizationStateWaitCode.json";
        }


        [RelayCommand(CanExecute = nameof(CanConfirmCode))]
        private void ConfirmCode()
        {
            TgClientHelper.TgClient.Send(new CheckAuthenticationCode(ConfirmationCode), new AuthorizationRequestHandler());

        }

        private bool CanConfirmCode()
        {
            return !string.IsNullOrEmpty(ConfirmationCode);
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
