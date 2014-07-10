#I "packages/FSharp.Formatting/lib/net40"
#I "packages/RazorEngine/lib/net40"
#I "packages/FSharp.Compiler.Service/lib/net40"
#I "packages/FAKE/tools"
#r "FSharp.Compiler.Service"
#r "packages/Microsoft.AspNet.Razor/lib/net40/System.Web.Razor.dll"
#r "RazorEngine.dll"
#r "FSharp.Markdown.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.Literate.dll"
#r "FakeLib.dll"

#load "FsLab.fsx"
#load "formatters.fsx"

open FSharp.Literate
open Fake
open System.IO

printfn "Clean up!"
CleanDir "output"

printfn "Generate!"
#time
let scriptFile = Path.Combine(__SOURCE_DIRECTORY__, "generate", "glue.fsx")
let template = Path.Combine(__SOURCE_DIRECTORY__, "template.html")
let output = Path.Combine(__SOURCE_DIRECTORY__, "output", "glue.html")
let fsi = Formatters.createFsiEvaluator (Path.GetDirectoryName output) (Path.GetDirectoryName output)
Literate.ProcessScriptFile(scriptFile, template, output, fsiEvaluator=fsi)
#time
printfn "Done!"