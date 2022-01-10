using DuplicatesFinder_v4.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

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
        public bool? IsPics
        {
            get
            {
                return (ispics != null) ? ispics : true;
            }
            set
            {
                ispics = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsPics"));
            }
        }

        private bool? isdocs;
        public bool? IsDocs
        {
            get
            {
                return (isdocs != null) ? isdocs : true;
            }
            set
            {
                isdocs = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsDocs"));
            }
        }

        private bool? isvideos;
        public bool? IsVideos
        {
            get
            {
                return (isvideos != null) ? isvideos : true;
            }
            set
            {
                isvideos = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsVideos"));
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

                    if (ispics == false && isdocs == false && isvideos == false)
                    {
                        System.Windows.MessageBox.Show("You should make the choice");
                        return;
                    }

                    GetModel.UserPath = EnteredPath;
                    GetModel.Pics = IsPics;
                    GetModel.Docs = IsDocs;
                    GetModel.Videos = IsVideos;

                    GetModel.BeginFindDuplicates(list =>
                        {
                            RunOnMainThread(() =>
                            {
                                if (list.Count == 0)
                                    System.Windows.MessageBox.Show("No one matches");

                                DuplicatesViewModel.Divide(list);
                            });
                        });

                    // RunOnMainThread(() =>
                    //{ 
                    //    DuplicatesViewModel.Divide(GetModel.FindDuplicatesAsync()); 
                    //});

                    //Task.Run(() =>
                    //{
                    //    ObservableCollection<ObservableCollection<FileConsist>> findedDuplicates = GetModel.FindDuplicates();
                    //    OnResult(findedDuplicates);
                    //});
                }
                ));
            }
        }

        //private void OnResult(ObservableCollection<ObservableCollection<FileConsist>> list)
        //{
        //    RunOnMainThread(() =>
        //        {
        //            if (list.Count == 0)
        //                MessageBox.Show("No one matches");

        //            DuplicatesViewModel.Divide(list);
        //        });
        //}

        private ICommand onClickBrowse;
        public ICommand OnClickBrowse
        {
            get
            {
                return onClickBrowse ?? (onClickBrowse = new RelayCommand((r) =>
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

        private ICommand onClickExport;
        public ICommand OnClickExport
        {
            get
            {
                return onClickExport ?? (onClickExport = new RelayCommand((r) =>
                {
                    Task.Run(() => DuplicatesViewModel.SaveToTxt());
                    MessageBoxResult result = System.Windows.MessageBox.Show(
                        "Save was successful completed! \nOpen containing folder ? ", 
                        "DuplicatesFinder", 
                        MessageBoxButton.YesNo, 
                        MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(DuplicatesViewModel.pathWithAppFolder);
                    }
                }
                ));
            }
        }

        public void RunOnMainThread(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}
