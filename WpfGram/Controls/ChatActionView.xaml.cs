using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using LottieSharp.WPF;

using SkiaSharp;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace WpfGram.Controls
{
    /// <summary>
    /// Логика взаимодействия для ChatActionView.xaml
    /// </summary>
    public partial class ChatActionView : UserControl
    {


        public TdApi.ChatAction Action
        {
            get { return (TdApi.ChatAction)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); OnActionChanged(); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(TdApi.ChatAction), typeof(ChatActionView), new FrameworkPropertyMetadata(null) { BindsTwoWayByDefault = true });



        public bool HasAction
        {
            get { return (bool)GetValue(HasActionProperty); }
            set { SetValue(HasActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasActionProperty =
            DependencyProperty.Register("HasAction", typeof(bool), typeof(ChatActionView), new PropertyMetadata(false));



        public ChatActionView()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DataContextChanged += ChatActionView_DataContextChanged;
        }

        private void ChatActionView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnActionChanged();
        }

        private void OnActionChanged()
        {
            
            try
            {
                prefix.Text = postfix.Text = string.Empty;
                if (DataContext is TdApi.ChatAction action)
                {
                    var Player = new LottieAnimationView() { Height = 20, Width = 20, };
                    Player.OnStop += Player_OnStop;
                    //Animation.Content = Player;
                    switch (action)
                    {
                        case TdApi.ChatActionTyping:
                          //  Player.FileName = $"{Global.AppLocationPath}resources/animations/Typing";
                            postfix.Text = "typing";
                            break;
                        case TdApi.ChatActionChoosingSticker:
                           // prefix.Text = "ch";
                           // Player.FileName = $"{Global.AppLocationPath}resources/animations/Look";
                            postfix.Text = "choosing a sticker";

                            break;
                        case TdApi.ChatActionCancel:
                         //   StopAnimation();
                            break;
                        default:
                            return;
                    }
                    //if (!string.IsNullOrEmpty(Player.FileName))
                    //{
                    //    Player.Visibility = Visibility.Visible;

                    //    Player.PlayAnimation();
                    //}
                    this.Visibility = Visibility.Visible;
                    HasAction = true;
                }
                else
                {
                    Visibility = Visibility.Hidden;
                    HasAction = false;
                }
            }
            catch { }
        }

        private void Player_OnStop(object? sender, EventArgs e)
        {
            //DataContext = null;
        }

        private void StopAnimation()
        {
            //if (Player.IsPlaying)
            //    Player.StopAnimation();

            this.Visibility = Visibility.Collapsed;
            DataContext = null;

        }

    }
}
