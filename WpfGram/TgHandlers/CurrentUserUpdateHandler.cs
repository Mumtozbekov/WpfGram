using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WpfGram.Helpers;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;
namespace WpfGram.TgHandlers
{
    public class CurrentUserUpdateHandler : Td.ClientResultHandler
    {
        void Td.ClientResultHandler.OnResult(TdApi.BaseObject @object)
        {
            if (@object is TdApi.User)
            {
                TgClientHelper.OnCurrentUserUpdated(@object);
            }
            else
            {
                TgClientHelper.OnUpdatedRecieved(@object);
            }
        }
    }
}
