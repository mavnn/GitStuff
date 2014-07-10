(*** hide ***)
#I "../packages/FParsec/lib/net40-client"
#I "../packages/FAKE/tools"
#I "../packages/Deedle/lib/net40"
#I "../packages/FSharp.Charting/lib/net40"
#r "FParsecCS.dll"
#r "FParsec.dll"
#r "FakeLib.dll"
#r "Deedle.dll"
#r "FSharp.Charting.dll"
#load "../src/logParser.fsx"
#load "../src/readLog.fsx"
open Fake.EnvironmentHelper
open System
open FParsec
open LogParser
open ReadLog
open FSharp.Charting
open Deedle

let srcPath = getBuildParamOrDefault "srcPath" __SOURCE_DIRECTORY__
let result = parseLog <| gitLog srcPath

(** 
## Truncated git log... *)

(*** hide ***)

type Commit =
    {
        Date : DateTime
        Author : string
        Files : string list
    }

let frame =
    result
    |> List.mapi (fun i (d, a, f) -> { Date = d.AddTicks(int64 i); Author = a; Files = f })
    |> List.rev
    |> Frame.ofRecords
    |> Frame.indexRowsDate "Date"
    |> Frame.sortRowsByKey

(*** include-value:frame ***)

(** 
# Time to slice and dice... 

## Files changed per commit
*)

(*** hide ***)

// Add column for file count
let files = frame.Rows |> Series.mapValues(fun row -> row.GetAs<string list>("Files").Length)
frame?FileCount <- files

let chart = Chart.Line(frame?FileCount |> Series.observations)

(*** include-value:chart ***)

(** 
## Windowed by time...
*)

(*** hide ***)
let commitsPerWeek =
    frame.GetColumn<string>("Author")
    |> Series.chunkDistInto (TimeSpan(7, 0, 0, 0, 0)) Stats.count

let chart2 = Chart.Line(commitsPerWeek |> Series.observations)

(*** include-value:chart2 ***)