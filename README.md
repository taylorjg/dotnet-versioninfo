```
dotnet pack --output .
dotnet tool install -g dotnet-versioninfo --add-source .
dotnet tool update -g dotnet-versioninfo --add-source .
versioninfo -h
```

# Links

* [.NET Core 2.1 Global Tools](https://natemcmaster.com/blog/2018/05/12/dotnet-global-tools/)
* [Creating a .NET Core global CLI tool for squashing images with the TinyPNG API](https://andrewlock.net/creating-a-net-core-global-cli-tool-for-squashing-images-with-the-tinypng-api/)
* [Version vs VersionSuffix vs PackageVersion: What do they all mean?](https://andrewlock.net/version-vs-versionsuffix-vs-packageversion-what-do-they-all-mean/)
* [CommandLineUtils](https://natemcmaster.github.io/CommandLineUtils/)
