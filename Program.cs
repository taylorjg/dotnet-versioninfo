using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using Glob;

namespace dotnet_versioninfo
{
    [Command(
        Name = "dotnet versioninfo",
        FullName = "dotnet-versioninfo",
        Description = "Display version information of .NET Core assemblies.")]
    class Program
    {
        private const string DEFAULT_BASE_DIR = ".";
        private const string DEFAULT_PATTERN = "**/*.dll";

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Base directory to which the glob pattern is applied [current directory]")]
        [DirectoryExists]
        public string BaseDir { get; }

        [Option(Description = "Glob pattern [**/*.dll]")]
        public string Pattern { get; }

        [Option(Description = "Show relative paths in the results [false]")]
        public bool Relative { get; }

        [Option(Description = "Display the version of this tool and then exit")]
        public bool Version { get; }

        private void ProcessFile(string fileName)
        {
            Console.WriteLine($"{fileName}");
            var fvi = FileVersionInfo.GetVersionInfo(fileName);
            Console.WriteLine($"\tFileVersion:\t{fvi.FileVersion}");
            Console.WriteLine($"\tProductVersion:\t{fvi.ProductVersion}");
        }

        private Func<string, string> ToRelativePath(string relativeBaseDir, string absoluteBaseDir) =>
            (string path) => Path.Combine(relativeBaseDir, Path.GetRelativePath(absoluteBaseDir, path));

        private T Identity<T>(T t) => t;

        private static void DisplayVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var aiva = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            Console.WriteLine($"{aiva.InformationalVersion}");
        }

        private static IEnumerable<FileInfo> GlobFilesWorkaround(DirectoryInfo di, string pattern) {
            var glob = new Glob.Glob(pattern, GlobOptions.Compiled);
            return di.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                .Where(directory => glob.IsMatch(directory.FullName));
        }

        private int OnExecute()
        {
            if (Version) {
                DisplayVersion();
                return 0;
            }

            var actualBaseDir = BaseDir ?? DEFAULT_BASE_DIR;
            var actualPattern = Pattern ?? DEFAULT_PATTERN;

            var absoluteBaseDir = Path.GetFullPath(actualBaseDir);
            var pathTransformer = Relative
                ? ToRelativePath(actualBaseDir, absoluteBaseDir)
                : Identity;
            var directoryInfo = new DirectoryInfo(absoluteBaseDir);
            var fileNames = actualPattern.Contains("**")
                ? directoryInfo.GlobFiles(actualPattern)
                    .Select(fileInfo => fileInfo.FullName)
                    .Select(pathTransformer)
                    .ToList()
                : GlobFilesWorkaround(directoryInfo, actualPattern)
                    .Select(fileInfo => fileInfo.FullName)
                    .Select(pathTransformer)
                    .ToList();
            fileNames.ForEach(ProcessFile);
            return 0;
        }
    }
}
