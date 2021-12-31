﻿using DuplicatesFinder_v4.ViewModels;
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
        bool isClean = false;
        public MainView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object j, RoutedEventArgs e)
        {
            DataContext = new MainViewModel();
            pathFromUser.Focus();
        }

        void ClearTextBox(Object j, RoutedEventArgs arg)
        {
            if (!isClean)
            {
                pathFromUser.Clear();
                isClean = true;
            }
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
