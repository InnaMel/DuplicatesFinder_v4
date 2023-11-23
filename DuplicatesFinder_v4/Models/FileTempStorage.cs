using DuplicatesFinder_v4.Models;

namespace DuplicatesFinderV4.Models
{
    class FileTempStorage : FileConsist
    {
        public string TempPath { get; set; }

        public FileTempStorage(FileConsist fileConsist, string tempPath)
        {
            this.FileName = fileConsist.FileName;
            this.FilePath = fileConsist.FilePath;
            this.FileExtension = fileConsist.FileExtension;
            this.FileSize = fileConsist.FileSize;
            this.DateTimeCreate = fileConsist.DateTimeCreate;
            this.DateTimeCreateString = fileConsist.DateTimeCreateString;
            this.DateTimeModified = fileConsist.DateTimeModified;
            this.DateTimeModifiedString = fileConsist.DateTimeModifiedString;
            TempPath = tempPath;
        }
    }
}
