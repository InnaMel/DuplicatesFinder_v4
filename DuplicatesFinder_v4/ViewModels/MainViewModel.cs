using DuplicatesFinder_v4.Models;
using DuplicatesFinder_v4.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace DuplicatesFinder_v4.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public Model GetModel { get; set; }
        public DuplicatesViewModel DuplicatesViewModel { get; set; }

        public MainViewModel()
        {
            GetModel = new Model();
            DuplicatesViewModel = new DuplicatesViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string enteredPath = "your path here";
        public string EnteredPath
        {
            get { return enteredPath; }
            set
            {
                if (enteredPath != value)
                {
                    enteredPath = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("enteredPath"));
                    }
                }
            }
        }

        private ICommand onClickSearch;
        public ICommand OnClickSearch
        {
            get
            {
                return onClickSearch ?? (onClickSearch = new RelayCommand((r) =>
                {
                    GetModel.userPath = EnteredPath;
                    DuplicatesViewModel.Divide(GetModel.FindDuplicates());
                }
                ));
            }
        }

        private ICommand onClickBrowse;
        public ICommand OnClickBrowse
        {
            get 
            {
                return onClickBrowse ?? ( onClickBrowse = new RelayCommand((r) =>
                {
                    EnteredPath = String.Empty;
                    using (var folderDialog = new FolderBrowserDialog())
                    {
                        DialogResult result = folderDialog.ShowDialog();

                        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                        {
                            EnteredPath = folderDialog.SelectedPath;
                        }
                    }
                }
                ));
            }
        }
    }
}
