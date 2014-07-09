@echo off
if not exist packages\FAKE\tools\Fake.exe ( 
  .nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion -Prerelease
)
if not exist packages\FParsec\lib\net40-client\FParsecCS.dll ( 
  .nuget\nuget.exe install FParsec -OutputDirectory packages -ExcludeVersion
)
if not exist packages\FSharp.Formatting ( 
  .nuget\nuget.exe install FSharp.Formatting -OutputDirectory packages -ExcludeVersion
)
if not exist packages\Deedle ( 
  .nuget\nuget.exe install Deedle -OutputDirectory packages -ExcludeVersion
)
SET srcPath=%1
REM packages\FSharp.Formatting.CommandTool\tools\fsformatting.exe literate --processDirectory --inputDirectory=generate --outputDirectory=output
"C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0\Fsi.exe" generate.fsx