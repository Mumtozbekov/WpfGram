using System.Windows;

using WpfGram.Helpers;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;
namespace WpfGram.TgHandlers
{
    public class AuthorizationRequestHandler : Td.ClientResultHandler
    {
        void Td.ClientResultHandler.OnResult(TdApi.BaseObject @object)
        {
            if (@object is TdApi.Error error)
            {
                /// Print("Receive an error:" + _newLine + @object);
                MessageBox.Show(error.Message);
                TgClientHelper.OnAuthorizationStateUpdated(null); // repeat last action
            }
            else
            {
                // result is already received through UpdateAuthorizationState, nothing to do
            }
        }
    }
}
