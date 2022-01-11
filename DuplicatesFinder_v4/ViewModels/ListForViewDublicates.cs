using DuplicatesFinder_v4.Models;
using System.Collections.ObjectModel;

namespace DuplicatesFinder_v4.ViewModels
{

    public class ListForViewDuplicates
    {
        public string NameDuplicates { get; set; }
        public ObservableCollection<FileConsist> FullInfoFiles { get; set; }

        public ListForViewDuplicates()
        {
            FullInfoFiles = new ObservableCollection<FileConsist>();
        }

        public override string ToString()
        {
            return $"{NameDuplicates} {FullInfoFiles.Count}";
        }
    }
}
