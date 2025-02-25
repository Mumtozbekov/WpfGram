﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Telegram.Td.Api;

using Typography.OpenFont.Tables;

using WpfGram.Helpers;
using WpfGram.TgHandlers;

namespace WpfGram.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private HashSet<long> _chatsSet;

        [ObservableProperty]
        private ObservableCollection<ChatViewModel> _chats;
        [ObservableProperty]
        private ChatViewModel _selectedChat;
        private int _chatsPage = 1;
        private int _chatsLimit = 10;
        public MainViewModel()
        {
            Chats = new();
            _chatsSet = new();
            ICollectionView view = CollectionViewSource.GetDefaultView(Chats);

            view.SortDescriptions.Add(new SortDescription("LastMessage.Date", ListSortDirection.Descending));
            TgClientHelper.UpdatedRecieved += TgClientHelper_UpdatedRecieved;
            TgClientHelper.LoadChatsAsync(null);
            LoadChats();
        }

        private async void LoadChats()
        {
            var chats = await TgClientHelper.GetChats(_chatsPage, _chatsLimit);
            foreach (var chat in chats)
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {

                    if (!_chatsSet.Contains(chat.Id))
                    {
                        _chatsSet.Add(chat.Id);
                        Chats.Add(new(chat));
                    }
                });
            }
        }
        private async void TgClientHelper_UpdatedRecieved(BaseObject @obj)
        {
            await Task.Run(async () =>
            {
                if (obj is File photo)
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        var chatVm = Chats.FirstOrDefault(x => x.Chat.Photo?.Small.Id == photo.Id);
                        if (chatVm != null)
                        {
                            chatVm.Chat.Photo.Small.Local.Path = photo.Local.Path;
                        }
                    });
                }

                switch (obj)
                {
                    case UpdateNewMessage msg:
                        if (_chatsSet.Contains(msg.Message.ChatId))
                        {
                            var chatVm = Chats.FirstOrDefault(x => x.Chat.Id == msg.Message.ChatId);

                            await Task.Run(async () =>
                            {

                                await Application.Current.Dispatcher.InvokeAsync(async () =>
                                {

                                    if (chatVm != null)
                                    {
                                        chatVm.LastMessage = msg.Message;
                                        chatVm.Messages.Insert(0, new MessageViewModel(msg.Message, chatVm.Chat));
                                    }

                                });
                            });
                        }
                        else
                        {
                            _chatsSet.Add(msg.Message.ChatId);
                            var newChat = await TgClientHelper.GetChats([msg.Message.ChatId]);
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                Chats.Add(new(newChat[0]));
                            });
                        }
                        // SortChats();
                        break;
                    case UpdateUserStatus updateStatus:

                        if (_chatsSet.Contains(updateStatus.UserId))
                        {
                            Chats.FirstOrDefault(c => c.Chat.Id == updateStatus.UserId).Status = updateStatus.Status;
                        }
                        break;
                    case UpdateChatAction updateAction:
                        if (_chatsSet.Contains(updateAction.ChatId))
                        {
                            var chat = Chats.FirstOrDefault(c => c.Chat.Id == updateAction.ChatId);
                            chat.Action = updateAction.Action;
                            chat.IsTyping = true;
                            await Task.Delay(2000);
                            chat.Action = null;
                            chat.IsTyping = false;


                        }
                        break;

                    case UpdateChatPosition pos:
                        if (_chatsSet.Contains(pos.ChatId))
                        {

                            var updatedchat = Chats.FirstOrDefault(c => c.Chat.Id == pos.ChatId);
                            if (updatedchat != null && updatedchat.Chat.Positions.Length > 0)
                                updatedchat.Chat.Positions[0] = pos.Position;
                            //  SortChats();
                        }
                        else
                        {
                            TgClientHelper.TgClient.Send(new GetChat(pos.ChatId), new UpdateHandler());

                        }
                        break;
                   

                }
            });
        }

        public async void SortChats()
        {
            try
            {

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var sortedItemsList = Chats.OrderByDescending(c => c.Chat.Positions?[0]?.Order).ThenBy(c => c?.LastMessage?.Date).ToList();
                    //var sortedItemsList = Chats.OrderByDescending(c => c?.LastMessage?.Date).ThenBy(c => c.Chat.Positions?[0]?.Order).ToList();

                    foreach (var item in sortedItemsList)
                    {
                        Chats.Move(Chats.IndexOf(item), sortedItemsList.IndexOf(item));
                    }

                });
            }
            catch { }
        }

        public async void MessagesScrollChanged(object parameter)
        {

            if (parameter is ScrollViewer scv)
            {
                if (scv.ScrollableHeight < 0 || scv.ScrollableHeight <= scv.VerticalOffset + 20)
                {
     
                    if (SelectedChat != null)
                        await SelectedChat.GetMessages(5);
                }
            }
        }
        public async void ChatsScrollChanged(object parameter)
        {

            if (parameter is ScrollViewer scv)
            {

                if (scv.ScrollableHeight < 0 || scv.ScrollableHeight <= scv.VerticalOffset + 20 && _chatsSet.Count > 0)
                {
                    _chatsPage += 1;
                    LoadChats();
                }
            }
        }
        internal void ChatSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 0 || (e.RemovedItems[0] is ChatViewModel cv && !cv.Equals(_selectedChat)))
            {

                if (_selectedChat != null && _selectedChat.Messages.Count == 0)
                {
                    _selectedChat.GetMessages(100, true);
                }
                //_selectedChat.LoadChatInfo();

                //ChatInfoFrame?.Navigate(new ChatInfoPage(_selectedChat));



            }
            _selectedChat?.GetUser();
        }
        [RelayCommand]
        private void SendMessage(object o)
        {
            if (o is string _text)
            {
                InputMessageContent content = new InputMessageText(new FormattedText(_text, null), new LinkPreviewOptions(), true);
                TgClientHelper.TgClient.Send(new SendMessage(SelectedChat.Chat.Id, 0, null, null, null, content), new DefaultHandler());
                _text = null;
            }
        }

    }
}
