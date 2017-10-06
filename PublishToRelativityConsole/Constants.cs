namespace PublishToRelativityConsole
{
    public class Constants
    {
        public const string BuildRapConsoleApplicationFileName = "BuildRAPcmd.exe";

        public class ErrorMessages
        {
            public static readonly string QueryApplicationVersionError = "An error occured when querying for the current application version";
            public static readonly string InstallApplicationError = "An error occured when installating the application";
            public static readonly string ArgsParseError = "An error occured when parsing console arguments";
            public static readonly string CreateRapError = "An error occured when creating RAP file";
            public static readonly string PublishToRelativityConsoleError = "An error occured when building RAP file and installing it into the workspace";
        }
    }
}
