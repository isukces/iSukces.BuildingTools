using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iSukces.Build;

public class BuildingScriptBase
{
    protected BuildingScriptBase(Config cfg)
    {
        Cfg = cfg;
        List<string> items = new();
        if (!string.IsNullOrEmpty(cfg.MainProjectFolder))
            items.AddRange(cfg.MainProjectFolder.Split('\\', '/'));
        var tmp = cfg.GetCompiledBinary();
        if (!string.IsNullOrEmpty(tmp))
            items.AddRange(tmp.Split('\\', '/'));
        CompiledBinariesDir = SlnDir(items.ToArray());
    }

    private static void Kill(string exe)
    {
        try
        {
            ExeRunner.Execute(new[] { 128 }, "taskkill", "/im", exe, "/f");
        }
        catch (RunException ex)
        {
            if (ex.ExitCode != 128)
                throw;
        }
    }


    protected void A03_Compile()
    {
        var start = DateTime.Now;
        var msBuild = new MsBuild
        {
            Exe           = Cfg.MsBuild,
            Configuration = Cfg.BuildConfiguration.ToString().ToUpper(),
            Multiple      = true,
            Solution      = Path.Combine(Cfg.SlnDir.FullName, Cfg.SolutionShortFileName),
            Target        = "Clean",
            NoWarn        = string.Join(",", Cfg.NoWarn.OrderBy(a => a)),
            LogLevel      = MsBuildLogLevel.Quiet
        };
        msBuild.Run();

#if !SKIP_COMPILE
        // nuger restore
        ExeRunner.Execute(Cfg.Nuget, "restore", Cfg.SolutionShortFileName);

        // build
        msBuild.Target = "Build";
        msBuild.Run();
        ExConsole.WriteLine("Compile time {0}", DateTime.Now - start);
#endif
    }


    public void A06_DeleteEmbeddedAndMergedBinaries(string[] ilMerged, StringList manuallyEmbeddedPlusForbidden = null)
    {
        manuallyEmbeddedPlusForbidden ??= new StringList(new List<string>());
        var filesToDelete = (manuallyEmbeddedPlusForbidden + ilMerged).Items;
        filesToDelete = filesToDelete
            .Distinct(StringComparer.InvariantCultureIgnoreCase)
            .OrderBy(a => a)
            .ToList();

        foreach (var assemblyFileName in filesToDelete)
        {
            foreach (var fileName in BuildUtils.AssemblyRelatedFiles(assemblyFileName, true))
            {
                var fi = new FileInfo(Path.Combine(CompiledBinariesDir, fileName));
                if (fi.Exists)
                {
                    ExConsole.WriteLine("DEL {0}", new Filename(fi.Name));
                    fi.Delete();
                }
            }
        }
    }

    public void A08_SyncToHotOutput(DirectorySynchronizeItem output, Func<FileInfo, bool> predicate)
    {
        if (string.IsNullOrEmpty(output?.Folder))
            return;
        if (Cfg.BuildConfiguration != DebugOrRelease.Release) return;
        var synchronizer = new FileSynchronizer
        {
            SourceDir = CompiledBinariesDir,
            TargetDir = output.Folder,
            Flags     = output.Flags
        };
        new DirectoryInfo(output.Folder).Create();
        synchronizer.Synchronize(predicate);
    }

    protected string A08_UpdateVersions(string[] projs, string mainName, bool updateVersions)
    {
        var csProjs = projs.Select(a => new FileInfo(Path.Combine(Cfg.SlnDir.FullName, a))).ToArray();

        var mains = csProjs.Where(a => string.Equals(a.Name, mainName, StringComparison.OrdinalIgnoreCase)).ToArray();
        if (mains.Length == 0)
            throw new Exception($"Cannot find main project {mainName}");
        if (mains.Length > 1)
            throw new Exception($"More than one main project {mainName}");
        var    main = mains[0];
        string version;
        {
            var csProj = CsProjFile.Load(main.FullName);
            version = csProj.GetVersion("AssemblyVersion");
        }
        if (updateVersions)
        {
            version = CsProjFile.UpdateVersion(version);
            foreach (var i in csProjs)
            {
                if (i.Name.IndexOf("isukces", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;
                var csProj      = CsProjFile.Load(i.FullName);
                var projVersion = version;
                csProj.SetAllVersions(projVersion);
                csProj.Save(i.FullName);
            }
        }

        return version;
    }

    protected virtual bool AcceptFile(FileInfo file)
    {
        if (string.Equals(file.Extension, ".pdb", StringComparison.OrdinalIgnoreCase))
            return false;
        if (string.Equals(file.Name, Cfg.ExeName + ".config", StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }


    public void KillBeforeCompile()
    {
        var processes = Cfg.ProcessesToKillBeforeCompile.Distinct().ToArray();
        foreach (var i in processes)
            Kill(i);
    }

    public string SlnDir(params string[] pathItems)
    {
        var dir = Cfg.SlnDir.FullName;
        foreach (var item in pathItems)
            dir = Path.Combine(dir, item);
        dir = new DirectoryInfo(dir).FullName;
        return dir;
    }

    public DirectorySynchronizeItem HotOutput { get; set; }

    public Config Cfg { get; }

    protected string CompiledBinariesDir { get; set; }
}
