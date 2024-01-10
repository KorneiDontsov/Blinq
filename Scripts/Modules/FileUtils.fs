module FileUtils

open System.IO

let read (filePath: string) parse =
   if File.Exists(filePath) then
      File.ReadAllText(filePath) |> parse |> ValueSome
   else
      ValueNone

let write (filePath: string) number =
   let rec ensureDirectoryExists (directoryPath: string) =
      match Directory.Exists(directoryPath) with
      | true -> ()
      | false ->
         Path.GetDirectoryName(directoryPath) |> ensureDirectoryExists
         Directory.CreateDirectory(directoryPath) |> ignore

   Path.GetDirectoryName(filePath) |> ensureDirectoryExists
   File.WriteAllText(filePath, number.ToString())
