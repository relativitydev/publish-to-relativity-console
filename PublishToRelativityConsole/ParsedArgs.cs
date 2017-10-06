using System;
using System.Collections.Generic;
using System.IO;

namespace PublishToRelativityConsole
{
    public class ParsedArgs
    {
        public string RsapiUrl { get; set; }
        public string RelativityUserName { get; set; }
        public string RelativityPassword { get; set; }
        public int WorkspaceArtifactId { get; set; }
        public Guid ApplicationGuid { get; set; }
        public FileInfo RapFilePath { get; set; }

        public ParsedArgs(List<string> argsList)
        {

            try
            {
                foreach (string arg in argsList)
                {
                    string[] separator = { ":" };
                    string[] key = arg.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
                    switch (key[0].ToLower())
                    {
                        case "/masterurl":
                            RsapiUrl = key[1];
                            break;
                        case "/caseusername":
                            RelativityUserName = key[1];
                            break;
                        case "/caseuserpassword":
                            RelativityPassword = key[1];
                            break;
                        case "/caseid":
                            WorkspaceArtifactId = Convert.ToInt32(key[1]);
                            break;
                        case "/applicationguid":
                            ApplicationGuid = new Guid(key[1]);
                            break;
                        case "/destinationpath":
                            RapFilePath = new FileInfo(key[1]);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.ErrorMessages.ArgsParseError, ex);
            }
        }
    }
}
