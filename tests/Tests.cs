using DotNetVersionInfo;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace Tests
{
    using MockFileSystemDict = Dictionary<string, MockFileData>;
    using MockFileVersionInfoDict = Dictionary<string, GetVersionInfoFunc>;

    public class Tests
    {
        private static ResultComparer ResultComparerInstance = new ResultComparer();

        [Fact]
        public void NoMatch()
        {
            var fileSystem = new MockFileSystem();
            var fileVersionInfo = new MockFileVersionInfo();
            var domain = new Domain(fileSystem, fileVersionInfo);

            var actual = domain.ProcessFiles("anything", showRelativePaths: false);
            
            Assert.Empty(actual);
        }

        [Fact]
        public void TopLevelFileSuccessResult()
        {
            const string FILENAME = "/file.dll";
            const string FILE_VERSION = "0.0.1.0";
            const string PRODUCT_VERSION = "0.0.1";

            var fileSystem = new MockFileSystem(new MockFileSystemDict
            {
                {FILENAME, new MockFileData(new byte[]{})}
            });
            var fileVersionInfo = new MockFileVersionInfo(new MockFileVersionInfoDict
            {
                {FILENAME, _ => (FILE_VERSION, PRODUCT_VERSION)}
            });
            var domain = new Domain(fileSystem, fileVersionInfo);
            var expected = new []
            {
                new SuccessResult {
                    FileName = FILENAME,
                    FileVersion = FILE_VERSION,
                    ProductVersion = PRODUCT_VERSION
                }
            };

            var actual = domain.ProcessFiles(FILENAME, showRelativePaths: false);

            Assert.Equal(expected, actual, ResultComparerInstance);
        }

        [Fact]
        public void TopLevelFileFailureResult()
        {
            const string FILENAME = "/file.dll";

            var fileSystem = new MockFileSystem(new MockFileSystemDict
            {
                {FILENAME, new MockFileData(new byte[]{})}
            });
            var fileVersionInfo = new MockFileVersionInfo(new MockFileVersionInfoDict
            {
                {FILENAME, _ => throw new System.UnauthorizedAccessException()}
            });
            var domain = new Domain(fileSystem, fileVersionInfo);

            var actual = domain.ProcessFiles(FILENAME, showRelativePaths: false);

            Assert.Equal(1, actual.Count());
            var firstResult = actual.First();
            Assert.IsType<FailureResult>(firstResult);
            var failureResult = firstResult as FailureResult;
            Assert.Equal(FILENAME, failureResult.FileName);
            Assert.StartsWith("UnauthorizedAccessException", failureResult.Error);
        }
    }
}
