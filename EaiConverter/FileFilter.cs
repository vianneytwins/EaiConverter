namespace EaiConverter
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;

    using EaiConverter.Processor;

    public class FileFilter : IFileFilter
    {
        private readonly HashSet<string> authorizedFiles = new HashSet<string>();

        private bool isFilterActive;

        public FileFilter(string initFilePath)
        {
            this.isFilterActive = false;
            if (!string.IsNullOrEmpty(initFilePath))
            {
                string line;

                var file = new StreamReader(initFilePath);
                while ((line = file.ReadLine()) != null)
                {
                    this.authorizedFiles.Add(line.Replace("\\", @"/"));
                }

                file.Close();
                this.isFilterActive = true;
            }
        }

        public bool IsFileAuthorized(string filePath)
        {
            if (this.isFilterActive)
            {
                return this.authorizedFiles.Contains(filePath.Replace("\\", @"/"));
            }

            return true;
        }
    }
}
