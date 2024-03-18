using System;
using System.IO;

namespace iSukces.Build;

public sealed class ReactorProtect
{
    private static void SyncDirectories(string sourceDir, string targetDir)
    {
        var synchronizer = new FileSynchronizer
        {
            SourceDir = sourceDir,
            TargetDir = targetDir,
            Flags     = SyncFlags.ClearOtherFiles
        };
        synchronizer.Synchronize(a =>
        {
            return true;
        });
        foreach (var i in new DirectoryInfo(sourceDir).GetDirectories())
        {
            var s1 = Path.Combine(sourceDir, i.Name);
            var t1 = Path.Combine(targetDir, i.Name);
            new DirectoryInfo(t1).Create();
            SyncDirectories(s1, t1);
        }
    }

    public void Run(string output)
    {
        Console.WriteLine("NrProtect");
        BuildUtils.Clear(new DirectoryInfo(FilesToProtect));
        BuildUtils.Clear(new DirectoryInfo(Secured));

        SyncDirectories(output, FilesToProtect);

        var nrProj = new FileInfo(NrProj);
        var w      = ExeRunner.WorkingDir;
        ExeRunner.WorkingDir = nrProj.Directory.FullName;
        ExeRunner.Execute(ReactorExe, "-project", nrProj.Name);
        ExeRunner.WorkingDir = w;

        var synchronizer = new FileSynchronizer
        {
            SourceDir = Secured,
            TargetDir = output,
            Flags     = SyncFlags.OnlyExistingFiles
        };
        synchronizer.Synchronize(a => string.Equals(a.Extension, ".dll", StringComparison.OrdinalIgnoreCase));
    }

    #region Properties

    public string NrProj         { get; set; }
    public string Secured        { get; set; }
    public string FilesToProtect { get; set; }
    public string ReactorExe     { get; set; }

    #endregion
}
