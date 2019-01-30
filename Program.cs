using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using CommandLine;
using JobManagement;
using SimpleImpersonation;

namespace BinaryBootstrapper
{
    class Program
    {
        private static Options _options = new Options();

        private static readonly ManualResetEvent _exitWaitHandle = new ManualResetEvent(false);
        private static Process _childProcess;

        static int Main(string[] args)
        {
            SystemEvents.SetConsoleEventHandler(ConsoleEventCallback);
            //IDisposable impresonationContext = null;
            try
            {
                _options = LoadOptions(args);
                Impersonate(args);
                new Thread(() =>
                {
                    Console.ReadLine();
                    _exitWaitHandle.Set();
                }).Start();
                _exitWaitHandle.WaitOne();
                return 0;
            }
            catch (ValidationException ve)
            {
                Console.Error.WriteLine(ve.Message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            finally
            {
                Shutdown();
            }
            return 1;
        }

        private static void Impersonate(string[] args)
        {
            var exeName = args[0];
            //var exeName = Process.GetCurrentProcess().MainModule.FileName;
            //var args = $"--appRootPath={_options.AppRootPath} --port={_options.Port}";
            //if(_options.UseSSL)
            //    args += $" --thumbprint={_options.Thumbprint} --protocol={_options.Protocol}";
            var processStartInfo = new ProcessStartInfo(exeName)
            {
                Domain = _options.Domain,
                UserName = _options.UsernameWithoutDomain,
                Password = _options.Password.ToSecureString(),
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                Verb="runas"
            };
            _childProcess = Process.Start(processStartInfo);
            if(_childProcess == null)
                throw new Exception("Impersonation of child process failed");
            _childProcess.BeginOutputReadLine();
            _childProcess.BeginErrorReadLine();
            _childProcess.OutputDataReceived += (sender, eventArgs) => Console.WriteLine(eventArgs.Data);
            _childProcess.ErrorDataReceived += (sender, eventArgs) => Console.Error.WriteLine(eventArgs.Data);
            
            var job = new Job();
            job.AddProcess(_childProcess.Handle);
        }

        private static void Shutdown()
        {
            if (_childProcess != null)
            {
                _childProcess.StandardInput.Write("\n");
                _childProcess.WaitForExit(5000);
            }
        }

        static bool ConsoleEventCallback(CtrlEvent eventType)
        {
            _exitWaitHandle.Set();
            return true;
        }

        public static Options LoadOptions(string[] args)
        {
            var options = new Options();
            int port;
            if (int.TryParse(Environment.GetEnvironmentVariable("PORT"), out port))
            {
                options.Port = port;
            }
            options.User = Environment.GetEnvironmentVariable("SERVICE_USERNAME");
            options.Password = Environment.GetEnvironmentVariable("SERVICE_PASSWORD");

            Console.WriteLine($"Using user {options.User} to launch application");
            //var isValid = Parser.Default.ParseArgumentsStrict(args, options);
            //if (!isValid)
            //{
            //    throw new ValidationException("bad args!");
            //}
            
            var userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            options.ApplicationInstanceId = Guid.NewGuid().ToString();
            options.TempDirectory = Path.Combine(userProfileFolder, $"tmp{options.ApplicationInstanceId}");
            var configDirectory = Path.Combine(options.TempDirectory, "config");
            options.AppConfigPath = Path.Combine(configDirectory, "App.config");
          
            return options;
        }

    }
}
