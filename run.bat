@echo off
if not exist packages\FAKE (
    .nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion -Prerelease
)
if not exist packages\FParsec (
    .nuget\nuget.exe install FParsec -OutputDirectory packages -ExcludeVersion
)
if not exist packages\FSharp.Formatting (
    .nuget\nuget.exe install FSharp.Formatting -OutputDirectory packages -ExcludeVersion -Version 2.4.6
)
if not exist packages\FsLab (
    .nuget\nuget.exe install FsLab -OutputDirectory packages -ExcludeVersion
)

SET srcPath=%1

"C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0\Fsi.exe" generate.fsx
