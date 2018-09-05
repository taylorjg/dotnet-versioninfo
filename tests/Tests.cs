using DotNetVersionInfo;
using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void NoMatch()
        {
            var fileSystem = new MockFileSystem();
            var domain = new Domain(fileSystem);
            var actual = domain.ProcessFiles("anything", false);
            Assert.Empty(actual);
        }
    }
}
