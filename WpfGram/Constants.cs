using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfGram
{
    internal class Constants
    {
        public static int ApiId;
        public static string ApiHash = "";
        public static Dictionary<string, string> AvatarColors = new Dictionary<string, string>(){

            {"LightBlue","#81a4d8"},
            {"LightOrange","#e3aa7a"},
            {"LightRed","#c57278"},
            {"LightPurple","#9f88df"},
            {"LightGreen","#93c96a"},
            {"LightCyan","#91c7c8"},
            {"LightPink","#d379ac"},

            {"Red","#d6454c"},
            {"Orange","#d99546"},
            {"Purple","#775cbc"},
            {"Green","#3ea34a"},
            {"Cyan","#6ca2a4"},
            {"Blue","#3e7eba"},
            {"Pink","#b04f85"}
        };
    }
}
