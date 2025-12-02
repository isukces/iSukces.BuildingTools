using System;
using System.Collections.Generic;
using System.IO;

namespace iSukces.Build;

public class BuildConfig
{
    public string? GetCompiledBinary()
    {
        var tmp = CompiledBinary?.Replace("{0}", BuildConfiguration.ToString());
        return tmp;
    }


    public BuildConfig WithNoWarn(IEnumerable<string> codes)
    {
        foreach (var i in codes)
            NoWarn.Add(i);
        return this;
    }

    #region Properties

    public List<string> ProcessesToKillBeforeCompile { get; } = new List<string>();

    public DirectoryInfo SlnDir { get; set; }

    public string SolutionShortFileName { get; set; }

    public string MsBuild { get; set; } =

        @"c:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";

    public List<DirectorySynchronizeItem> OutputFolders { get; } = new List<DirectorySynchronizeItem>();

    // public static Config Instance => ConfigHolder.SingleIstance;

    public DebugOrRelease BuildConfiguration { get; set; }

    public HashSet<string> SkipClearBinObj { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Relative to SolutionDir
    /// </summary>
    public string MainProjectFolder { get; set; }

    /// <summary>
    ///     Relative to MainProject folder
    /// </summary>
    public string? CompiledBinary { get; set; } = "bin\\{0}";

    public string                    ExeName { get; set; }
    public CompilerWarningsContainer NoWarn  { get; set; } = new();
    
    
    [Obsolete("No longer used. Dotnet restere is used instead", true)]
    public string                    Nuget   { get; set; } = "nuget.exe";

    public IlMergeConfig? IlMerge { get; set; }

    public string  RarExe                { get; set; }
    public string  RarOutput             { get; set; }
    public string  InnoSetupSourceScript { get; set; }
    public string  InnoSetupCompilerExe  { get; set; }
    public SlnFile Solution              { get; set; }
    public string  PublishOutputDir      { get; set; }

    public string InstallationFolder { get; set; }

    public bool UpdateVersions { get; set; }

    public PublishSettings PublishSettings { get; } = new();

    #endregion
}

public class PublishSettings
{
    public string Runtime   { get; set; } = "win-x64";
    public string Framework { get; set; } = "net8.0-windows";

    public bool SelfContained { get; set; } = false;
    public bool Force         { get; set; } = true;
}
