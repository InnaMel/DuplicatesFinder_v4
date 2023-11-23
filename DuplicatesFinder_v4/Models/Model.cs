using DuplicatesFinder_v4.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DuplicatesFinder_v4.Models
{
    public class Model
    {
        private bool isMatch = false;
        private string userPath;

        public string UserPath
        {
            get { return userPath; }
            set { userPath = value; }
        }
        public bool? Pics { private get; set; }
        public bool? Docs { private get; set; }
        public bool? Videos { private get; set; }

        public void BeginFindDuplicates(Action<ObservableCollection<ObservableCollection<FileConsist>>> onCompleted)
        {
            Task.Run(() =>
            {
                var foundDuplicates = FindDuplicates();
                onCompleted(foundDuplicates);
            }
            );
        }

        public ObservableCollection<ObservableCollection<FileConsist>> FindDuplicates()
        {
            var allFiles = FindAllFiles();
            ObservableCollection<ObservableCollection<FileConsist>> findedDuplicates = null;

            if (allFiles != null)
            {
                findedDuplicates = new ObservableCollection<ObservableCollection<FileConsist>>();

                for (int i = 0; i < allFiles.Count; i++)
                {
                    if (allFiles[i].IsCheckedAsDuplicate == false)
                    {
                        var subsidiaryList = new List<FileConsist>();

                        for (int j = i; j < allFiles.Count; j++)
                        {
                            if (allFiles[j].IsCheckedAsDuplicate == false && allFiles[i].FileName.ToLower() == allFiles[j].FileName.ToLower())
                            {
                                subsidiaryList.Add(allFiles[j]);
                                allFiles[j].IsCheckedAsDuplicate = true;
                            }
                        }
                        if (subsidiaryList.Count > 1)
                        {
                            findedDuplicates.Add(ConvertToObservable(subsidiaryList));
                        }
                    }
                }
            }

            return findedDuplicates;
        }

        private List<FileConsist> FindAllFiles()
        {
            var allFiles = FindAllFilesFromDirectory();
            List<FileConsist> listSplitedFiles = null;

            if (allFiles != null)
            {
                listSplitedFiles = new List<FileConsist>();

                foreach (var item in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(item);

                    if (Docs == true) CheckMatchExtension(fileInfo.Extension, typeof(ExtensionsDoc));
                    if (Pics == true) CheckMatchExtension(fileInfo.Extension, typeof(ExtensionsPic));
                    if (Videos == true) CheckMatchExtension(fileInfo.Extension, typeof(ExtensionsVideo));

                    if (isMatch)
                    {
                        FileConsist newSplitedFile = new FileConsist(fileInfo);

                        listSplitedFiles.Add(newSplitedFile);

                        isMatch = false;
                    }
                }
            }
            return listSplitedFiles;
        }

        private void CheckMatchExtension(string newSplitedFile, Type enumExt)
        {
            if (isMatch)
            {
                return;
            }

            foreach (var valueEnum in Enum.GetNames(enumExt))
            {
                if (valueEnum == newSplitedFile.TrimStart('.'))
                {
                    isMatch = true;
                    break;
                }
            };
        }

        private List<string> FindAllFilesFromDirectory()
        {
            List<string> direct = null;
            List<string> files = null;

            if (Directory.Exists(userPath))
            {
                // All files first
                files = Directory.GetFiles(userPath).ToList();

                // All directories
                direct = Directory.GetDirectories(userPath).ToList();

                if (direct != null)
                {
                    foreach (var item in direct)
                    {
                        userPath = item;
                        files = files.Concat(FindAllFilesFromDirectory()).ToList();
                    }
                }
            }
            else
            {
                MessageBox.Show("Wrong path!");
            }

            return files;
        }

        // Convert list To Observable 
        private ObservableCollection<FileConsist> ConvertToObservable(List<FileConsist> listFilesAsClass)
        {
            var resultCollection = new ObservableCollection<FileConsist>();
            listFilesAsClass.ForEach(file => resultCollection.Add(file));

            return resultCollection;
        }
    }
}

