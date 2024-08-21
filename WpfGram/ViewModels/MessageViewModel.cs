using System.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

using Telegram.Td.Api;

using WpfGram.Helpers;
using WpfGram.TgHandlers;

namespace WpfGram.ViewModels
{
    public partial class MessageViewModel : ObservableObject
    {
        [ObservableProperty]
        private Message _message;
        [ObservableProperty]
        private Chat _chat;
        [ObservableProperty]
        private Message replyToMessage;
        [ObservableProperty]
        private string replyToUserName;
        [ObservableProperty]
        private MessageContent content;
        [ObservableProperty]
        private string senderPhoto;
        [ObservableProperty]
        private long senderId;
        [ObservableProperty]
        private string senderName;
        [ObservableProperty]
        private bool animationClicked;
        [ObservableProperty]
        private bool isRead;

        public MessageViewModel()
        {
            TgClientHelper.UpdatedRecieved += TgClientHelper_UpdatedRecieved;
        }
        public MessageViewModel(Message message, Chat chat) : this()
        {
            this.Message = message;
            this.Content = message.Content;
            if (!message.IsOutgoing)
                GetSender(_message);

            if (Message.ReplyTo is MessageReplyToMessage replyToMessage)
                GetMessageReplyTo(replyToMessage);

            IsRead = chat.LastReadOutboxMessageId >= _message.Id;
        }

        private void TgClientHelper_UpdatedRecieved(BaseObject obj)
        {
            if (obj is UpdateAnimatedEmojiMessageClicked click)
            {
                if (click.MessageId == Message.Id)
                    AnimationClicked = true;
            }
            switch (obj)
            {
                case UpdateChatReadOutbox updateRead:
                    if (updateRead.ChatId == Message.ChatId && Message.Id <= updateRead.LastReadOutboxMessageId)
                        IsRead = true;
                    break;
                default:
                    break;
            }
        }
        private async void GetMessageReplyTo(MessageReplyToMessage replyToMessage)
        {

            ReplyToMessage = await TgClientHelper.GetMessage(replyToMessage.ChatId, replyToMessage.MessageId);
            ReplyToUserName = await TgClientHelper.GetSenderName(ReplyToMessage?.SenderId);
        }
        private async void GetSender(Message message)
        {
            if (message.SenderId is MessageSenderUser senderUser)
            {
                SenderId = senderUser.UserId;
                var user = await TgClientHelper.GetUser(senderUser.UserId);
                if (user.ProfilePhoto != null && string.IsNullOrEmpty(user?.ProfilePhoto?.Small.Local.Path))
                    TgClientHelper.TgClient.Send(new DownloadFile(user.ProfilePhoto.Small.Id, 1, 0, 1, true), new DefaultHandler());
                SenderPhoto = user.ProfilePhoto?.Small.Local.Path;
                SenderName = $"{user.FirstName} {user.LastName}";
            }
            else if (message.SenderId is MessageSenderChat senderChat)
            {
                SenderId = senderChat.ChatId;
                var chat = await TgClientHelper.GetChat(senderChat.ChatId);
                if (chat.Photo != null && string.IsNullOrEmpty(chat?.Photo?.Small.Local.Path))
                    TgClientHelper.TgClient.Send(new DownloadFile(chat.Photo.Small.Id, 1, 0, 1, true), new DefaultHandler());
                SenderPhoto = chat.Photo?.Small.Local.Path;
                SenderName = chat.Title;
            }
        }
    }
}
