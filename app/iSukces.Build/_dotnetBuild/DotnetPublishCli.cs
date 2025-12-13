using System;
using System.Collections.Generic;
using System.IO;

namespace iSukces.Build;

public class DotnetPublishCli
{
    public static bool StandardFilter(FileInfo file, string baseName)
    {
        var ext = file.Extension.ToLower();
        switch (ext)
        {
            case ".pdb" or ".xml": return false;
            case ".exe" or ".dll": return true;
            case ".json":
                var name = baseName + ".deps.json";
                if (EqualsCi(name)) return true;
                name = baseName + ".runtimeconfig.json";
                return EqualsCi(name);
            case ".config":
                return EqualsCi(baseName + ".dll.config");
        }

        return true;

        bool EqualsCi(string name)
        {
            return string.Equals(file.Name, name, StringComparison.OrdinalIgnoreCase);
        }
    }

    public List<string> GetCommandLineparameters()
    {
        var r = new XBuilder();
        r.Add(Verb.ToString().ToLower());
        r.Add(SlnFile);
        r.Add("--configuration", Configuration.ToString().ToLower());
        if (Verb == DotnetVerbs.Publish)
            r.Add("--runtime", Runtime);
        r.Add("--framework", Framework);

        r.Add2(Force, "--force");
        r.Add2(NoLogo, "--nologo");
        r.Add2(NoRestore, "--no-restore");
        r.Add2(NoBuild, "--no-build");
        r.Add2(Nodependencies, "--no-dependencies");
        if (SelfContained)
            r.Add("--self-contained", SelfContained.ToString().ToLower());
        else
            r.Add("--no-self-contained");

        r.Add("--output", OutputDir);
        r.Add("--nowarn", NoWarn.AsDotnetBuild, true);
        {
            if (MsBuild is not null)
            {
                r.AddNotNull("/nr:", MsBuild.NodeReuse);
                r.AddNotNull("/p:UseSharedCompilation=", MsBuild.UseSharedCompilation);
                r.AddNotNull("/m:", MsBuild.MaxCpuCount);
            }
        }
        if (!string.IsNullOrEmpty(Configfile))
            r.Add("--configfile", Configfile);
        if (!NuGetAudit)
            r.Add("-p:NuGetAudit=false");

        r.AddRange(NonstandardCommandLineparameters);
        return r;
    }

    public void Run()
    {
        var clone = (DotnetPublishCli)MemberwiseClone();
        if (string.IsNullOrEmpty(clone.OutputDir))
            throw new Exception("OutputDir is not set");
        if (string.IsNullOrEmpty(clone.SlnFile))
            throw new Exception("SlnFile is not set");

        var tmp = ExeRunner.WorkingDir;
        if (!string.IsNullOrEmpty(tmp))
        {
            clone.SlnFile   = Path.Combine(tmp, clone.SlnFile);
            clone.OutputDir = Path.Combine(tmp, clone.OutputDir);
        }

        var f = new FileInfo(clone.SlnFile);
        if (f.Directory is not null)
        {
            ExeRunner.WorkingDir = f.Directory.FullName;
            clone.SlnFile        = f.Name;
        }

        var od = new DirectoryInfo(clone.OutputDir);
        if (od.Exists)
            BuildUtils.Clear(od);
        else
            od.Create();

        var pList = clone.GetCommandLineparameters().ToArray();
        ExeRunner.Execute("dotnet", pList);
        ExeRunner.WorkingDir = tmp;

        if (AcceptFileAfterBuild is null) return;
        foreach (var f2 in od.GetFiles("*.*", SearchOption.AllDirectories))
            if (!AcceptFileAfterBuild(f2))
                f2.Delete();
    }

    public DotnetVerbs Verb { get; set; } = DotnetVerbs.Publish;

    public List<string> NonstandardCommandLineparameters { get; } = new();

    public string? SlnFile { get; set; }

    public bool Nodependencies { get; set; }

    public DebugOrRelease Configuration { get; set; }

    public string? Runtime { get; set; }

    /// <summary>
    ///     The directory in which to place the published artifacts.
    /// </summary>
    public string? OutputDir { get; set; }

    /// <summary>
    ///     The target framework to publish for.
    /// </summary>
    public string? Framework { get; set; }

    /// <summary>
    ///     Publishes the application as a self-contained application.
    /// </summary>
    public bool SelfContained { get; set; }

    public bool Force { get; set; }

    public bool NoBuild   { get; set; }
    public bool NoRestore { get; set; }
    public bool NoLogo    { get; set; }

    public Func<FileInfo, bool> AcceptFileAfterBuild { get; set; }

    public CompilerWarningsContainer NoWarn { get; set; } = new();

    public MsBuildConfig? MsBuild { get; set; }

    public string? Configfile { get; set; }
    public bool    NuGetAudit { get; set; } = true;

    /*
    public string VersionSuffix                  { get; set; }
    public string VersionPrefix                  { get; set; }
    public string NoDependencies                 { get; set; }
    public string NoIncremental                  { get; set; }
    public string NoCache                        { get; set; }
    public string NoManifest                     { get; set; }
    public string NoVersion                      { get; set; }
    public string NoOptimization                 { get; set; }
    public string NoWarn                         { get; set; }
    public string Verbosity                      { get; set; }
    public string Interactive                    { get; set; }
    public string MSBuildArgs                    { get; set; }
    public string MSBuildArgsAppend              { get; set; }
    public string MSBuildArgsPrepend             { get; set; }
    public string MSBuildPath                    { get; set; }
    public string MSBuildVersion                 { get; set; }
    public string MSBuildRuntime                 { get; set; }
    public string MSBuildArchitecture            { get; set; }
    public string MSBuildNoLogo                  { get; set; }
    public string MSBuildNoAutoResponse          { get; set; }
    public string MSBuildForceConsoleLogger      { get; set; }
    public string MSBuildConsoleLoggerParameters { get; set; }
    public string MSBuildMaxCpuCount             { get; set; }
    */
}
/*
 dotnet publish [<PROJECT>|<SOLUTION>] [-a|--arch <ARCHITECTURE>]
    [-c|--configuration <CONFIGURATION>]
    [-f|--framework <FRAMEWORK>] [--force] [--interactive]
    [--manifest <PATH_TO_MANIFEST_FILE>] [--no-build] [--no-dependencies]
    [--no-restore] [--nologo] [-o|--output <OUTPUT_DIRECTORY>]
    [--os <OS>] [-r|--runtime <RUNTIME_IDENTIFIER>]
    [--sc|--self-contained [true|false]] [--no-self-contained]
    [-s|--source <SOURCE>] [--use-current-runtime, --ucr [true|false]]
    [-v|--verbosity <LEVEL>] [--version-suffix <VERSION_SUFFIX>]

dotnet publish -h|--help
 */

public enum DotnetVerbs
{
    Publish,
    Build,
    Restore
}
