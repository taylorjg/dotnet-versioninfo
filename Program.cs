using System;
using System.Diagnostics;
using System.Reflection;
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
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Display the version of this tool and then exit")]
        public bool Version { get; }

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

        private static void DisplayVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var aiva = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            Console.WriteLine($"{aiva.InformationalVersion}");
        }

        private int OnExecute()
        {
            if (Version) {
                DisplayVersion();
                return 0;
            }

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
