// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

static string GetSolutionPath () {
   var directoryPath = Directory.GetCurrentDirectory();
   do {
      if (Directory.GetFiles(directoryPath, "*.sln").Length > 0) {
         return directoryPath;
      }

      directoryPath = Path.GetDirectoryName(directoryPath);
   } while (directoryPath != null);

   throw new Exception("Failed to find solution directory.");
}

static string GetCSharpProjectPath (string solutionPath, string projectName) {
   return Path.Combine(solutionPath, projectName, projectName + ".csproj");
}

var solutionPath = GetSolutionPath();
MSBuildLocator.RegisterDefaults();

var workspace = MSBuildWorkspace.Create();
var mainProject =
   await workspace.OpenProjectAsync(GetCSharpProjectPath(solutionPath, "Blinq"));

var compilation = await mainProject.GetCompilationAsync();
Debug.Assert(compilation != null);
var parseDiagnostics = compilation.GetParseDiagnostics();

compilation.Emit(new MemoryStream());
