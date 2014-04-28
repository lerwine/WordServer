using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WordNetImporter.Src
{
    public class DirectoryRetriever : StatusReportingObject<DirectoryRetriever.DirectoryRetrieverWorker, FileInfo[]>
    {
        public string DataFolder { get; set; }
        public bool Recursive { get; set; }
        public string MatchPattern { get; set; }
        public RegexOptions MatchOptions { get; set; }

        public class DirectoryRetrieverWorker : StatusReportingObject<DirectoryRetrieverWorker, FileInfo[]>.Worker
        {
            private DirectoryInfo _dataFolder;
            private bool _recursive;
            private Regex _matchRegex;

            public DirectoryRetrieverWorker(string dataFolder, bool recursive, string matchPattern, RegexOptions matchOptions)
            {
                this._dataFolder = new DirectoryInfo(dataFolder);
                this._recursive = recursive;
                this._matchRegex = (String.IsNullOrEmpty(matchPattern)) ? null : new Regex(matchPattern, matchOptions);
            }

            public FileInfo[] GetResults()
            {
                if (!this._dataFolder.Exists)
                    throw new DirectoryNotFoundException("Directory does not exist");

                return this.ProcessFolder(this._dataFolder).ToArray();
            }

            private IEnumerable<FileInfo> ProcessFolder(DirectoryInfo directoryInfo)
            {
                this.RaiseStatusUpdate(StatusCode.Progress, "Processing Folder", directoryInfo.FullName);
                IEnumerable<FileInfo> result = null;

                try
                {
                    FileInfo[] files = directoryInfo.GetFiles();
                    this.RaiseStatusUpdate(StatusCode.Progress, String.Format("Found {0} files", files.Length), directoryInfo.FullName);

                    if (this._recursive)
                        result = directoryInfo.GetDirectories().SelectMany(d => this.ProcessFolder(directoryInfo)).Concat(files);
                    else
                        result = files;
                }
                catch (Exception error)
                {
                    this.RaiseStatusUpdate(error, "Error getting contents of folder", directoryInfo.FullName);
                    if (result == null)
                        result = new FileInfo[0];
                }

                return result;
            }
        }

        protected override DirectoryRetriever.DirectoryRetrieverWorker CreateWorker()
        {
            return new DirectoryRetrieverWorker(this.DataFolder, this.Recursive, this.MatchPattern, this.MatchOptions);
        }
    }
}
