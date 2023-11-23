using DuplicatesFinder_v4.ViewModels;
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

namespace DuplicatesFinder_v4.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private MainViewModel mainViewModel;
        private bool isClean = false;

        public MainView()
        {
            InitializeComponent();
            mainViewModel = new MainViewModel();
        }

        private void Window_Loaded(object j, RoutedEventArgs e)
        {
            DataContext = mainViewModel;
            pathFromUser.Focus();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainViewModel.DuplicatesViewModel.DeleteTempFilesAsync();
        }

        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isClean )
            {
                pathFromUser.Clear();
                isClean = true;
            }

            if (e.Key == Key.Enter)
            {
                (DataContext as MainViewModel).OnClickSearch.Execute(null);
            }
        }
    }
}
