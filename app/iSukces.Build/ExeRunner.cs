using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace iSukces.Build;

public static class ExeRunner
{
    public static int Execute(string exe, params string[] args)
    {
        return Execute(null, exe, args);
    }

    public static int Execute(string exe, EnvironmentVariables? variable, params string[] args)
    {
        return Execute(null, exe, variable, args);
    }

    public static string? LastRunningCommand { get; set; }

    public static int Execute(IEnumerable<int>? ignoreErrorCodes, string exe, params string[] args)
    {
        return Execute(ignoreErrorCodes, exe, null, args);
    }
    
    public static int Execute(IEnumerable<int>? ignoreErrorCodes, string exe, EnvironmentVariables? variables, params string[] args)
    {
        var p = new Process
        {
            StartInfo =
            {
                UseShellExecute        = false,
                RedirectStandardOutput = true,
                FileName               = exe,
                Arguments              = string.Join(" ", args),
                WorkingDirectory       = WorkingDir
            }
        };

        // Redirect the output stream of the child process.
        LastRunningCommand = string.Format("{0} {1}", 
            new Filename(p.StartInfo.FileName).Name.CliQuoteIfNecessary(),
            p.StartInfo.Arguments);
        if (variables is not null)
        {
            foreach (var (key, value) in variables)
            {
                p.StartInfo.EnvironmentVariables[key] = value;
                var text = "Set environment "
                           +ExConsole.Foreground(ConsoleColor.DarkYellow)
                           + key 
                           +ExConsole.Reset
                           + "=" 
                           +ExConsole.Foreground(ConsoleColor.Cyan)
                           + value;
                ExConsole.WriteLine(text);

            }
        }
        ExConsole.WriteLine("Starting {0}", LastRunningCommand);
        p.Start();

        while (true)
        {
            var a = p.StandardOutput.ReadLine();
            if (a == null)
                break;
            ExConsole.WriteLine(a);
        }

        p.WaitForExit();

        ExConsole.WriteLine("Result " + p.ExitCode);
        if (p.ExitCode != 0)
        {
            ExConsole.WriteLine("Failed {0} {1}", new Filename(p.StartInfo.FileName), p.StartInfo.Arguments);
            if (ignoreErrorCodes is null || !ignoreErrorCodes.Contains(p.ExitCode))
                throw new RunException("Task finished with error", p.ExitCode);
        }

        return p.ExitCode;
    }

    public static string WorkingDir
    {
        get => Directory.GetCurrentDirectory();
        set
        {
            if (wasSet && value == WorkingDir) return;
            ExConsole.WriteLine("Change working dir {0}", new Filename(value));
            Directory.SetCurrentDirectory(value);
            wasSet = true;
        }
    }

    private static bool wasSet;
}

public class RunException : Exception
{
    public RunException(string message, int exitCode, Exception? innerException = null)
        : base(message, innerException)
    {
        ExitCode = exitCode;
    }

    public int ExitCode { get; }
}
