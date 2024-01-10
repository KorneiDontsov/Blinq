#load "Modules/Environment.fs"
#load "Modules/DotNet.fs"

open System.IO
open Environment

DotNet.exec "clean"
DotNet.exec "nuget locals global-packages --clear"

let root = Directory.GetCurrentDirectory()
let projectPaths =
   Directory.GetDirectories(root)
   |> Seq.filter (fun dirPath -> Path.GetFileName(dirPath).StartsWith(ProjectNames.main))
   |> Seq.toList

let packFilePaths =
   projectPaths
   |> Seq.map (fun projectPath -> Path.Combine(projectPath, "pack"))
   |> Seq.filter Directory.Exists
   |> Seq.collect Directory.GetFiles
let localPackagePaths =
   Directory.GetFiles(Path.Combine(root, "NuGet", "LocalPackages"), "*.nupkg")
let filesToRemove = [packFilePaths; localPackagePaths]
filesToRemove |> Seq.concat |> Seq.iter File.Delete

let generatedCodeDirectoryPaths =
   projectPaths
   |> Seq.map (fun projectPath -> Path.Combine(projectPath, "gen"))
   |> Seq.filter Directory.Exists
   |> Seq.collect Directory.GetDirectories
let directoriesToRemove = [generatedCodeDirectoryPaths]
directoriesToRemove |> Seq.concat |> Seq.iter (fun dirPath -> Directory.Delete(dirPath, recursive=true))
