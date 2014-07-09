@echo off
if not exist packages\FAKE\tools\Fake.exe ( 
  .nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion -Prerelease
)
if not exist packages\FParsec\lib\net40-client\FParsecCS.dll ( 
  .nuget\nuget.exe install FParsec -OutputDirectory packages -ExcludeVersion
)
if not exist packages\FSharp.Formatting\lib\net40\FSharp.Literate.dll ( 
  .nuget\nuget.exe install FSharp.Formatting -OutputDirectory packages -ExcludeVersion
)
if not exist packages\Deedle ( 
  .nuget\nuget.exe install Deedle -OutputDirectory packages -ExcludeVersion
)

packages\FAKE\tools\FAKE.exe generate.fsx "path=%1"
