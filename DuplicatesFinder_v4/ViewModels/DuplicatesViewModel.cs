using DuplicatesFinder_v4.Models;
using DuplicatesFinderV4;
using DuplicatesFinderV4.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;

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
                if (pathWithAppFolder == null)
                {
                    var getCurrentNameUser = Environment.UserName;
                    pathWithAppFolder = Path.Combine($"C:\\Users\\{getCurrentNameUser}\\Downloads", "DuplicatesFinder");
                    Directory.CreateDirectory(pathWithAppFolder);
                }
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
                    eachDuplicatesForView.Ico = GetIconsBitmap(collectionFileConsist[0].FilePath, collectionFileConsist[0].FileName);
                    eachDuplicatesForView.FullInfoFiles = collectionFileConsist;

                    CollectionForDuplicatesView.Add(eachDuplicatesForView);
                }
            }
        }

        public async Task SaveToTxtAsync(Action message)
        {
            var dateCreation = DateTime.Now.ToShortDateString();
            var setFileSaveName = $"Dublicates_{dateCreation}.txt";
            var setFilePath = Path.Combine(PathWithAppFolder, setFileSaveName);
            var option = new JsonSerializerOptions { WriteIndented = true };

            if (File.Exists(setFilePath))
            {
                File.Delete(setFilePath);
            }

            var currentChecked = CheckedFiles();
            using (FileStream exportFile = new FileStream(setFilePath, FileMode.OpenOrCreate))
            {
                if (currentChecked.Count != 0)
                {
                    foreach (var item in currentChecked)
                    {
                        await JsonSerializer.SerializeAsync<FileConsist>(exportFile, item, option);
                    }
                }
                else
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

            message.Invoke();
        }

        public async Task DeleteCheckedItemsAsync()
        {
            await SaveFilesToTempFolder();
            DeleteFromList();
            await DeleteFromFolder();
        }

        public async Task UndoDeleteCheckedItemsAsync()
        {
            RecoveryToList();
            await RecoveryToFolder();
            listTempFiles = new List<FileTempStorage>();
        }

        public async Task DeleteTempFilesAsync()
        {
            listTempFiles = new List<FileTempStorage>();
            await Task.Run(() => DeleteTempFiles(pathTempFolder));
        }

        private void DeleteTempFiles(string currentPath)
        {
            if (Directory.Exists(pathTempFolder))
            {
                List<string> directories = null;
                var directoryInfo = new DirectoryInfo(currentPath);

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                directories = Directory.GetDirectories(currentPath).ToList();
                if (directories.Count != 0)
                {
                    foreach (var directory in directories)
                    {
                        currentPath = directory;
                        DeleteTempFiles(currentPath);
                    }
                }

                if (Directory.GetDirectories(pathTempFolder).Count() == 0)
                {
                    Directory.Delete(pathTempFolder);
                }
                else
                    Directory.Delete(currentPath);
            }
        }

        private Task SaveFilesToTempFolder()
        {
            var taskSaveToTempFolder = Task.Run(() =>
            {
                var checkedUserItems = CheckedFiles();
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

                            AddTolistTempFiles(checkedItem, newfullPathNestedTempFile);
                            isBreak = true;
                            break;
                        }
                    }
                    if (!isBreak)
                    {
                        var newfullPathTempFile = Path.Combine(pathTempFolder, checkedItem.FileName);
                        File.Copy(fullPathCurrentFile, newfullPathTempFile);
                        AddTolistTempFiles(checkedItem, newfullPathTempFile);
                    }
                    index++;
                });
            });
            return taskSaveToTempFolder;
        }

        private Task DeleteFromFolder()
        {
            return Task.Run(() =>
            {
                listTempFiles.ForEach(itemChecked =>
                {
                    var fullPathFile = Path.Combine(itemChecked.FilePath, itemChecked.FileName);
                    File.Delete(fullPathFile);
                });
            });
        }

        private void DeleteFromList()
        {
            CheckedFiles().ForEach(itemDeleted =>
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

        private Task RecoveryToFolder()
        {
            return Task.Run(() =>
            {
                listTempFiles.ForEach(deletedFile =>
                {
                    var currentPathFile = Path.Combine(deletedFile.FilePath, deletedFile.FileName);
                    File.Copy(deletedFile.TempPath, currentPathFile);
                });
                DeleteTempFiles(pathTempFolder);
            });
        }

        private void RecoveryToList()
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

        private List<FileConsist> CheckedFiles()
        {
            return CollectionForDuplicatesView
                .SelectMany(item => item.FullInfoFiles)
                .Where(item => item.IsCheckedInView)
                .ToList();
        }

        private void AddTolistTempFiles(FileConsist fileConsist, string tempPath)
        {
            var currentTempFile = new FileTempStorage(fileConsist, tempPath);
            listTempFiles.Add(currentTempFile);
        }

        private ImageSource GetIconsBitmap (string filePath, string fileName)
        {
            string fulPath = Path.Combine(filePath, fileName);
            Icon ico = Icon.ExtractAssociatedIcon(fulPath);

            return ImageSourceExtension.ToImageSource(ico);
            //return ico.ToBitmap();
        }

    }
}
