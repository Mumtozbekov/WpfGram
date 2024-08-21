using System;
using System.Collections.Generic;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfGram.Models
{
    public partial class UrlPreviewModel:ObservableObject
    {
        [ObservableProperty]
        private string title;
        [ObservableProperty]
        private string imageUrl;

        [ObservableProperty]
        private string description;

      

    }
}
