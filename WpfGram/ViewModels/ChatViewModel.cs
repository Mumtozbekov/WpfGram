using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using Telegram.Td.Api;

using WpfGram.Helpers;
using WpfGram.TgHandlers;

namespace WpfGram.ViewModels
{
    public partial class ChatViewModel : ObservableObject
    {
        HashSet<long> _messageSet;
        [ObservableProperty]
        private Chat _chat;
        [ObservableProperty]
        private string photo;
        [ObservableProperty]
        private Message _lastMessage;
        [ObservableProperty]
        private ObservableCollection<MessageViewModel> _messages = new();
        [ObservableProperty]
        private UserStatus _status;
        [ObservableProperty]
        private ChatAction _action;
        [ObservableProperty]
        private bool _isTyping;
        [ObservableProperty]
        private double _scrollPosition;
        [ObservableProperty]
        private User _user;

        public ChatViewModel()
        {
            _messageSet = new();

        }
        public ChatViewModel(Chat chat) : this()
        {
            _chat = chat;
            LastMessage = _chat?.LastMessage;
            LoadPhoto();
        }

        private async void LoadPhoto()
        {
            if (_chat.Photo != null && string.IsNullOrEmpty(_chat.Photo.Small.Local.Path))
            {
                TgClientHelper.TgClient.Send(new DownloadFile(_chat.Photo.Small.Id, 1, 0, 1, true), new CustomUpdateHandler((o) =>
                {
                    Photo = _chat.Photo.Small.Local.Path;
                }));
            }
            else
                Photo = _chat.Photo?.Small?.Local?.Path;
        }
        public async void GetUser()
        {
            if (User == null)
                User = await TgClientHelper.GetUser(Chat.Id);
        }
        public async Task GetMessages(int limit = 10, bool local = false)
        {

            await Task.Run(async () =>
            {
                long fromMessageId;
                if (Messages.Count > 0)
                {

                    fromMessageId = Messages.Last().Message.Id;
                }
                else
                    fromMessageId = 0;

                TgClientHelper.TgClient?.Send(new GetChatHistory { ChatId = Chat.Id, FromMessageId = fromMessageId, Offset = 0, Limit = limit, OnlyLocal = local }, new CustomUpdateHandler(async result =>
                {
                    if (result is Messages res)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var message in res.MessagesValue)
                            {
                                if (!_messageSet.Contains(message.Id))
                                {
                                    Messages.Add(new MessageViewModel(message, Chat));
                                    _messageSet.Add(message.Id);
                                }
                            }
                        });
                    }
                    // Set the event to indicate that we got the response


                }));
            });


        }
    }
}