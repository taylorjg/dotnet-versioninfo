using Ganss.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace DotNetVersionInfo
{
    public class Domain
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFileVersionInfo _fileVersionInfo;

        public Domain(IFileSystem fileSystem, IFileVersionInfo fileVersionInfo)
        {
            _fileSystem = fileSystem;
            _fileVersionInfo = fileVersionInfo;
        }

        public IEnumerable<Result> ProcessFiles(
            string globPattern,
            bool showRelativePaths) 
        {
            var absoluteBaseDir = Path.GetFullPath(".");
            var pathTransformer = showRelativePaths
                ? ToRelativePath(absoluteBaseDir)
                : Identity;
            var glob = new Glob(globPattern, _fileSystem) { ErrorLog = Console.WriteLine };
            return glob
                .Expand()
                .Cast<FileInfoBase>()
                .Select(fileInfo => fileInfo.FullName)
                .Select(pathTransformer)
                .Select(ProcessFile)
                .ToList();
        }

        private Result ProcessFile(string fileName)
        {
            try {
                var fvi = _fileVersionInfo.GetVersionInfo(fileName);
                return new SuccessResult {
                    FileName = fileName,
                    FileVersion = fvi.FileVersion,
                    ProductVersion = fvi.ProductVersion
                };
            }
            catch (Exception ex) {
                return new FailureResult {
                    FileName = fileName,
                    Error = $"{ex.GetType().Name}: {ex.Message}"
                };
            }
        }

        private Func<string, string> ToRelativePath(string absoluteBaseDir) =>
            (string path) => Path.GetRelativePath(absoluteBaseDir, path);

        private T Identity<T>(T t) => t;
    }
}
