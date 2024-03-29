﻿using System;
using System.Drawing;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace DuplicatesFinder_v4.Models
{
    public class FileConsist
    {
        [JsonPropertyName("FILE NAME")]
        public string FileName { get; set; }

        [JsonPropertyName("File path")]
        public string FilePath { get; set; }

        [JsonIgnore]
        public string FileExtension { get; set; }

        [JsonPropertyName("File size, kb")]
        public double FileSize { get; set; }

        [JsonIgnore]
        public DateTime DateTimeCreate { get; set; }

        [JsonPropertyName("File created")]
        public string DateTimeCreateString { get; set; }

        [JsonIgnore]
        public DateTime DateTimeModified { get; set; }

        [JsonPropertyName("Last time modified")]
        public string DateTimeModifiedString { get; set; }

        public bool IsCheckedAsDuplicate = false;

        // for Binding to IsChecked in CheckBox
        [JsonIgnore]
        public bool IsCheckedInView { get; set; }
        [JsonIgnore]
        public Bitmap Ico { get; set; }

        public FileConsist() { }
        public FileConsist(FileInfo fullInfoFile)
        {
            this.FileName = fullInfoFile.Name;
            this.FilePath = fullInfoFile.DirectoryName;
            this.FileExtension = fullInfoFile.Extension.Remove(0, 1).ToUpper();
            this.FileSize = Math.Round(Convert.ToDouble((fullInfoFile.Length) / 1024), MidpointRounding.AwayFromZero);
            this.DateTimeCreate = fullInfoFile.CreationTime;
            this.DateTimeCreateString = fullInfoFile.CreationTime.ToString("g");
            this.DateTimeModified = File.GetLastWriteTime(fullInfoFile.FullName);
            this.DateTimeModifiedString = DateTimeModified.ToString("g");
            Ico = GetIconsBitmap(fullInfoFile.FullName); 
        }

        public override string ToString()
        {
            return $"{FileName} *** {FilePath} *** {DateTimeCreate.ToString("g")}";
        }

        public static bool operator ==(FileConsist compareFile0, FileConsist compareFile1)
        {
            if (compareFile0.FileName.ToLower() == compareFile1.FileName.ToLower() && compareFile0.FileExtension == compareFile1.FileExtension)
            {
                return true;
            }
            else return false;
        }
        public static bool operator !=(FileConsist compareFile0, FileConsist compareFile1)
        {
            if (compareFile0.FileExtension != compareFile1.FileExtension)
            {
                return true;
            }
            else return false;
        }

        private Bitmap GetIconsBitmap(string filePath)
        {
            Icon ico = Icon.ExtractAssociatedIcon(filePath);

            return ico.ToBitmap();
        }
    }
}
