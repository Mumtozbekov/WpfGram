using System.Collections.Generic;
using System.Drawing;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using QRCoder;

using Telegram.Td.Api;

using Wpf.Ui;
using Wpf.Ui.Controls;

using WpfGram.Entities;
using WpfGram.Helpers;
using WpfGram.Pages;
using WpfGram.TgHandlers;


namespace WpfGram.ViewModels
{
    public partial class AuthorizationViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private Bitmap qrImage;
        [ObservableProperty]
        private string phoneNumber;

        [ObservableProperty]
        private IList<Country> countries;
        [ObservableProperty]
        private Country selectedCountry;

        INavigationService _navigationService;
        public AuthorizationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        [RelayCommand]
        private void Next()
        {
            var _phoneNumber = phoneNumber?.Trim('+').Replace(" ", string.Empty);
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return;
            }
            TgClientHelper.TgClient.Send(new SetAuthenticationPhoneNumber(_phoneNumber, null), new AuthorizationRequestHandler());

        }


        private void OnAuthorizationStateUpdated(AuthorizationState state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {



                switch (state)
                {
                    case AuthorizationStateWaitPhoneNumber number:
                        break;
                    case AuthorizationStateWaitCode code:
                        _navigationService.Navigate(typeof(AuthorizationCodePage));
                        break;
                    case AuthorizationStateWaitPassword:
                        _navigationService.Navigate(typeof(AuthorizationPasswordPage));
                        break;
                    case AuthorizationStateReady:
                        _navigationService.Navigate(typeof(MainPage));
                        break;
                    case AuthorizationStateWaitOtherDeviceConfirmation waitOtherDeviceConfirmation:
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(waitOtherDeviceConfirmation.Link, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        QrImage = qrCode.GetGraphic(20);
                        break;
                }
            });
        }

        #region NavigationAwareMethiods
        public void OnNavigatedFrom()
        {

        }

        public void OnNavigatedTo()
        {
            TgClientHelper.AuthorizationStateUpdated += OnAuthorizationStateUpdated;
            Countries = Country.All;
        }

        #endregion
    }

}
