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
    static class Domain
    {
        public static IEnumerable<Result> ProcessFiles(string globPattern, bool showRelativePaths) 
        {
            var absoluteBaseDir = Path.GetFullPath(".");
            var pathTransformer = showRelativePaths
                ? ToRelativePath(absoluteBaseDir)
                : Identity;
            var glob = new Glob(globPattern) { ErrorLog = Console.WriteLine };
            return glob
                .Expand()
                .Cast<FileInfoBase>()
                .Select(fileInfo => fileInfo.FullName)
                .Select(pathTransformer)
                .Select(ProcessFile)
                .ToList();
        }

        private static Result ProcessFile(string fileName)
        {
            try {
                var fvi = FileVersionInfo.GetVersionInfo(fileName);
                return new SuccessResult {
                    FileName = fileName,
                    FileVersion = fvi.FileVersion,
                    ProductVersion = fvi.ProductVersion
                };
            }
            catch (Exception ex) {
                return new FailureResult {
                    FileName = fileName,
                    Error = ex.Message
                };
            }
        }

        private static Func<string, string> ToRelativePath(string absoluteBaseDir) =>
            (string path) => Path.GetRelativePath(absoluteBaseDir, path);

        private static T Identity<T>(T t) => t;
    }
}
