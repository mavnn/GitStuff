@echo off
.nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion -Prerelease
.nuget\nuget.exe install FParsec -OutputDirectory packages -ExcludeVersion
.nuget\nuget.exe install FSharp.Formatting -OutputDirectory packages -ExcludeVersion -Version 2.4.6
.nuget\nuget.exe install Deedle -OutputDirectory packages -ExcludeVersion

SET srcPath=%1

"C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0\Fsi.exe" generate.fsx