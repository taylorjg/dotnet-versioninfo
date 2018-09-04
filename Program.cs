using McMaster.Extensions.CommandLineUtils;
using System;
using System.Linq;
using System.Reflection;

namespace DotNetVersionInfo
{
    [Command(
        Name = "dotnet versioninfo",
        FullName = "dotnet-versioninfo",
        Description = "Display version information of .NET Core assemblies.")]
    class Program
    {
        [Argument(0, Description = "Glob pattern [default: **/*.dll]")]
        public string Pattern { get; }

        [Option(Description = "Show relative paths in the results")]
        public bool Relative { get; }

        [Option(Description = "Format the results as JSON")]
        public bool Json { get; }

        [Option(Description = "Display the version of this tool and then exit")]
        public bool Version { get; }

        private static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private const string DEFAULT_PATTERN = "**/*.dll";

        private int OnExecute()
        {
            try {
                if (Version) {
                    ShowToolVersion();
                }
                else {
                    var results = Domain.ProcessFiles(Pattern ?? DEFAULT_PATTERN, Relative).ToList();
                    if (Json)
                        Output.WriteJsonResults(results);
                    else
                        Output.WritePlainTextResults(results);
                }
                return 0;
            }
            catch (Exception ex) {
                Console.Error.WriteLine($"ERROR: {ex.Message}");
                return 1;
            }
        }
        private static void ShowToolVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var aiva = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            Console.WriteLine($"{aiva.InformationalVersion}");
        }
    }
}
