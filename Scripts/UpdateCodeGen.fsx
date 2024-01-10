open System
open System.IO

#load "Modules/Environment.fs"
#load "Modules/DotNet.fs"
#load "Modules/FileUtils.fs"

open Environment

let root = Directory.GetCurrentDirectory()

// Clean generated code files.
Directory.GetDirectories(Path.Combine(root, ProjectNames.main, "gen"))
|> Seq.iter (fun dirPath -> Directory.Delete(dirPath, recursive=true))

let versionFilePath =
    Path.Combine(root, ProjectNames.codeGen, "pack", "version.txt")

let currentVersion =
    FileUtils.read versionFilePath UInt32.Parse |> ValueOption.defaultValue 0u

let nextVersion = currentVersion + 1u

DotNet.exec $"build {ProjectNames.codeGen} --property:Version=0.0.{nextVersion}"
nextVersion |> FileUtils.write versionFilePath

// Invalidate package cache
let packageCachePath = Path.Combine(root, "NuGet", "PackageCaches", ProjectNames.codeGen.ToLowerInvariant())
if Directory.Exists(packageCachePath) then Directory.Delete(packageCachePath, recursive=true)

DotNet.exec $"restore {ProjectNames.main}"
