using System;
using System.Collections.Generic;
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

using Telegram.Td.Api;

using WpfGram.Helpers;
using WpfGram.ViewModels;

namespace WpfGram.Controls
{
    /// <summary>
    /// Логика взаимодействия для ChatUpdatesMessageContent.xaml
    /// </summary>
    public partial class ChatUpdatesMessageContent : UserControl
    {
        private MessageViewModel _viewModel;



        public string TextContent
        {
            get { return (string)GetValue(TextContentProperty); }
            set { SetValue(TextContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextContentProperty =
            DependencyProperty.Register("TextContent", typeof(string), typeof(ChatUpdatesMessageContent), new PropertyMetadata(""));


        public ChatUpdatesMessageContent()
        {
            InitializeComponent();
        }
        public ChatUpdatesMessageContent(MessageViewModel messageViewModel) : this()
        {
            _viewModel = messageViewModel;
        }
        public async override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            switch (_viewModel.Message.Content)
            {
                case MessageChatAddMembers addChatMember:
                    {
                        ContentTemplate = this.FindResource("AddChatMemberTemplate") as DataTemplate;
                        var member = await TgClientHelper.GetUser(addChatMember.MemberUserIds[0]);
                        if (_viewModel.SenderId == member.Id)
                        {
                            TextContent = $"{member.FirstName} {member.LastName} joined the group";
                        }
                        else
                            TextContent = $"{(_viewModel.SenderName.Length > 0 ? _viewModel.SenderName : "Deleted Account")} added {member.FirstName} {member.LastName}";
                    }
                    break;
                case MessageChatDeleteMember chatDeleteMember:
                    {
                        ContentTemplate = this.FindResource("AddChatMemberTemplate") as DataTemplate;
                        var member = await TgClientHelper.GetUser(chatDeleteMember.UserId);
                        if (_viewModel.SenderId == member.Id)
                        {
                            TextContent = $"{member.FirstName} {member.LastName} left the group";
                        }
                        else
                            TextContent = $"{(_viewModel.SenderName.Length > 0 ? _viewModel.SenderName : "Deleted Account")} removed {member.FirstName} {member.LastName}";
                    }
                    break;
                default:
                    ContentTemplate = this.FindResource("AddChatMemberTemplate") as DataTemplate;
                    TextContent = "Unsupported update";
                    break;
            }
        }
    }
}
