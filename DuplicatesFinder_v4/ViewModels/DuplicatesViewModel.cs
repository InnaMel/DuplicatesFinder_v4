﻿using DuplicatesFinder_v4.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace DuplicatesFinder_v4.ViewModels
{
    public class DuplicatesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool? ischeckedFile;
        public bool? isCheckedFile
        {
            get
            {
                return (ischeckedFile != null) ? ischeckedFile : true;
            }
            set
            {
                ischeckedFile = value;
                PropertyChanged(this, new PropertyChangedEventArgs("isCheckedFile"));
            }
        }

        public ObservableCollection<ListForViewDuplicates> CollectionForDuplicatesView { get; set; }
        public DuplicatesViewModel()
        {
            CollectionForDuplicatesView = new ObservableCollection<ListForViewDuplicates>();
        }
        public void Divide(ObservableCollection<ObservableCollection<FileConsist>> wholeCollectionFileConsist)
        {
            if (wholeCollectionFileConsist != null)
            {
                //CollectionForDuplicatesView.Clear();
                foreach (var collectionFileConsist in wholeCollectionFileConsist)
                {
                    ListForViewDuplicates eachDuplicatesForView = new ListForViewDuplicates();

                    eachDuplicatesForView.NameDuplicates = collectionFileConsist[0].FileName;

                    eachDuplicatesForView.FullInfoFiles = collectionFileConsist;

                    CollectionForDuplicatesView.Add(eachDuplicatesForView);
                }
            }
        }

        public async void SaveToTxt()
        {
            var dateCreation = DateTime.Now.ToShortDateString();
            var getCurrentNameUser = Environment.UserName;
            var createFolder = Path.Combine($"C:\\Users\\{getCurrentNameUser}\\Downloads", "DuplicatesFinder");
            var setFileSaveName = $"Dublicates_{dateCreation}.txt";
            Directory.CreateDirectory(createFolder);
            var setFilePath = $"{createFolder}\\{setFileSaveName}";
            var option = new JsonSerializerOptions { WriteIndented = true };

            if (File.Exists(setFilePath))
            {
                File.Delete(setFilePath);
            }

            using (FileStream exportFile = new FileStream(setFilePath, FileMode.OpenOrCreate))
            {
                foreach (ListForViewDuplicates listItem in CollectionForDuplicatesView)
                {
                    foreach (FileConsist item in listItem.FullInfoFiles)
                    {
                        await JsonSerializer.SerializeAsync<FileConsist>(exportFile, item, option);
                    }
                }
                exportFile.Close();
            }
            MessageBox.Show("Save was successful completed! \nYou can find the file in folder Downloads\\DuplicatesFinder");
        }
    }
}
