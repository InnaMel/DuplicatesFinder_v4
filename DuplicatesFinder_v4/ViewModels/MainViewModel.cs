﻿using DuplicatesFinder_v4.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
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
        private const string TextSuccessfulCompleted = "Save was successful completed! \nOpen containing folder ? ";
        private bool? ispics;
        private bool? isdocs;
        private bool? isvideos;
        private string enteredPath = "choose a directory";

        private ICommand onClickSearch;
        private ICommand onClickBrowse;
        private ICommand onClickExport;
        private ICommand onClickDelete;
        private ICommand onClickUndoDelete;

        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        public Model MainModel { get; set; }
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
                    DuplicatesViewModel.SaveToTxtAsync(() =>
                        {
                            RunOnMainThread(() =>
                            {
                                MessageBoxResult result = System.Windows.MessageBox.Show(
                                    TextSuccessfulCompleted,
                                    "DuplicatesFinder",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Information);

                                if (result == MessageBoxResult.Yes)
                                {
                                    Process.Start(DuplicatesViewModel.PathWithAppFolder);
                                }
                            });
                        });
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
                    DuplicatesViewModel.DeleteTempFilesAsync();

                    if (ispics == false && isdocs == false && isvideos == false)
                    {
                        System.Windows.MessageBox.Show("You should make the choice");
                        return;
                    }

                    MainModel.UserPath = EnteredPath;
                    MainModel.Pics = IsPics;
                    MainModel.Docs = IsDocs;
                    MainModel.Videos = IsVideos;

                    MainModel.BeginFindDuplicates(list =>
                        {
                            RunOnMainThread(() =>
                            {
                                if (list?.Count == 0)
                                    System.Windows.MessageBox.Show("No one matches");

                                DuplicatesViewModel.Divide(list);
                            });
                        });
                }
                ));
            }
        }

        public ICommand OnClickDelete
        {
            get
            {
                return onClickDelete ?? (onClickDelete = new RelayCommand((r) =>
                {
                    DuplicatesViewModel.DeleteCheckedItemsAsync();
                }
                ));
            }
        }

        public ICommand OnClickUndoDelete
        {
            get
            {
                return onClickUndoDelete ?? (onClickUndoDelete = new RelayCommand((r) =>
                {
                    DuplicatesViewModel.UndoDeleteCheckedItemsAsync();
                }
                ));
            }
        }

        public MainViewModel()
        {
            MainModel = new Model();
            DuplicatesViewModel = new DuplicatesViewModel();
        }

        public void RunOnMainThread(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}
