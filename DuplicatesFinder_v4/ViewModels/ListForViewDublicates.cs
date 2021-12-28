using DuplicatesFinder_v4.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicatesFinder_v4.ViewModels
{

    public class ListForViewDuplicates
    {
        public string NameDuplicates { get; set; }
        //public ObservableCollection<string> Paths { get; set; }
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
