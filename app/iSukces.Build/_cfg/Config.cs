﻿using System;
using System.Collections.Generic;
using System.IO;

namespace iSukces.Build;

public class Config
{
    private Config()
    {
    }

    public string GetCompiledBinary()
    {
        var tmp = CompiledBinary?.Replace("{0}", BuildConfiguration.ToString());
        return tmp;
    }

    public List<string> ProcessesToKillBeforeCompile { get; } = new List<string>();

    public DirectoryInfo SlnDir { get; set; }

    public string SolutionShortFileName { get; set; }

    public string MsBuild { get; set; } =

        @"c:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";

    public List<DirectorySynchronizeItem> OutputFolders { get; } = new List<DirectorySynchronizeItem>();

    public static Config Instance => ConfigHolder.SingleIstance;

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

    public string          ExeName { get; set; }
    public HashSet<string> NoWarn  { get; }      = new();
    public string          Nuget   { get; set; } = "nuget.exe";

    public IlMergeConfig IlMerge { get; set; }

    public string  RarExe                { get; set; }
    public string  RarOutput             { get; set; }
    public string  InnoSetupSourceScript { get; set; }
    public string  InnoSetupCompilerExe  { get; set; }
    public SlnFile Solution              { get; set; }
    public string  PublishOutputDir      { get; set; }

    public string InstallationFolder { get; set; }

    private static class ConfigHolder
    {
        public static readonly Config SingleIstance = new Config();
    }
}
