using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Wpf.Ui;

using WpfGram.ViewModels;

namespace WpfGram.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private MainViewModel viewModel => DataContext as MainViewModel;
        public MainPage()
        {
            InitializeComponent();
        }
        public MainPage(INavigationService navigationService, IPageService pageService, MainViewModel viewModel) : this()
        {
            DataContext = viewModel;
            //navigationService.SetNavigationControl(NavigationStore);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            viewModel?.ChatSelectionChanged(e);

        }

    }
}
