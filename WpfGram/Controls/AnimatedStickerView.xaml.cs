using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Telegram.Td.Api;

using WpfGram.Helpers;
using WpfGram.TgHandlers;
using WpfGram.Utils;

using TdApi = Telegram.Td.Api;

namespace WpfGram.Controls
{
    /// <summary>
    /// Логика взаимодействия для AnimatedStickerView.xaml
    /// </summary>
    public partial class AnimatedStickerView : ContentControl
    {


        public bool AnimationClicked
        {
            get { return (bool)GetValue(AnimationClickedProperty); }
            set { SetValue(AnimationClickedProperty, value); OnAnimationClick(); }
        }

        // Using a DependencyProperty as the backing store for AnimationClicked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationClickedProperty =
            DependencyProperty.Register("AnimationClicked", typeof(bool), typeof(AnimatedStickerView), new PropertyMetadata(false));

        private  void OnAnimationClick()
        {
            if (AnimationClicked is true)
                PremiumPlayer.PlayAnimation();
        }

        private string AnimatedStickerPath;
        private string PremiumAnimationPath;
        private Message _message;
        public AnimatedStickerView()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _message = DataContext as Message;
            var content = _message.Content;
            Sticker sticker = null;
            if (content is MessageSticker st)
            {
                sticker = st.Sticker;
                if (st.IsPremium)
                    GetPremiumAnimation(sticker.FullType);
            }
            else if (content is MessageAnimatedEmoji anim)
                sticker = anim.AnimatedEmoji.Sticker;

            GetAnimationPath(sticker);

        }



        private async void GetAnimationPath(Sticker? sticker)
        {
            if (sticker != null)
            {

                if (!string.IsNullOrEmpty(sticker.StickerValue.Local.Path) && System.IO.File.Exists(sticker.StickerValue.Local.Path.Replace(".tgs", "")))
                {
                    AnimatedStickerPath = sticker.StickerValue.Local.Path.Replace(".tgs", "");


                }
                else
                {
                    AnimatedStickerPath = await Task.Run(async () =>
                    {

                        var stickerPath = await TgClientHelper.DownloadFile(sticker.StickerValue.Id);
                        if (string.IsNullOrEmpty(stickerPath))
                            return string.Empty;
                        stickerPath = ZipUtils.ExtractEntry(stickerPath, sticker.Id.ToString());
                        return stickerPath;
                    });
                }
                AnimatedStickerPlayer.FileName = AnimatedStickerPath;
                if (!string.IsNullOrEmpty(AnimatedStickerPlayer.FileName))
                    AnimatedStickerPlayer.PlayAnimation();
            }
        }
        private async void GetPremiumAnimation(StickerFullType fullType)
        {

            if (fullType is StickerFullTypeRegular regular && regular.PremiumAnimation is File file)
            {

                if (!string.IsNullOrEmpty(file.Local.Path) && System.IO.File.Exists(file.Local.Path.Replace(".tgs", "")))
                {
                    PremiumAnimationPath = file.Local.Path.Replace(".tgs", "");


                }
                else
                {
                    PremiumAnimationPath = await Task.Run(async () =>
                    {

                        var stickerPath = await TgClientHelper.DownloadFile(file.Id);
                        if (string.IsNullOrEmpty(stickerPath))
                            return string.Empty;
                        stickerPath = ZipUtils.ExtractEntry(stickerPath, file.Id.ToString());
                        return stickerPath;
                    });
                }
                PremiumPlayer.FileName = PremiumAnimationPath;

            }

        }
        private void AnimatedStickerPlayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(PremiumAnimationPath))
            {
                PremiumPlayer.Visibility = Visibility.Visible;
                PremiumPlayer?.PlayAnimation();
                TgClientHelper.TgClient.Send(new ClickAnimatedEmojiMessage(_message.ChatId, _message.Id),new DefaultHandler());
            }
        }

        private void PremiumPlayer_OnStop(object sender, EventArgs e)
        {
            PremiumPlayer.Visibility= Visibility.Collapsed;

        }
    }
}
