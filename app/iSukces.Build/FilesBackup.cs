using System;
using System.Collections.Generic;
using System.IO;

namespace iSukces.Build;

public sealed class FilesBackup
{
    public void Backup(string fileName)
    {
        if (_backupFiles.ContainsKey(fileName))
            return;
        var p = Path.Combine(Path.GetTempPath(), 
            Guid.NewGuid().ToString("N") + ".BAK");
        _backupFiles.Add(fileName, p);
        File.Copy(fileName, p, true);
    }

    public void RestoreAll()
    {
        foreach (var i in _backupFiles)
        {
            if (!File.Exists(i.Value)) continue;
            File.Copy(i.Value, i.Key, true);
            File.Delete(i.Value);
        }

        _backupFiles.Clear();
    }

    private readonly Dictionary<string, string> _backupFiles = new(StringComparer.OrdinalIgnoreCase);
}
