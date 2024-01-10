module DotNet

open System.Diagnostics

let exec (arguments: string) =
   use proc = Process.Start("dotnet", arguments)
   proc.WaitForExit()
