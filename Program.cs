using System;
using System.Diagnostics;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using Glob;

namespace dotnet_versioninfo
{
    [Command(Description = "Display version information of .NET Core assemblies")]
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void ProcessFile(string fileName)
        {
            var fvi = FileVersionInfo.GetVersionInfo(fileName);
            Console.WriteLine($"{fileName}");
            Console.WriteLine($"\tFileVersion:\t{fvi.FileVersion}");
            Console.WriteLine($"\tProductVersion:\t{fvi.ProductVersion}");
        }

        private Func<string, string> ToRelativePath(string relativeTo) =>
            (string path) => Path.GetRelativePath(relativeTo, path);

        private T Identity<T>(T t) => t;

        private int OnExecute()
        {
            var baseDir = ".";
            var pattern = "**/*.dll";
            var absoluteBaseDir = Path.GetFullPath(baseDir);
            var useRelativePaths = true;
            var pathTransformer = useRelativePaths
                ? ToRelativePath(absoluteBaseDir)
                : Identity;
            var directoryInfo = new DirectoryInfo(absoluteBaseDir);
            var fileNames = directoryInfo.GlobFiles(pattern)
                .Select(fileInfo => fileInfo.FullName)
                .Select(pathTransformer)
                .ToList();
            fileNames.ForEach(ProcessFile);
            return 0;
        }
    }
}
