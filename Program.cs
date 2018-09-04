using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using Glob;
using Newtonsoft.Json;

namespace dotnet_versioninfo
{
    class Result
    {
        public string FileName { get; set; }
        public string FileVersion { get; set; }
        public string ProductVersion { get; set; }
    }

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

        [Option(Description = "Format the results as JSON [false]")]
        public bool Json { get; }

        [Option(Description = "Display the version of this tool and then exit")]
        public bool Version { get; }

        private Result ProcessFile(string fileName)
        {
            var fvi = FileVersionInfo.GetVersionInfo(fileName);
            return new Result {
                FileName = fileName,
                FileVersion = fvi.FileVersion,
                ProductVersion = fvi.ProductVersion
            };
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
                .Where(fileInfo => glob.IsMatch(fileInfo.FullName));
        }

        private static void WritePlainTextResults(List<Result> results)
        {
            results.ForEach(result => {
                Console.WriteLine($"{result.FileName}");
                Console.WriteLine($"\tFileVersionInfo.FileVersion:\t{result.FileVersion}");
                Console.WriteLine($"\tFileVersionInfo.ProductVersion:\t{result.ProductVersion}");
            });
        }

        private static void WriteJsonResults(List<Result> results)
        {
            var jsonString = JsonConvert.SerializeObject(results, Formatting.Indented);
            Console.WriteLine(jsonString);
        }

        private int OnExecute()
        {
            if (Version) {
                DisplayVersion();
                return 0;
            }

            var baseDir = BaseDir ?? DEFAULT_BASE_DIR;
            var pattern = Pattern ?? DEFAULT_PATTERN;

            var absoluteBaseDir = Path.GetFullPath(baseDir);
            var pathTransformer = Relative
                ? ToRelativePath(baseDir, absoluteBaseDir)
                : Identity;
            var directoryInfo = new DirectoryInfo(absoluteBaseDir);
            var fileInfos = pattern.Contains("**")
                ? directoryInfo.GlobFiles(pattern)
                : GlobFilesWorkaround(directoryInfo, pattern);
            var results = fileInfos
                .Select(fileInfo => fileInfo.FullName)
                .Select(pathTransformer)
                .Select(ProcessFile)
                .ToList();

            if (Json)
                WriteJsonResults(results);
            else
                WritePlainTextResults(results);

            return 0;
        }
    }
}
