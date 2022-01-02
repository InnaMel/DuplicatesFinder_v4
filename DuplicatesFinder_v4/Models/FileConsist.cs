using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DuplicatesFinder_v4.Models
{
    public class FileConsist
    {
        [JsonPropertyName ("File name")]
        public string FileName { get; set; }
        [JsonPropertyName("File path")]
        public string FilePath { get; set; }
        [JsonIgnore]
        public string ExtensionFile { get; set; }
        [JsonPropertyName("File size, kb")]
        public double SizeFile { get; set; }
        [JsonIgnore]
        public DateTime DateTimeCreate { get; set; }
        [JsonPropertyName("File created")]
        public string DateTimeCreateString { get; set; }

        public bool isChecked = false;

        public FileConsist() {}
        public FileConsist(FileInfo fullInfoFile)
        {
            this.FileName = fullInfoFile.Name;
            this.FilePath = fullInfoFile.DirectoryName;
            this.ExtensionFile = fullInfoFile.Extension.Remove(0, 1).ToUpper();
            this.SizeFile = Math.Round(((double)fullInfoFile.Length) / 1024, MidpointRounding.AwayFromZero);
            this.DateTimeCreate = fullInfoFile.CreationTime;
            this.DateTimeCreateString = fullInfoFile.CreationTime.ToString("g");
        }

        public override string ToString()
        {
            return $"{FileName} *** {FilePath} *** {DateTimeCreate.ToString("g")}";
        }
    }
}
