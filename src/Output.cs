using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DotNetVersionInfo
{
    static class Output
    {
        public static void WritePlainTextResults(List<Result> results)
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

        public static void WriteJsonResults(List<Result> results)
        {
            var jsonString = JsonConvert.SerializeObject(results, Formatting.Indented);
            Console.WriteLine(jsonString);
        }
    }
}
