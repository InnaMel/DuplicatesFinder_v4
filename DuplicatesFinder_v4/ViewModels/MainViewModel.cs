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
        private bool? ispics;
        private bool? isdocs;
        private bool? isvideos;
        private bool visibilityRectangle = false;
        private double opacityRectangle = 0.0;
        private string enteredPath = "your path here";
        private ICommand onClickSearch;
        private ICommand onClickBrowse;
        private ICommand onClickExport;
        private event PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        public Model GetModel { get; set; }
        public DuplicatesViewModel DuplicatesViewModel { get; set; }

        public bool? IsPics
        {
            get
            {
                return (ispics != null) ? ispics : true;
            }
            set
            {
                ispics = value;
                propertyChanged(this, new PropertyChangedEventArgs("IsPics"));
            }
        }

        public bool? IsDocs
        {
            get
            {
                return (isdocs != null) ? isdocs : true;
            }
            set
            {
                isdocs = value;
                propertyChanged(this, new PropertyChangedEventArgs("IsDocs"));
            }
        }

        public bool? IsVideos
        {
            get
            {
                return (isvideos != null) ? isvideos : true;
            }
            set
            {
                isvideos = value;
                propertyChanged(this, new PropertyChangedEventArgs("IsVideos"));
            }
        }

        public bool VisibilityRectangle
        {
            get { return visibilityRectangle; }
            set
            {
                visibilityRectangle = value;
                propertyChanged(this, new PropertyChangedEventArgs("VisibilityRectangle")); ;
            }
        }
        
        public double OpacityRectangle
        {
            get { return opacityRectangle; }
            set
            {
                opacityRectangle = value;
                propertyChanged(this, new PropertyChangedEventArgs("OpacityRectangle")); ;
            }
        }

        public string EnteredPath
        {
            get { return enteredPath; }
            set
            {
                if (enteredPath != value)
                {
                    enteredPath = value;
                    if (propertyChanged != null)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("enteredPath"));
                    }
                }
            }
        }
        
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
                        Process.Start(DuplicatesViewModel.PathWithAppFolder);
                    }
                }
                ));
            }
        }
       
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

                    OpacityRectangle = 0.2;
                    VisibilityRectangle = true;

                    GetModel.BeginFindDuplicates(list =>
                        {
                            RunOnMainThread(() =>
                            {
                                if (list.Count == 0)
                                    System.Windows.MessageBox.Show("No one matches");

                                DuplicatesViewModel.Divide(list);

                                OpacityRectangle = 0.0;
                                VisibilityRectangle = false;
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

        public MainViewModel()
        {
            GetModel = new Model();
            DuplicatesViewModel = new DuplicatesViewModel();
        }

        public void RunOnMainThread(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
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
    }
}
