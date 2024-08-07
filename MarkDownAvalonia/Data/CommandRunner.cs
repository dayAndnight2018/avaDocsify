using System;
using System.Diagnostics;
using System.IO;

namespace MarkDownAvalonia.Data
{
    /// <summary>
    /// 命令行
    /// </summary>
    public class CommandRunner
    {
        public string ExecutablePath { get; }
        public string WorkingDirectory { get; }
 
        public CommandRunner(string executablePath, string workingDirectory = null)
        {
            ExecutablePath = executablePath ?? throw new ArgumentNullException(nameof(executablePath));
            WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(executablePath);
        }
 
        public string Run(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(ExecutablePath, arguments)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = WorkingDirectory,
                }
            };
            process.Start();
            var log = process.StandardOutput.ReadToEnd();
            Console.WriteLine(log);
            return log;
        }
    }
}