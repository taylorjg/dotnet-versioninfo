using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using Ganss.IO;
using Newtonsoft.Json;

namespace dotnet_versioninfo
{
    abstract class Result
    {
        [JsonProperty(Order = 1)]
        public string FileName { get; set; }
    }

    class SuccessResult : Result
    {
        [JsonProperty(Order = 2)]
        public string FileVersion { get; set; }
        [JsonProperty(Order = 3)]
        public string ProductVersion { get; set; }
    }

    class FailureResult : Result
    {
        [JsonProperty(Order = 2)]
        public string Error { get; set; }
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

        private Func<string, string> ToRelativePath(string absoluteBaseDir) =>
            (string path) => Path.GetRelativePath(absoluteBaseDir, path);

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
                switch (result) {
                    case SuccessResult success:
                        Console.WriteLine($"\tFileVersionInfo.FileVersion:\t{success.FileVersion}");
                        Console.WriteLine($"\tFileVersionInfo.ProductVersion:\t{success.ProductVersion}");
                        break;
                    case FailureResult failure:
                        Console.WriteLine($"\tError: {failure.Error}");
                        break;
                }
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
                ? ToRelativePath(absoluteBaseDir)
                : Identity;
            var glob = new Glob(pattern) { ErrorLog = Console.WriteLine };
            var results = glob
                .Expand()
                .Cast<FileInfoBase>()
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
