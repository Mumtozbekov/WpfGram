using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace WpfGram.TgHandlers
{
    internal class CustomUpdateHandler : Td.ClientResultHandler
    {
        private Action<TdApi.BaseObject> CustomAction;
        public CustomUpdateHandler(Action<TdApi.BaseObject> customAction)
        {

            CustomAction = customAction;

        }
        void Td.ClientResultHandler.OnResult(TdApi.BaseObject @object)
        {
            CustomAction?.Invoke(@object);
        }
    }
}
