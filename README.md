When this project is finished, I will publish it to NuGet.
In the meantime, the tool can be installed like this:

```
dotnet pack --output .
dotnet tool install -g dotnet-versioninfo --add-source .
```

Then it can be used as follows.
It displays version information for all assemblies matching `**/*.dll`
relative to the current directory.

```
versioninfo
/Users/jontaylor/HomeProjects/dotnet-versioninfo/obj/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersion:    1.0.0.0
        ProductVersion: 1.0.0
/Users/jontaylor/HomeProjects/dotnet-versioninfo/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersion:    1.0.0.0
        ProductVersion: 1.0.0
/Users/jontaylor/HomeProjects/dotnet-versioninfo/bin/Debug/netcoreapp2.1/publish/Microsoft.DotNet.PlatformAbstractions.dll
        FileVersion:    2.0.0
        ProductVersion: 2.0.0 built by: dlab-DDVSOWINAGE041. Commit Hash: e8b8861ac7faf042c87a5c2f9f2d04c98b69f28d
/Users/jontaylor/HomeProjects/dotnet-versioninfo/bin/Debug/netcoreapp2.1/publish/McMaster.Extensions.CommandLineUtils.dll
        FileVersion:    2.2.3.272
        ProductVersion: 2.2.3-rtm.272
/Users/jontaylor/HomeProjects/dotnet-versioninfo/bin/Debug/netcoreapp2.1/publish/dotnet-versioninfo.dll
        FileVersion:    1.0.0.0
        ProductVersion: 1.0.0
/Users/jontaylor/HomeProjects/dotnet-versioninfo/bin/Debug/netcoreapp2.1/publish/Microsoft.Extensions.DependencyModel.dll
        FileVersion:    2.0.0
        ProductVersion: 2.0.0 built by: dlab-DDVSOWINAGE041. Commit Hash: e8b8861ac7faf042c87a5c2f9f2d04c98b69f28d
/Users/jontaylor/HomeProjects/dotnet-versioninfo/bin/Debug/netcoreapp2.1/publish/McMaster.NETCore.Plugins.dll
        FileVersion:    0.1.1.22
        ProductVersion: 0.1.1-rtm.22+07b28f88149a1b073db1c71ea7f00ec5ebd20dd6
/Users/jontaylor/HomeProjects/dotnet-versioninfo/bin/Debug/netcoreapp2.1/publish/Glob.dll
        FileVersion:    0.4.0.0
        ProductVersion: 0.4.0
/Users/jontaylor/HomeProjects/dotnet-versioninfo/bin/Debug/netcoreapp2.1/publish/Newtonsoft.Json.dll
        FileVersion:    9.0.1.19813
        ProductVersion: 9.0.1
```

# Links

* [.NET Core 2.1 Global Tools](https://natemcmaster.com/blog/2018/05/12/dotnet-global-tools/)
* [Creating a .NET Core global CLI tool for squashing images with the TinyPNG API](https://andrewlock.net/creating-a-net-core-global-cli-tool-for-squashing-images-with-the-tinypng-api/)
* [Version vs VersionSuffix vs PackageVersion: What do they all mean?](https://andrewlock.net/version-vs-versionsuffix-vs-packageversion-what-do-they-all-mean/)
* [CommandLineUtils](https://natemcmaster.github.io/CommandLineUtils/)
