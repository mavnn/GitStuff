#I "packages/FSharp.Formatting/lib/net40"
#I "packages/RazorEngine/lib/net40"
#I "packages/FSharp.Compiler.Service/lib/net40"
#r "FSharp.Compiler.Service"
#r "packages/Microsoft.AspNet.Razor/lib/net40/System.Web.Razor.dll"
#r "RazorEngine.dll"
#r "FSharp.Markdown.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.Literate.dll"

open FSharp.Literate
open System.IO

let fsi = FsiEvaluator()
let scriptFile = Path.Combine(__SOURCE_DIRECTORY__, "generate", "glue.fsx")
let template = Path.Combine(__SOURCE_DIRECTORY__, "template.html")
let output = Path.Combine(__SOURCE_DIRECTORY__, "output", "glue.html")
Literate.ProcessScriptFile(scriptFile, template, output, fsiEvaluator=fsi)
