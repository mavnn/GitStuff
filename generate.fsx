#I "packages/RazorEngine/lib/net40"
#I "packages/FSharp.Formatting/lib/net40"
#r "RazorEngine.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.Literate.dll"
#r "packages/FAKE/tools/FakeLib.dll"
open FSharp.Literate
open System.IO
open Fake

let target = getBuildParam "path"
printfn "Generating output for %s" target

let source = __SOURCE_DIRECTORY__
let template = Path.Combine(source, "template.html")
let script = Path.Combine(source, "glue.fsx")
let fsi = FsiEvaluator()

Literate.ProcessScriptFile(script, template, fsiEvaluator = fsi)