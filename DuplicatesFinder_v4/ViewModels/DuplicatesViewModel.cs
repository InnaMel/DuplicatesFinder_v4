using DuplicatesFinder_v4.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DuplicatesFinder_v4.ViewModels
{
    public class DuplicatesViewModel : INotifyPropertyChanged
    {
        private static string pathWithAppFolder;
        private event PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        public static string PathWithAppFolder
        {
            get
            {
                var getCurrentNameUser = Environment.UserName;
                pathWithAppFolder = Path.Combine($"C:\\Users\\{getCurrentNameUser}\\Downloads", "DuplicatesFinder");
                Directory.CreateDirectory(pathWithAppFolder);
                return pathWithAppFolder;
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
                foreach (var collectionFileConsist in wholeCollectionFileConsist)
                {
                    var eachDuplicatesForView = new ListForViewDuplicates();

                    eachDuplicatesForView.NameDuplicates = collectionFileConsist[0].FileName;

                    eachDuplicatesForView.FullInfoFiles = collectionFileConsist;

                    CollectionForDuplicatesView.Add(eachDuplicatesForView);
                }
            }
        }

        public async void SaveToTxt()
        {
            var dateCreation = DateTime.Now.ToShortDateString();
            var setFileSaveName = $"Dublicates_{dateCreation}.txt";
            var setFilePath = Path.Combine(PathWithAppFolder, setFileSaveName);
            var option = new JsonSerializerOptions { WriteIndented = true };

            if (File.Exists(setFilePath))
            {
                File.Delete(setFilePath);
            }

            using (FileStream exportFile = new FileStream(setFilePath, FileMode.OpenOrCreate))
            {
                foreach (var listItem in CollectionForDuplicatesView)
                {
                    foreach (var item in listItem.FullInfoFiles)
                    {
                        await JsonSerializer.SerializeAsync<FileConsist>(exportFile, item, option);
                    }
                }
            }
        }

        public void DeleteCheckedItems()
        {
            deleteFromFolder(checkedItems());
            deleteFromList(checkedItems());
        }

        public void UndoDeleteCheckedItems()
        {
            recoveryToFolder();
            recoveryToList();
        }

        private void deleteFromFolder(List<FileConsist> listChecked)
        {
            listChecked.ForEach(itemChecked =>
            {
                var fullPathFile = Path.Combine(itemChecked.FilePath, itemChecked.FileName);
                File.Delete(fullPathFile);
            });
        }

        private void deleteFromList(List<FileConsist> listChecked)
        {
            listChecked.ForEach(itemDeleted =>
            {
                foreach (var duplicatesView in CollectionForDuplicatesView)
                {
                    // use For loop - because of can`t change collection in Foreach
                    for (int index = 0; index < duplicatesView.FullInfoFiles.Count(); index++)
                    {
                        var currentFile = duplicatesView.FullInfoFiles[index];
                        if (itemDeleted.FilePath == currentFile.FilePath && itemDeleted.FileName == currentFile.FileName)
                        {
                            duplicatesView.FullInfoFiles.Remove(currentFile);
                        }
                    }
                }
            });
        }

        private void recoveryToFolder()
        {
            var deletedFiles = checkedItems();


        }

        private void recoveryToList()
        {
            var deletedFiles = checkedItems();

            deletedFiles.ForEach(deletedFile =>
            {

            });
        }

        private List<FileConsist> checkedItems()
        {
            return CollectionForDuplicatesView
                .SelectMany(item => item.FullInfoFiles)
                .Where(item => item.IsCheckedInView)
                .ToList();
        }
    }
}
