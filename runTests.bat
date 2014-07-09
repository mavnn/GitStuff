@echo off
if not exist packages\FAKE\tools\Fake.exe ( 
  .nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion -Prerelease
)
if not exist packages\FParsec\lib\net40-client\FParsecCS.dll ( 
  .nuget\nuget.exe install FParsec -OutputDirectory packages -ExcludeVersion
)

packages\FAKE\tools\FAKE.exe parserTests.fsx