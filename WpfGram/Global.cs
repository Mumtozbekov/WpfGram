

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace WpfGram
{
    public static class Global
    {
        public static string AppLocationPath = AppDomain.CurrentDomain.BaseDirectory;

        public static GlobalData Data { get; set; }
       
        static Global()
        {
            Data = new GlobalData();
        }
    }



    public partial class GlobalData : ObservableObject
    {
        [ObservableProperty]
        private TdApi.User _user;

    }
}
