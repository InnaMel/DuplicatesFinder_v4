using DuplicatesFinder_v4.Models;
using DuplicatesFinder_v4.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand((r) =>
                {
                    GetModel.userPath = EnteredPath;
                    DuplicatesViewModel.Divide(GetModel.FindDuplicates());
                }
                ));
            }
        }

    }
}
