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
    enum ExtensionsPic
    {
        jpg,
        png,
        bmp,
        jpeg,
        tif,
        svg,
        gif,
        psd,
        ai,
        eps,
        cdr,
        raw
    }

    enum ExtensionsDoc
    {
        txt,
        pdf,
        doc,
        docx,
        rtf,
        xls,
        xlsx
    }

    enum ExtensionsVideo
    {
        mp4,
        mov,
        wmv,
        flv,
        avi,
        mkv
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public Model GetModel { get; set; }
        public DuplicatesViewModel DuplicatesViewModel { get; set; }
        private bool? ispics;
        public bool? isPics
        {
            get
            {
                return (ispics != null) ? ispics : true;
            }
            set
            {
                ispics = value;
                PropertyChanged(this, new PropertyChangedEventArgs("isPics"));
            }
        }

        private bool? isdocs;
        public bool? isDocs
        {
            get
            {
                return (isdocs != null) ? isdocs : true;
            }
            set
            {
                isdocs = value;
                PropertyChanged(this, new PropertyChangedEventArgs("isDocs"));
            }
        }

        private bool? isvideos;
        public bool? isVideos
        {
            get
            {
                return (isvideos != null) ? isvideos : true;
            }
            set
            {
                isvideos = value;
                PropertyChanged(this, new PropertyChangedEventArgs("isVideos"));
            }
        }

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
                    DuplicatesViewModel.CollectionForDuplicatesView.Clear();

                    if (ispics == false && isdocs == false && isvideos == false )
                    {
                        MessageBox.Show("You should make the choice");
                        return;
                    }

                    GetModel.UserPath = EnteredPath;
                    GetModel.Pics = isPics;
                    GetModel.Docs = isDocs;
                    GetModel.Videos = isVideos;
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
