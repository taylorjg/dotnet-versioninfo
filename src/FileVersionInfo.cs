using System;
using System.Diagnostics;

namespace DotNetVersionInfo
{
    public interface IFileVersionInfo
    {
        FileVersionInfo GetVersionInfo(string fileName);
    }

    public class FileVersionInfoImpl : IFileVersionInfo
    {
        public FileVersionInfo GetVersionInfo(string fileName)
        {
            return FileVersionInfo.GetVersionInfo(fileName);
        }
    }
}
