using kCura.Relativity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using RelativityApplication = kCura.Relativity.Client.DTOs.RelativityApplication;

namespace PublishToRelativityConsole
{
    public class RsapiHelper
    {
        private Uri RsapiUri { get; }
        private string RelativityUserName { get; }
        private string RelativityPassword { get; }

        public RsapiHelper(string rsapiUrl, string relativityUserName, string relativityPassword)
        {
            RsapiUri = new Uri(rsapiUrl);
            RelativityUserName = relativityUserName;
            RelativityPassword = relativityPassword;
        }

        private IRSAPIClient GetRsapiClient()
        {
            IRSAPIClient rsapiClient = new RSAPIClient(RsapiUri, new kCura.Relativity.Client.UsernamePasswordCredentials(RelativityUserName, RelativityPassword));
            return rsapiClient;
        }

        public Version GetCurrentApplicationVersion(int workspaceArtifactId, Guid applicationGuid)
        {
            try
            {
                using (IRSAPIClient rsapiClient = GetRsapiClient())
                {
                    rsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
                    RelativityApplication relativityApplication;

                    try
                    {
                        relativityApplication = rsapiClient.Repositories.RelativityApplication.ReadSingle(applicationGuid);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{Constants.ErrorMessages.QueryApplicationVersionError}. ReadSingle.", ex);
                    }

                    Version relativityApplicationVersion = new Version(relativityApplication.Version);
                    return relativityApplicationVersion;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.ErrorMessages.QueryApplicationVersionError, ex);
            }
        }

        public void InstallApplication(int workspaceArtifactId, FileInfo rapFilePath)
        {
            try
            {
                using (IRSAPIClient rsapiClient = GetRsapiClient())
                {
                    rsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
                    List<int> appsToOverride = new List<int>();
                    const bool forceFlag = true;
                    AppInstallRequest appInstallRequest = new AppInstallRequest(appsToOverride, forceFlag)
                    {
                        FullFilePath = rapFilePath.FullName
                    };

                    appsToOverride.Add(1043043); //todo: testing

                    try
                    {
                        rsapiClient.InstallApplication(rsapiClient.APIOptions, appInstallRequest);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{Constants.ErrorMessages.InstallApplicationError}. InstallApplication.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.ErrorMessages.InstallApplicationError, ex);
            }
        }
    }
}
