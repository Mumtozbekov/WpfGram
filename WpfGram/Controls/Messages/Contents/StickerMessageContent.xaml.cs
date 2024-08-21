using System.Windows.Controls;

using WpfGram.ViewModels;

namespace WpfGram.Controls
{
    /// <summary>
    /// Логика взаимодействия для StickerMessageContent.xaml
    /// </summary>
    public partial class StickerMessageContent : UserControl
    {
        public StickerMessageContent()
        {
            InitializeComponent();
        }
        public StickerMessageContent(MessageViewModel message) : this()
        {
            DataContext = message;
        }
    }
}
