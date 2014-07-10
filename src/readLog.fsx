#r "../packages/FAKE/tools/FakeLib.dll"

open Fake
open System
open System.Diagnostics

let gitPath = getBuildParamOrDefault "git" @"C:\Program Files (x86)\Git\bin\git.exe"
let srcPath = getBuildParamOrDefault "srcPath" __SOURCE_DIRECTORY__

let gitLog () =
    let p = new Process()

    let startInfo =
        let psi = ProcessStartInfo()
        psi.WorkingDirectory <- srcPath
        psi.FileName <- gitPath
        psi.RedirectStandardOutput <- true
        psi.UseShellExecute <- false
        psi.Arguments <- "log --pretty=format:|%ct%n|%an%n --name-only"
        psi

    p.StartInfo <- startInfo
    p.Start() |> ignore
    p.StandardOutput.BaseStream

