using System;
using System.Collections.Generic;
using System.IO;

namespace iSukces.Build;


[Obsolete("Use MsBuildCli instead", true)]
public class MsBuild
{
    
}

public class MsBuildCli : MsBuildConfig
{
    public void Run()
    {
        var solution = new FileInfo(Solution);
        var par      = new CommandLineParameters();
        par.Add(solution.Name);

        AddP("Configuration", Configuration);
        AddP("NoWarn", NoWarn, true);
        if (LogLevel.HasValue)
            par.Add("-v:" + LogLevel.ToString()!.ToLower());
        if (Multiple)
            par.Add("-m");

        if (!string.IsNullOrWhiteSpace(Target))
            par.Add($"-t:{Target}");

        if (!string.IsNullOrWhiteSpace(PublishDir))
            par.Add($"/p:PublishDir={PublishDir.CliQuoteIfNecessary()}");

        if (!string.IsNullOrWhiteSpace(RuntimeIdentifier))
            par.Add($"/p:RuntimeIdentifier={RuntimeIdentifier.CliQuoteIfNecessary()}");

        Add1("SelfContained", SelfContained);
        Add1("UseCurrentRuntimeIdentifier", UseCurrentRuntimeIdentifier);

        Add1("PublishReadyToRun", PublishReadyToRun);
        // AddP("DefineConstants", "HOT,TRACE;DEVELOPER;JETBRAINS_ANNOTATIONS;NETFRAMEWORK;NO_LIVE_LANGUAGE_CHANGE;PIPELINEDESIGNER", true);

        foreach (var i in NonstandardCommandLineparameters)
            par.Add(i);

        var pList = par.ToArray();
        LastCommand          = Exe.CliQuoteIfNecessary() + " " + string.Join(" ", pList);
        ExeRunner.WorkingDir = solution.Directory.FullName;
        ExeRunner.Execute(Exe, pList);
        return;

        void Add1(string key, bool? value)
        {
            if (value is not null)
                AddP(key, value.ToString()!.ToLower());
        }

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

    public string? LastCommand { get; private set; }

    public string? Exe               { get; set; }
    public string  Target            { get; set; } = "Build";
    public string? PublishDir        { get; set; }
    public string? RuntimeIdentifier { get; set; }
    public string? Solution          { get; set; }
    
    public bool? SelfContained               { get; set; }
    public bool? UseCurrentRuntimeIdentifier { get; set; }
    public bool? PublishReadyToRun           { get; set; }
    
    
    public List<string> NonstandardCommandLineparameters { get; } = [];
}