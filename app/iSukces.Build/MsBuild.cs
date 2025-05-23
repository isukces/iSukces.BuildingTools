﻿using System.IO;

namespace iSukces.Build;

public class MsBuild
{
    public void Run()
    {
        var solution = new FileInfo(Solution);
        var par      = new CommandLineParameters();
        par.Add(solution.Name);

        AddP("Configuration", Configuration);
        AddP("NoWarn", NoWarn, true);
        if (LogLevel.HasValue)
            par.Add("-v:" + LogLevel.ToString().ToLower());
        if (Multiple)
            par.Add("-m");

        if (!string.IsNullOrWhiteSpace(Target))
            par.Add($"-t:{Target}");

        // AddP("DefineConstants", "HOT,TRACE;DEVELOPER;JETBRAINS_ANNOTATIONS;NETFRAMEWORK;NO_LIVE_LANGUAGE_CHANGE;PIPELINEDESIGNER", true);

        var pList = par.ToArray();
        LastCommand          = Exe + " " + string.Join(" ", pList);
        ExeRunner.WorkingDir = solution.Directory.FullName;
        ExeRunner.Execute(Exe, pList);
        return;
        // "DF92B99D71C141BC924736C9107D1783"

        void AddP(string name, string value, bool quote = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            if (quote)
                value = BuildUtils.Quote(value);
            value = $"/p:{name}={value}";
            par.Add(value);
        }
    }

    #region Properties

    public string LastCommand { get; private set; }

    public string Exe { get; set; }

    public string Target { get; set; } = "Build";

    public string Configuration { get; set; } = "RELEASE";
    public string NoWarn        { get; set; } = "";

    public string Solution { get; set; }

    public bool             Multiple { get; set; }
    public MsBuildLogLevel? LogLevel { get; set; }

    #endregion
}

public enum MsBuildLogLevel
{
    Quiet, Minimal, Normal, Detailed, Diagnostic
}
