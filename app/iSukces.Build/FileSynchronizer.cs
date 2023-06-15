using System;
using System.Collections.Generic;
using System.IO;

namespace iSukces.Build;

public class FileSynchronizer
{
    private static bool ClearFilesExcept(DirectoryInfo dir, ICollection<string> keepFiles)
    {
        bool isEmpty = true;
        foreach (var file in dir.GetFiles())
        {
            if (keepFiles.Contains(file.FullName))
            {
                isEmpty = false;
                continue;
            }

            ExConsole.WriteLine("DEL {0}", file);
            file.Delete();
        }

        foreach (var di in dir.GetDirectories())
        {
            var canBeDeleted = ClearFilesExcept(di, keepFiles);
            if (canBeDeleted)
                di.Delete();
        }

        return isEmpty;
    }

    public IEnumerable<FileInfo> GetFileList(Func<FileInfo, bool> predicate)
    {
        foreach (var info in new DirectoryInfo(SourceDir).GetFiles())
        {
            if (predicate(info)) yield return info;
        }
    }

    public bool Synchronize(Func<FileInfo, bool> predicate)
    {
        if ((Flags & SyncFlags.TargetFolderExists) != 0)
            if (!Directory.Exists(TargetDir))
                return false;

        var result = false;

        var keepFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        foreach (var i in GetFileList(predicate))
        {
            var target = new FileInfo(Path.Combine(TargetDir, i.Name));
            keepFiles.Add(target.FullName);

            byte[] GetBytesToSave()
            {
                var expectedRawData = File.ReadAllBytes(i.FullName);
                if (!target.Exists)
                    return expectedRawData;
                if (target.Length != expectedRawData.Length)
                    return expectedRawData;
                var current = File.ReadAllBytes(target.FullName);
                for (var index = 0; index < current.Length; index++)
                {
                    var currentByte  = current[index];
                    var expectedByte = expectedRawData[index];
                    if (currentByte != expectedByte)
                        return expectedRawData;
                }

                return null;
            }

            if ((Flags & SyncFlags.OnlyExistingFiles) != 0)
                if (!target.Exists)
                    continue;

            var toSave = GetBytesToSave();
            if (toSave is null)
            {
                ExConsole.WriteLine("Not changed {0}", target.FullName);
                continue;
            }

            ExConsole.WriteLine("Save {0}", target.FullName);
            File.WriteAllBytes(target.FullName, toSave);
            result = true;
        }

        if ((Flags & SyncFlags.ClearOtherFiles) != 0)
        {
            ClearFilesExcept(new DirectoryInfo(TargetDir), keepFiles);
        }

        return result;
    }

    public string SourceDir { get; set; }
    public string TargetDir { get; set; }

    public SyncFlags Flags { get; set; }
}

[Flags]
public enum SyncFlags
{
    None = 0,
    OnlyExistingFiles = 1,
    TargetFolderExists = 2,
    ClearOtherFiles = 4
}
