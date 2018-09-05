using System;
using Newtonsoft.Json;

namespace DotNetVersionInfo
{
    public abstract class Result
    {
        [JsonProperty(Order = 1)]
        public string FileName { get; set; }
    }

    public class SuccessResult : Result
    {
        [JsonProperty(Order = 2)]
        public string FileVersion { get; set; }
        [JsonProperty(Order = 3)]
        public string ProductVersion { get; set; }
    }

    public class FailureResult : Result
    {
        [JsonProperty(Order = 2)]
        public string Error { get; set; }
    }
}
