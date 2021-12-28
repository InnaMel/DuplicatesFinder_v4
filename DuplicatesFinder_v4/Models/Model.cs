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
        public string userPath;

        /// <summary>
        /// creates new list of lists of duplicates
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ObservableCollection<ObservableCollection<FileConsist>> FindDuplicates()
        {
            List<FileConsist> allFiles = findAllFiles();
            ObservableCollection<ObservableCollection<FileConsist>> findedDuplicates = null;

            if (allFiles != null)
            {
                findedDuplicates = new ObservableCollection<ObservableCollection<FileConsist>>();

                for (int i = 0; i < allFiles.Count; i++)
                {
                    if (allFiles[i].isChecked == false)
                    {
                        List<FileConsist> subsidiaryList = new List<FileConsist>();
                        for (int j = i; j < allFiles.Count; j++)
                        {
                            if (allFiles[j].isChecked == false && allFiles[i].FileName == allFiles[j].FileName)
                            {
                                subsidiaryList.Add(allFiles[j]);
                                allFiles[j].isChecked = true;
                            }
                        }
                        if (subsidiaryList.Count > 1)
                        {
                            findedDuplicates.Add(ConvertToObservable(subsidiaryList));
                        }
                    }
                }
                if (findedDuplicates.Count == 0)
                    MessageBox.Show("No one matches");
            }
            return findedDuplicates;
        }

        List<FileConsist> findAllFiles()
        {
            List<string> allFiles = findFromDirectory();
            List<FileConsist> listSplitedFiles = null;

            if (allFiles != null)
            {
                listSplitedFiles = new List<FileConsist>();

                foreach (var item in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(item);
                    FileConsist newSplitFiles = new FileConsist(fileInfo);
                    listSplitedFiles.Add(newSplitFiles);
                }
            }
            return listSplitedFiles;
        }



        List<string> findFromDirectory()
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
                    foreach (string item in direct)
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

        // *** Convert list To Observable 
        ObservableCollection<FileConsist> ConvertToObservable(List<FileConsist> listFilesAsClass)
        {
            ObservableCollection<FileConsist> result = new ObservableCollection<FileConsist>();
            foreach (FileConsist i in listFilesAsClass)
            {
                result.Add(i);
            }

            return result;
        }
    }
}

