using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WpfGram.Helpers;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace WpfGram.TgHandlers
{
    public class UpdateHandler : Td.ClientResultHandler
    {
        void Td.ClientResultHandler.OnResult(TdApi.BaseObject @object)
        {
            if (@object is TdApi.UpdateAuthorizationState)
            {
                TgClientHelper.OnAuthorizationStateUpdated((@object as TdApi.UpdateAuthorizationState).AuthorizationState);
            }
            else
            {
                TgClientHelper.OnUpdatedRecieved(@object);
            }
        }
    }

    public class DefaultHandler : Td.ClientResultHandler
    {
        void Td.ClientResultHandler.OnResult(TdApi.BaseObject @object)
        {
           // Debug.WriteLine(@object.ToString());

            TgClientHelper.OnUpdatedRecieved(@object);
        }
    }
}
