using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicatesFinder_v4.Models
{
    public class FileConsist
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ExtensionFile { get; set; }
        public double SizeFile { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public string DateTimeCreateString { get; set; }

        public bool isChecked = false;

        public FileConsist() {}
        public FileConsist(FileInfo fullInfoFile)
        {
            this.FileName = fullInfoFile.Name;
            this.FilePath = fullInfoFile.DirectoryName;
            this.ExtensionFile = fullInfoFile.Extension;
            this.SizeFile = (int)fullInfoFile.Length;
            this.DateTimeCreate = fullInfoFile.CreationTime;
        }

        public override string ToString()
        {
            return $"{FileName} {FilePath}";
        }
    }
}
