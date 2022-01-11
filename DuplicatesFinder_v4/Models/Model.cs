using DuplicatesFinder_v4.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
                ObservableCollection<ObservableCollection<FileConsist>> findedDuplicates = FindDuplicates();
                onCompleted(findedDuplicates);
            }
            );
        }

        public ObservableCollection<ObservableCollection<FileConsist>> FindDuplicates()
        {
            List<FileConsist> allFiles = findAllFiles();
            ObservableCollection<ObservableCollection<FileConsist>> findedDuplicates = null;

            if (allFiles != null)
            {
                findedDuplicates = new ObservableCollection<ObservableCollection<FileConsist>>();

                for (int i = 0; i < allFiles.Count; i++)
                {
                    if (allFiles[i].IsChecked == false)
                    {
                        var subsidiaryList = new List<FileConsist>();

                        for (int j = i; j < allFiles.Count; j++)
                        {
                            if (allFiles[j].IsChecked == false && allFiles[i].FileName == allFiles[j].FileName)
                            {
                                subsidiaryList.Add(allFiles[j]);
                                allFiles[j].IsChecked = true;
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

        private List<FileConsist> findAllFiles()
        {
            List<string> allFiles = findFromDirectory();
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

        private List<string> findFromDirectory()
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
                        files = files.Concat(findFromDirectory()).ToList();
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
            ObservableCollection<FileConsist> resultCollection = new ObservableCollection<FileConsist>();

            foreach (var file in listFilesAsClass)
            {
                resultCollection.Add(file);
            }

            return resultCollection;
        }
        #region old code
        /// <summary>
        /// creates Observable Collection of all Observable Collections of duplicates
        /// </summary>
        /// <returns></returns>
        //public Task<ObservableCollection<ObservableCollection<FileConsist>>> FindDuplicatesAsync()
        //{
        //    Task<ObservableCollection<ObservableCollection<FileConsist>>>  task = Task.Run<ObservableCollection<ObservableCollection<FileConsist>>>(() =>
        //    {
        //        List<FileConsist> allFiles = findAllFiles();

        //        ObservableCollection<ObservableCollection<FileConsist>> findedDuplicates = null;

        //        if (allFiles != null)
        //        {
        //            findedDuplicates = new ObservableCollection<ObservableCollection<FileConsist>>();

        //            for (int i = 0; i < allFiles.Count; i++)
        //            {
        //                if (allFiles[i].isChecked == false)
        //                {
        //                    List<FileConsist> subsidiaryList = new List<FileConsist>();
        //                    for (int j = i; j < allFiles.Count; j++)
        //                    {
        //                        if (allFiles[j].isChecked == false && allFiles[i].FileName == allFiles[j].FileName)
        //                        {
        //                            subsidiaryList.Add(allFiles[j]);
        //                            allFiles[j].isChecked = true;
        //                        }
        //                    }
        //                    if (subsidiaryList.Count > 1)
        //                    {
        //                        findedDuplicates.Add(ConvertToObservable(subsidiaryList));
        //                    }
        //                }
        //            }
        //            if (findedDuplicates.Count == 0)
        //                MessageBox.Show("No one matches");
        //        }
        //        return findedDuplicates;
        //    }
        //    );
        //    return task;
        //}
        #endregion
    }
}

