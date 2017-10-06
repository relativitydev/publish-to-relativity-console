using ResourceFileUpload;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PublishToRelativityConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("--> Start");
                List<string> argsList = args.ToList();

                Console.WriteLine("--> Parsing args");
                ParsedArgs parsedArgs = new ParsedArgs(argsList);
                Console.WriteLine("--> Parsed args");

                //Get current Application Version In Workspace 
                Console.WriteLine("--> Retrieving current Application Version In Workspace");
                RsapiHelper rsapiHelper = new RsapiHelper(parsedArgs.RsapiUrl, parsedArgs.RelativityUserName, parsedArgs.RelativityPassword);
                Version currentApplicationVersionInWorkspace = rsapiHelper.GetCurrentApplicationVersion(parsedArgs.WorkspaceArtifactId, parsedArgs.ApplicationGuid);
                Console.WriteLine($"--> Retrieved current Application Version In Workspace [{currentApplicationVersionInWorkspace}]");

                //Add Application Version to args
                Version newApplicationVersion = new Version(
                    currentApplicationVersionInWorkspace.Major,
                    currentApplicationVersionInWorkspace.Minor,
                    currentApplicationVersionInWorkspace.Build,
                    currentApplicationVersionInWorkspace.Revision + 1);
                argsList.Add($"/applicationversion:{newApplicationVersion}");

                //Update RAP filename
                Console.WriteLine("--> Updating Rap Filename with new Application version number");
                UpdateRapFileNameWithNewApplicationVersionNumber(argsList, newApplicationVersion, parsedArgs);
                Console.WriteLine("--> Updated Rap Filename with new Application version number");

                //create RAP file using RapBuilder
                //Console.WriteLine("--> Creating RAP file");
                //CreateRapFile(argsList);
                //Console.WriteLine("--> Created RAP file");

                //Install the new RAP in workspace
                Console.WriteLine("--> Installing application into workspace");
                InstallRapFile(argsList);
                Console.WriteLine("--> Installed application into workspace");
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.ErrorMessages.PublishToRelativityConsoleError, ex);
            }
        }

        private static void UpdateRapFileNameWithNewApplicationVersionNumber(List<string> argsList, Version newApplicationVersion, ParsedArgs parsedArgs)
        {
            string argsName = "/destinationPath";
            string givenRapFileName = argsList.First(x => x.Contains(argsName));
            argsList.Remove(givenRapFileName);
            string split = givenRapFileName.Substring(0, givenRapFileName.Length - 4);
            string newRapFileName = $"{split}_{newApplicationVersion}.rap";
            argsList.Add(newRapFileName);
            parsedArgs.RapFilePath = new FileInfo(newRapFileName.Replace($"{argsName}:", ""));
        }

        private static void CreateRapFile(List<string> argsList)
        {
            try
            {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string exeFullPath = $@"{exeDirectory}\{Constants.BuildRapConsoleApplicationFileName}";
                string argsString = string.Join(" ", argsList);
                Console.WriteLine($"--> Args [{argsString}]");

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = exeFullPath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = argsString
                };

                using (Process exeProcess = Process.Start(processStartInfo))
                {
                    exeProcess?.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.ErrorMessages.CreateRapError, ex);
            }
        }

        private static void InstallRapFile(List<string> argsList)
        {
            try
            {
                ArgsToProcess argsToProcess = new ArgsToProcess(argsList.ToArray());
                ResourceFileUploader resourceFileUploader = new ResourceFileUploader();
                resourceFileUploader.RunUpdater(argsToProcess);
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.ErrorMessages.CreateRapError, ex);
            }
        }
    }
}
