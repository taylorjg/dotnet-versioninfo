## Description

A while back, I encountered the following error whilst developing a little F# app on `.NET Core` / `macOS`:

```
System.MissingMethodException: Method not found: 'System.Tuple`2<Microsoft.FSharp.Core.FSharpFunc`2<System.__Canon,Microsoft.FSharp.Core.FSharpOption`1<!!1>>,Microsoft.FSharp.Core.FSharpFunc`2<!!1,Microsoft.FSharp.Core.FSharpFunc`2<System.__Canon,System.__Canon>>> Prism.ofEpimorphism(Microsoft.FSharp.Core.FSharpFunc`2<System.__Canon,Microsoft.FSharp.Core.FSharpOption`1<!!1>>, Microsoft.FSharp.Core.FSharpFunc`2<!!1,System.__Canon>)'.
   at Chiron.Mapping.Json.writeWith[a](FSharpFunc`2 toJson, String key, a value)
   at Social.Suave.ToJson@158-6.Invoke(Unit unitVar) in /Users/jontaylor/HomeProjects/fsharp/tmp/FsTweet/src/FsTweet.Web/Social.fs:line 158
   at Chiron.Builder.Delay@193.Invoke(Json json)
   at Social.Suave.onFindFolloweesSuccess(FSharpList`1 users) in /Users/jontaylor/HomeProjects/fsharp/tmp/FsTweet/src/FsTweet.Web/Social.fs:line 207
   at Chessie.ErrorHandling.AsyncExtensions.Async.map@241.Invoke(a x)
   at Microsoft.FSharp.Control.AsyncBuilderImpl.args@506-1.Invoke(a a)
```

I wanted to examine the versions of all the DLLs in order to troubleshoot the problem.
However, I could not find an easy way to do this.
I eventually fixed the problem (briefly described [here](https://github.com/taylorjg/FsTweet#package-woes)).

As a result of this experience, I decided to write a
[.NET Core Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
to make it easy to display the version information of .NET Core assemblies.

## Installation

*NOTE: THIS TOOL HAS NOT BEEN NOT RELEASED YET*

```
dotnet tool install --global dotnet-versioninfo
```

## Usage

```
$ versioninfo -h
Display version information of .NET Core assemblies.

Usage: versioninfo [arguments] [options]

Arguments:
  Pattern        Glob pattern [default: **/*.dll]

Options:
  -r|--relative  Show relative paths in the results
  -j|--json      Format the results as JSON
  -v|--version   Display the version of this tool and then exit
  -?|-h|--help   Show help information
```

## Examples

### No Arguments or options

```
$ versioninfo
/Users/jontaylor/HomeProjects/dotnet-versioninfo/tests/obj/Debug/netcoreapp2.1/tests.dll
        FileVersionInfo.FileVersion:    1.0.0.0
        FileVersionInfo.ProductVersion: 1.0.0
/Users/jontaylor/HomeProjects/dotnet-versioninfo/tests/bin/Debug/netcoreapp2.1/xunit.runner.reporters.netcoreapp10.dll
        FileVersionInfo.FileVersion:    2.3.1.3858
        FileVersionInfo.ProductVersion: 2.3.1
/Users/jontaylor/HomeProjects/dotnet-versioninfo/tests/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
/Users/jontaylor/HomeProjects/dotnet-versioninfo/tests/bin/Debug/netcoreapp2.1/xunit.runner.visualstudio.dotnetcore.testadapter.dll
        FileVersionInfo.FileVersion:    2.3.1.3858
        FileVersionInfo.ProductVersion: 2.3.1
/Users/jontaylor/HomeProjects/dotnet-versioninfo/tests/bin/Debug/netcoreapp2.1/xunit.runner.utility.netcoreapp10.dll
        FileVersionInfo.FileVersion:    2.3.1.3858
        FileVersionInfo.ProductVersion: 2.3.1
/Users/jontaylor/HomeProjects/dotnet-versioninfo/tests/bin/Debug/netcoreapp2.1/tests.dll
        FileVersionInfo.FileVersion:    1.0.0.0
        FileVersionInfo.ProductVersion: 1.0.0
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/obj/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/bin/Debug/netcoreapp2.1/publish/McMaster.Extensions.CommandLineUtils.dll
        FileVersionInfo.FileVersion:    2.2.3.272
        FileVersionInfo.ProductVersion: 2.2.3-rtm.272
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/bin/Debug/netcoreapp2.1/publish/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/bin/Debug/netcoreapp2.1/publish/Glob.dll
        FileVersionInfo.FileVersion:    2.0.13.0
        FileVersionInfo.ProductVersion: 2.0.13
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/bin/Debug/netcoreapp2.1/publish/Newtonsoft.Json.dll
        FileVersionInfo.FileVersion:    9.0.1.19813
        FileVersionInfo.ProductVersion: 9.0.1
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/bin/Debug/netcoreapp2.1/publish/System.IO.Abstractions.dll
        FileVersionInfo.FileVersion:    2.1.0.209
        FileVersionInfo.ProductVersion: 2.1.0.209
```

### Glob pattern argument

```
$ versioninfo **/netcore**/*version*.dll
/Users/jontaylor/HomeProjects/dotnet-versioninfo/tests/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/obj/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
/Users/jontaylor/HomeProjects/dotnet-versioninfo/src/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
```

### Relative paths

```
$ versioninfo **/netcore**/*version*.dll -r
tests/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
src/obj/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
src/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll
        FileVersionInfo.FileVersion:    0.0.12.0
        FileVersionInfo.ProductVersion: 0.0.12
```

### JSON output

```
$ versioninfo **/netcore**/*version*.dll -r -j
[
  {
    "FileName": "tests/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll",
    "FileVersion": "0.0.12.0",
    "ProductVersion": "0.0.12"
  },
  {
    "FileName": "src/obj/Debug/netcoreapp2.1/dotnet-versioninfo.dll",
    "FileVersion": "0.0.12.0",
    "ProductVersion": "0.0.12"
  },
  {
    "FileName": "src/bin/Debug/netcoreapp2.1/dotnet-versioninfo.dll",
    "FileVersion": "0.0.12.0",
    "ProductVersion": "0.0.12"
  }
]
```

# Links

* [.NET Core 2.1 Global Tools](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
* [Getting started with creating a .NET Core global tool package](https://natemcmaster.com/blog/2018/05/12/dotnet-global-tools/)
* [A list of tools to extend the .NET Core command line (dotnet)](https://github.com/natemcmaster/dotnet-tools)
* [Creating a .NET Core global CLI tool for squashing images with the TinyPNG API](https://andrewlock.net/creating-a-net-core-global-cli-tool-for-squashing-images-with-the-tinypng-api/)
* [Version vs VersionSuffix vs PackageVersion: What do they all mean?](https://andrewlock.net/version-vs-versionsuffix-vs-packageversion-what-do-they-all-mean/)
* [CommandLineUtils](https://natemcmaster.github.io/CommandLineUtils/)
