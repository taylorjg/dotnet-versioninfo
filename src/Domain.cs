using Ganss.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;

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
            var glob = new Glob(
                globPattern,
                new GlobOptions { ErrorLog = HandeGlobError },
                _fileSystem
            );
            return glob
                .Expand()
                .Cast<FileInfoBase>()
                .Select(fileInfo => fileInfo.FullName)
                .Select(fullName => (fullName, transformedFileName: pathTransformer(fullName)))
                .Select(ProcessFile)
                .ToList();
        }

        private Result ProcessFile((string fullName, string transformedFileName) tuple)
        {
            try {
                var fvi = _fileVersionInfo.GetVersionInfo(tuple.fullName);
                return new SuccessResult {
                    FileName = tuple.transformedFileName,
                    FileVersion = fvi.FileVersion,
                    ProductVersion = fvi.ProductVersion
                };
            }
            catch (Exception ex) {
                return new FailureResult {
                    FileName = tuple.transformedFileName,
                    Error = $"{ex.GetType().Name}: {ex.Message}"
                };
            }
        }

        private Func<string, string> ToRelativePath(string absoluteBaseDir) =>
            (string path) => Path.GetRelativePath(absoluteBaseDir, path);

        private T Identity<T>(T t) => t;

        private void HandeGlobError(string error)
        {
            // Issue #1: Glob.cs effectively includes ex.ToString() in `error` e.g.
            //   Log("Error finding file system entries in {0}: {1}.", parentDir, ex);
            // Unfortunately, this includes the stack trace which makes the error messages
            // look pretty ugly (like unhandled exceptions). So let's try to clean it up a bit.
            var regex1 = new Regex(@"[\s]+--->[\s]+"); // start of InnerException
            var regex2 = new Regex(Environment.NewLine + @"[\s]+at[\s]+"); // start of StackTrace
            var match1 = regex1.Match(error);
            var match2 = regex2.Match(error);
            if (match1.Success || match2.Success) {
                var index = match1.Success ? match1.Index : match2.Index;
                var cleanedError = error.Substring(0, index);
                Console.Error.WriteLine(cleanedError);
            }
            else {
                Console.Error.WriteLine(error);
            }
        }
    }
}
