using System;
using Newtonsoft.Json;

namespace DotNetVersionInfo
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
}
