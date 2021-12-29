using DuplicatesFinder_v4.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicatesFinder_v4.ViewModels
{
    public class DuplicatesViewModel
    {
        public ObservableCollection<ListForViewDuplicates> CollectionForDuplicatesView { get; set; }
        public DuplicatesViewModel()
        {
            CollectionForDuplicatesView = new ObservableCollection<ListForViewDuplicates>();
        }
        public void Divide(ObservableCollection<ObservableCollection<FileConsist>> wholeCollectionFileConsist)
        {
            if (wholeCollectionFileConsist != null)
            {
                CollectionForDuplicatesView.Clear();
                foreach (var collectionFileConsist in wholeCollectionFileConsist)
                {
                    ListForViewDuplicates eachDuplicatesForView = new ListForViewDuplicates();
                    eachDuplicatesForView.NameDuplicates = collectionFileConsist[0].FileName;

                    foreach (var fileConsist in collectionFileConsist)
                    {
                        FileConsist fileConsistMedium = new FileConsist() { 
                            FilePath = fileConsist.FilePath, 
                            DateTimeCreateString = (fileConsist.DateTimeCreate).ToString("g"), 
                            SizeFile = Math.Round((fileConsist.SizeFile / 1000000), 3), 
                            ExtensionFile = (fileConsist.ExtensionFile).Remove(0,1).ToUpper() };
                        eachDuplicatesForView.FullInfoFiles.Add(fileConsistMedium);
                    }
                    CollectionForDuplicatesView.Add(eachDuplicatesForView);
                }
            }
        }
    }
}
