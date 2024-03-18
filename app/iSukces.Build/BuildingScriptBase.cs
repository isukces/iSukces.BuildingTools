using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace iSukces.Build;

public class BuildingScriptBase : IRollbackContainer
{
    protected BuildingScriptBase(BuildConfig configuration)
    {
        Configuration = configuration;
        List<string> items = new();
        if (!string.IsNullOrEmpty(configuration.MainProjectFolder))
            items.AddRange(configuration.MainProjectFolder.Split('\\', '/'));
        var tmp = configuration.GetCompiledBinary();
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
        var start   = DateTime.Now;
        var msBuild = new MsBuild();
        try
        {
            msBuild.Exe           = Configuration.MsBuild;
            msBuild.Configuration = Configuration.BuildConfiguration.ToString().ToUpper();
            msBuild.Multiple      = true;
            msBuild.Solution      = Path.Combine(Configuration.SlnDir.FullName, Configuration.SolutionShortFileName);
            msBuild.Target        = "Clean";
            msBuild.NoWarn        = string.Join(";", Configuration.NoWarn.OrderBy(a => a));
            msBuild.LogLevel      = MsBuildLogLevel.Quiet;
            msBuild.Run();
        }
        catch (Exception e)
        {
            Log(e, msBuild.LastCommand);
            throw;
        }

        try
        {
            // nuget restore
            ExeRunner.Execute(Configuration.Nuget, "restore", Configuration.SolutionShortFileName);
        } catch (Exception e)
        {
            Log(e, "nuget restore");
            throw;
        }

        try
        {
            // build
            msBuild.Target = "Build";
            msBuild.Run();
            ExConsole.WriteLine("Compile time {0}", DateTime.Now - start);
        }
        catch (Exception e)
        {
            Log(e, msBuild.LastCommand);
            throw;
        }
        
        void Log(Exception e, string command)
        {
            ExConsole.WriteLine("Error running command");
            ExConsole.WriteLine(command);
            ExConsole.WriteLine(e.Message);
        }
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
        if (Configuration.BuildConfiguration != DebugOrRelease.Release) return;
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
        var csProjs = projs.Select(a => new FileInfo(Path.Combine(Configuration.SlnDir.FullName, a))).ToArray();

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


    public class SavedFile
    {
        public SavedFile(string csProj, string copy)
        {
            CsProj = csProj;
            Copy   = copy;
        }

        public string CsProj { get; }
        public string Copy   { get; }
    }

    protected void A09_TurnOffGeneratePackageOnBuild()
    {
        var f    = Configuration.SlnDir.GetFiles("*.csproj", SearchOption.AllDirectories);
        var list = new List<SavedFile>();
        foreach (var i in f)
        {
            var save = false;
            var xml  = XDocument.Load(i.FullName);
            if (xml.Root is null)
                continue;
            var ns = xml.Root.Name.Namespace;
            var q  = xml.Root.Elements(ns + "PropertyGroup");
            foreach (var j in q.Elements(ns + "GeneratePackageOnBuild"))
            {
                if (j.Value == "false") continue;
                j.Value = "false";
                save    = true;
            }

            if (!save) continue;
            var csproj              = i.FullName; // i is the FileInfo object
            var destinationFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csproj");
            File.Copy(csproj, destinationFilePath, true); // true to overwrite existing files
            list.Add(new SavedFile(csproj, destinationFilePath));
            xml.Save(csproj);
        }

        if (list.Count == 0) return;
        AddRollbackAction(() =>
        {
            foreach (var i in list)
            {
                File.Copy(i.Copy, i.CsProj, true);
                File.Delete(i.Copy);
            }
        });
    }


    protected virtual bool AcceptFile(FileInfo file)
    {
        if (string.Equals(file.Extension, ".pdb", StringComparison.OrdinalIgnoreCase))
            return false;
        if (string.Equals(file.Name, Configuration.ExeName + ".config", StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    public void AddRollbackAction(Action action)
    {
        RollbackActions.Add(action);
    }


    public void KillBeforeCompile()
    {
        var processes = Configuration.ProcessesToKillBeforeCompile.Distinct().ToArray();
        foreach (var i in processes)
            Kill(i);
    }

    public void RollbackModifications()
    {
        foreach (var action in RollbackActions)
            action();
        RollbackActions.Clear();
    }

    public string SlnDir(params string[] pathItems)
    {
        var dir = Configuration.SlnDir.FullName;
        foreach (var item in pathItems)
            dir = Path.Combine(dir, item);
        dir = new DirectoryInfo(dir).FullName;
        return dir;
    }

    public DirectorySynchronizeItem HotOutput { get; set; }

    public BuildConfig Configuration { get; }

    protected string CompiledBinariesDir { get; init; }

    public ReactorProtect? Reactor { get; init; }

    readonly List<Action> RollbackActions = new List<Action>();
}
