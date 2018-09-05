using DotNetVersionInfo;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tests
{
    using MockFileVersionInfoDict = Dictionary<string, GetVersionInfoFunc>;

    public delegate (string fileVersion, string productVersion) GetVersionInfoFunc(string fileName);

    class MockFileVersionInfo : IFileVersionInfo
    {
        MockFileVersionInfoDict _mockData;

        public MockFileVersionInfo()
        {
            _mockData = new MockFileVersionInfoDict();
        }

        public MockFileVersionInfo(MockFileVersionInfoDict mockData)
        {
            _mockData = mockData;
        }

        public FileVersionInfo GetVersionInfo(string fileName)
        {
            if (_mockData.TryGetValue(fileName, out GetVersionInfoFunc getVersionInfoFunc)) {
                var tuple = getVersionInfoFunc(fileName);
                var type = typeof(FileVersionInfo);
                var instance = (FileVersionInfo)FormatterServices.GetUninitializedObject(type);
                ReflectionHelper.SetPropertyBackingField(type, instance, "FileVersion", tuple.fileVersion);
                ReflectionHelper.SetPropertyBackingField(type, instance, "ProductVersion", tuple.productVersion);
                return instance;
            }
            return null;
        }
    }
}
