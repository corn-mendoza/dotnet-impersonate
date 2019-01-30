using System;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;

namespace BinaryBootstrapper
{
    public class Options
    {
        private string _user;
      
        [Option("appRootPath", DefaultValue = ".", HelpText = "app web root path", Required = false)]
        public string AppRootPath { get; set; } = Environment.CurrentDirectory;

        [Option("port", HelpText = "port for the application to listen with", Required = false)]
        public int Port { get; set; } = 8080;


        [Option("user", HelpText = "windows username to run application to run under (if user part of domain, use DOMAIN\\Username)", Required = false)]
        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                UsernameWithoutDomain = _user;
                Domain = null;
                if (_user == null) return;

                var match = Regex.Match(_user, @"^(?<domain>\w+)\\(?<user>\w+)$"); // parse out domain from format DOMAIN\Username

                if (match.Success)
                {
                    UsernameWithoutDomain = match.Groups["user"].Value;
                    Domain = match.Groups["domain"].Value;
                }
            }
        }

        public string UsernameWithoutDomain { get; private set; }
        public string Domain { get; private set; }

        [Option("password", HelpText = "windows password to run application to run under", Required = false)]
        public string Password { get; set; }
        [Option("thumbprint", HelpText = "certificate thumbprint", Required = false)]
        public string Thumbprint { get; set; }
        [Option("protocol", HelpText = "http or https", Required = false)]
        public string Protocol { get; set; } = "http";

        public bool UseSSL => Protocol == "https";
        public string AspnetConfigPath { get; set; } = string.Empty;
        public string AppConfigPath { get; set; } = string.Empty;
        public string WebConfigPath { get; set; } = string.Empty;
        public string TempDirectory { get; set; } = string.Empty;
        public string ConfigDirectory => Path.Combine(TempDirectory, "config");
        public string ApplicationHostConfigPath { get; set; } = string.Empty;
        public string ApplicationInstanceId { get; set; } = Guid.NewGuid().ToString();
    }
}