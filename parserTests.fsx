#r "packages/FParsec/lib/net40-client/FParsecCS.dll"
#r "packages/FParsec/lib/net40-client/FParsec.dll"
#load "logParser.fsx"

open FParsec
open System
open System.IO
open LogParser

let test1 = """|1364401289
|Michael Newton

.gitignore

nuget.config

|1364401289
|Michael Newton

.gitignore
nuget.config

|1364401289
|

.gitignore
nuget.config
"""

let makeStream (s : String) =
    let m = new MemoryStream()
    let w = new StreamWriter(m)
    w.Write(s)
    w.Flush()
    m.Seek(0L, SeekOrigin.Begin) |> ignore
    m

let result = runParserOnString stream () "test1" test1

match result with
| Success (x, _, _) -> printfn "%A" x
| Failure (e, _, _) -> printfn "Error: %A" e

