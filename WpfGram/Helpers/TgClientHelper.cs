using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Td.Api;

using WpfGram.TgHandlers;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace WpfGram.Helpers
{
    public class TgClientHelper
    {
        public static Td.Client? TgClient = null;
        private static AuthorizationState _authorizationState;
        public static event Action<AuthorizationState> AuthorizationStateUpdated;
        public static event Action<BaseObject> UpdatedRecieved;
        public static event Action<BaseObject> CurrentUserUpdated;
        private static Dictionary<long, Chat> _chats;

        public static void Init()
        {
            _chats = new();
            TgClient = Td.Client.Create(new UpdateHandler());
        }
        public static void OnAuthorizationStateUpdated(AuthorizationState authorizationState)
        {
            _authorizationState = authorizationState;
            AuthorizationStateUpdated?.Invoke(authorizationState);
        }
        public static void OnUpdatedRecieved(BaseObject @object)
        {
            UpdatedRecieved?.Invoke(@object);
            if (@object is Chat chat)
            {
                if (!_chats.ContainsKey(chat.Id))
                {
                    _chats.Add(chat.Id, chat);
                }
            }
        }
        public static Td.Client CreateTdClient()
        {
            Init();
            return Td.Client.Create(new UpdateHandler());

        }
        public static AuthorizationState GetAuthorizationState()
        {

            return _authorizationState;

        }

        public async static void OnCurrentUserUpdated(BaseObject @object)
        {
            if (@object is User user)
            {
                if (user.ProfilePhoto != null && string.IsNullOrEmpty(user.ProfilePhoto.Big.Local.Path))
                    await DownloadFile(user.ProfilePhoto.Big.Id);

                Global.Data.User = user;
            }
            CurrentUserUpdated?.Invoke(@object);
        }

        public static async void LoadChatsAsync(ChatList chatList)
        {
            TgClientHelper.TgClient.Send(new LoadChats(null, 5), new CustomUpdateHandler(async result =>
            {
                if (result is Ok)
                    LoadChatsAsync(chatList);
                else if (result is Error error && error.Code == 404)
                    return;

            }));

        }
        public static async Task<List<Chat>> GetChats(int page, int limit)
        {
            return _chats.Select(x => x.Value).Skip((page - 1) * limit).Take(limit).ToList();
        }
        public static List<Message> GetMessages(long chatId, long fromMessageId, int offset, int limit)
        {
            var gotMessagesEvent = new ManualResetEventSlim(false);

            var messages = new List<Message>();

            TgClient?.Send(new GetChatHistory { ChatId = chatId, FromMessageId = fromMessageId, Offset = offset, Limit = limit, OnlyLocal = false }, new CustomUpdateHandler(result =>
            {
                if (result is Messages res)
                {
                    foreach (var message in res.MessagesValue)
                    {
                        messages.Add(message);
                    }
                }
                // Set the event to indicate that we got the response
                gotMessagesEvent.Set();


            }));

            // Wait for the response
            gotMessagesEvent.Wait();

            return messages;
        }
        public static async Task<List<Chat>> GetChats(long[] ids)
        {
            var chats = new List<Chat>();

            await Task.Run(() =>
            {
                foreach (var chatId in ids)
                {

                    var gotMessagesEvent = new ManualResetEventSlim(false);

                    TgClient?.Send(new GetChat(chatId), new CustomUpdateHandler(result =>
                    {
                        if (result is TdApi.Chat res)
                        {

                            chats.Add(res);

                        }
                        // Set the event to indicate that we got the response
                        gotMessagesEvent.Set();


                    }));
                    gotMessagesEvent.Wait();
                }
            });

            // Wait for the response

            return chats;
        }

        public async static Task<string> DownloadFile(int fileId)
        {
            var gotMessagesEvent = new AutoResetEvent(false);

            string path = string.Empty;
            await Task.Run(() =>
            {

                // Request the last 50 messages from the chat
                TgClient?.Send(new DownloadFile(fileId, 1, 0, 0, true), new CustomUpdateHandler(result =>
                {
                    if (result is File f)
                    {
                        path = f.Local.Path;
                        gotMessagesEvent.Set();
                    }


                    // Set the event to indicate that we got the response
                }));

                // Wait for the response
                gotMessagesEvent.WaitOne();
            });
            return path;
        }

        public async static Task<Message> GetMessage(long chatId, long messageId)
        {
            var gotMessagesEvent = new AutoResetEvent(false);

            Message message = null;
            await Task.Run(() =>
            {
                TgClient?.Send(new GetMessage(chatId, messageId), new CustomUpdateHandler(result =>
                {
                    if (result is Message m)
                    {
                        message = m;
                    }
                    gotMessagesEvent.Set();
                }));

                gotMessagesEvent.WaitOne();
            });
            return message;
        }

        public async static Task<TdApi.User> GetUser(long userId)
        {
            var gotMessagesEvent = new AutoResetEvent(false);

            TdApi.User user = null;
            await Task.Run(() =>
            {
                TgClient?.Send(new GetUser(userId), new CustomUpdateHandler(result =>
                {
                    if (result is TdApi.User u)
                    {
                        user = u;
                    }
                    gotMessagesEvent.Set();
                }));

                gotMessagesEvent.WaitOne();
            });
            return user;
        }

        public async static Task<TdApi.Chat> GetChat(long chatId)
        {
            var gotMessagesEvent = new AutoResetEvent(false);

            TdApi.Chat? chat = null;
            await Task.Run(() =>
            {
                TgClient?.Send(new GetChat(chatId), new CustomUpdateHandler(result =>
                {
                    if (result is TdApi.Chat c)
                    {
                        chat = c;
                    }
                    gotMessagesEvent.Set();
                }));

                gotMessagesEvent.WaitOne();
            });
            return chat;
        }
        public async static Task<TdApi.BasicGroupFullInfo> GetBasigGroupInfo(long groupId)
        {
            var gotMessagesEvent = new AutoResetEvent(false);

            TdApi.BasicGroupFullInfo group = null;
            await Task.Run(() =>
            {
                TgClient?.Send(new GetBasicGroupFullInfo(groupId), new CustomUpdateHandler(result =>
                {
                    if (result is TdApi.BasicGroupFullInfo c)
                    {
                        group = c;
                    }
                    gotMessagesEvent.Set();
                }));

                gotMessagesEvent.WaitOne();
            });
            return group;
        }
        public async static Task<TdApi.SupergroupFullInfo> GetSupergroupFullInfo(long groupId)
        {
            var gotMessagesEvent = new AutoResetEvent(false);

            TdApi.SupergroupFullInfo group = null;
            await Task.Run(() =>
            {
                TgClient?.Send(new GetSupergroupFullInfo(groupId), new CustomUpdateHandler(result =>
                {
                    if (result is TdApi.SupergroupFullInfo c)
                    {
                        group = c;
                    }
                    gotMessagesEvent.Set();
                }));

                gotMessagesEvent.WaitOne();
            });
            return group;
        }
        public async static Task<TdApi.UserFullInfo> GetUserFullInfo(long userId)
        {
            var gotMessagesEvent = new AutoResetEvent(false);

            TdApi.UserFullInfo user = null;
            await Task.Run(() =>
            {
                TgClient?.Send(new GetUserFullInfo(userId), new CustomUpdateHandler(result =>
                {
                    if (result is TdApi.UserFullInfo c)
                    {
                        user = c;
                    }
                    gotMessagesEvent.Set();
                }));

                gotMessagesEvent.WaitOne();
            });
            return user;
        }

        public async static Task<string> GetSenderName(MessageSender sender)
        {
            string senderName = string.Empty;

            if (sender is MessageSenderUser senderUser)
            {
                var user = await TgClientHelper.GetUser(senderUser.UserId);
                senderName = $"{user?.FirstName} {user?.LastName}";

            }
            else if (sender is MessageSenderChat senderChat)
            {
                var chat = await TgClientHelper.GetChat(senderChat.ChatId);
                senderName = chat?.Title;
            }
            return senderName;
        }

        internal async static Task<FoundChatMessages> GetChatShared(long chatId, TdApi.SearchMessagesFilter _filter, long _fromMessageId = 0)
        {
            var gotMessagesEvent = new AutoResetEvent(false);

            TdApi.FoundChatMessages result = null;
            await Task.Run(() =>
            {
                TgClient?.Send(new SearchChatMessages(chatId, "", null, _fromMessageId, 0, 10, _filter, 0), new CustomUpdateHandler(obj =>
                {
                    if (obj is TdApi.FoundChatMessages cms)
                    {
                        result = cms;
                    }
                    gotMessagesEvent.Set();
                }));

                gotMessagesEvent.WaitOne();
            });
            return result;
        }


    }
}
