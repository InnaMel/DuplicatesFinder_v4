﻿using DuplicatesFinder_v4.Models;
using DuplicatesFinderV4.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DuplicatesFinder_v4.ViewModels
{
    public class DuplicatesViewModel
    {
        const string TEMPORARY_FOLDER_NAME = "TempDF";
        private static string pathWithAppFolder;
        private List<FileTempStorage> listTempFiles;
        private string pathTempFolder;
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
            pathTempFolder = Path.Combine(PathWithAppFolder, TEMPORARY_FOLDER_NAME);
            CollectionForDuplicatesView = new ObservableCollection<ListForViewDuplicates>();
            listTempFiles = new List<FileTempStorage>();
        }

        public void Divide(ObservableCollection<ObservableCollection<FileConsist>> wholeCollectionFileConsist)
        {
            if (wholeCollectionFileConsist != null)
            {
                foreach (var collectionFileConsist in wholeCollectionFileConsist)
                {
                    var eachDuplicatesForView = new ListForViewDuplicates();

                    eachDuplicatesForView.NameDuplicates = collectionFileConsist[0].FileName.ToUpper();

                    eachDuplicatesForView.FullInfoFiles = collectionFileConsist;

                    CollectionForDuplicatesView.Add(eachDuplicatesForView);
                }
            }
        }

        public async void SaveToTxtAsync()
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

        public async void DeleteCheckedItemsAsync()
        {
            await saveFilesToTempFolder();
            await deleteFromFolder();
            deleteFromList();
        }

        public async void UndoDeleteCheckedItemsAsync()
        {
            recoveryToList();
            await recoveryToFolder();
        }

        public Task DeleteTempFiles()
        {
            if (Directory.Exists(pathTempFolder))
            {
                var sourcePathTempFolder = pathTempFolder;
                Task task = Task.Run(() =>
                {
                    List<string> directories = null;
                    var directoryInfo = new DirectoryInfo(pathTempFolder);

                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        file.Delete();
                    }

                    directories = Directory.GetDirectories(pathTempFolder).ToList();
                    if (directories != null)
                    {
                        foreach (var directory in directories)
                        {
                            pathTempFolder = directory;
                            DeleteTempFiles();
                        }
                    }
                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    Directory.Delete(sourcePathTempFolder);
                });
                return task;
            }
            return Task.CompletedTask;
        }

        private Task saveFilesToTempFolder()
        {
            Task taskSaveToTempFolder = Task.Run(() =>
            {
                var checkedUserItems = checkedFiles();
                var index = 1;
                var isBreak = false;
                if (!Directory.Exists(pathTempFolder))
                {
                    Directory.CreateDirectory(pathTempFolder);
                }

                checkedUserItems.ForEach(checkedItem =>
                {
                    isBreak = false;
                    var fullPathCurrentFile = Path.Combine(checkedItem.FilePath, checkedItem.FileName);
                    for (int i = index; i < checkedUserItems.Count; i++)
                    {
                        if (checkedItem == checkedUserItems[i])
                        {
                            var pathNestedTempFolder = Path.Combine(pathTempFolder, $"temp{checkedItem.GetHashCode()}");
                            if (!Directory.Exists(pathNestedTempFolder))
                            {
                                Directory.CreateDirectory(pathNestedTempFolder);
                            }
                            var newfullPathNestedTempFile = Path.Combine(pathNestedTempFolder, checkedItem.FileName);
                            File.Copy(fullPathCurrentFile, newfullPathNestedTempFile);

                            addTolistTempFiles(checkedItem, newfullPathNestedTempFile);
                            isBreak = true;
                            break;
                        }
                    }
                    if (!isBreak)
                    {
                        var newfullPathTempFile = Path.Combine(pathTempFolder, checkedItem.FileName);
                        File.Copy(fullPathCurrentFile, newfullPathTempFile);
                        addTolistTempFiles(checkedItem, newfullPathTempFile);
                    }
                    index++;
                });
            });
            return taskSaveToTempFolder;
        }

        private Task deleteFromFolder()
        {
            return Task.Run(() =>
            {
                checkedFiles().ForEach(itemChecked =>
                {
                    var fullPathFile = Path.Combine(itemChecked.FilePath, itemChecked.FileName);
                    File.Delete(fullPathFile);
                });
            });
        }

        private void deleteFromList()
        {
            checkedFiles().ForEach(itemDeleted =>
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

        private Task recoveryToFolder()
        {
            return Task.Run(() =>
            {
                listTempFiles.ForEach(deletedFile =>
                {
                    var currentPathFile = Path.Combine(deletedFile.FilePath, deletedFile.FileName);
                    File.Copy(deletedFile.TempPath, currentPathFile);
                });
            });
        }

        private void recoveryToList()
        {
            listTempFiles.ForEach(checkedFile =>
            {
                var isMatch = false;
                for (int i = 0; i < CollectionForDuplicatesView.Count; i++)
                {
                    var currentFile = CollectionForDuplicatesView[i];
                    if (checkedFile.FileName.ToUpper() == currentFile.NameDuplicates)
                    {
                        currentFile.FullInfoFiles.Add(checkedFile);
                        isMatch = true;
                        break;
                    }
                }

                if (!isMatch)
                {
                    var fileInfo = new ObservableCollection<FileConsist>();
                    fileInfo.Add(new FileConsist()
                    {
                        FileName = checkedFile.FileName,
                        FilePath = checkedFile.FilePath,
                        FileExtension = checkedFile.FileExtension,
                        FileSize = checkedFile.FileSize,
                        DateTimeCreate = checkedFile.DateTimeCreate,
                        DateTimeModified = checkedFile.DateTimeModified,
                        IsCheckedInView = checkedFile.IsCheckedInView
                    });
                    var newListForViewDuplicates = new ListForViewDuplicates() { FullInfoFiles = fileInfo, NameDuplicates = checkedFile.FileName };
                    CollectionForDuplicatesView.Add(newListForViewDuplicates);
                }
            });
        }

        private List<FileConsist> checkedFiles()
        {
            return CollectionForDuplicatesView
                .SelectMany(item => item.FullInfoFiles)
                .Where(item => item.IsCheckedInView)
                .ToList();
        }

        private void addTolistTempFiles(FileConsist fileConsist, string tempPath)
        {
            var currentTempFile = new FileTempStorage(fileConsist, tempPath);
            listTempFiles.Add(currentTempFile);
        }

    }
}
