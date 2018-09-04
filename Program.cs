using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using Ganss.IO;
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
        private const string DEFAULT_PATTERN = "**/*.dll";

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Argument(0, Description = "Glob pattern [default: **/*.dll]")]
        public string Pattern { get; }

        [Option(Description = "Show relative paths in the results")]
        public bool Relative { get; }

        [Option(Description = "Format the results as JSON")]
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

            var pattern = Pattern ?? DEFAULT_PATTERN;
            var absoluteBaseDir = Path.GetFullPath(".");
            var pathTransformer = Relative
                ? ToRelativePath(".", absoluteBaseDir)
                : Identity;
            var results = Glob.Expand(pattern)
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
