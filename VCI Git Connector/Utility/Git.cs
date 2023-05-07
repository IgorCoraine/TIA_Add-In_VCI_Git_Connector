using System;
using System.Text;
using Siemens.Engineering;
using Process = Siemens.Engineering.AddIn.Utilities.Process;
using ProcessStartInfo = Siemens.Engineering.AddIn.Utilities.ProcessStartInfo;

namespace Siemens.Applications.AddIns.VCIGitConnector.Utility
{
    public static class Git
    {
        public static Process CreateGitProcess(string arguments, string workingDirectory)
        {
            var info = new ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            return new Process
            {
                StartInfo = info
            };
        }

        public static bool ExecuteGitCommand(Process process, out string outputMessage, out string errorMessage, TiaPortal tiaPortal)
        {
            using (tiaPortal.ExclusiveAccess("Executing Git Command..."))
            {

                var outputData = new StringBuilder();
                var errorData = new StringBuilder();

                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    outputMessage = string.Empty;
                    errorMessage = "Could not find a valid Git installation" + Environment.NewLine + ex;
                    return false;
                }
                
                process.OutputDataReceived += (sender, args) => outputData.AppendLine(args.Data);
                process.BeginOutputReadLine();
                process.ErrorDataReceived += (sender, args) => errorData.AppendLine(args.Data);
                process.BeginErrorReadLine();
                process.WaitForExit();

                outputMessage = outputData.ToString();
                errorMessage = errorData.ToString();
            }

            return process.ExitCode == 0;
        }
    }
}
