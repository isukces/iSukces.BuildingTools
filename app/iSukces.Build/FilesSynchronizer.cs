using System;
using System.Collections.Generic;
using System.IO;

namespace iSukces.Build;

public static class FilesSynchronizer
{
    public static void SyncFiles(string src, string target, Func<FileInfo, bool> accept)
    {
        var srcDir = new DirectoryInfo(src);
        if (!srcDir.Exists)
            return;

        
        var keep = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        SyncFiles(src, target, accept, keep);
        
        var targetDir = new DirectoryInfo(target);
        foreach (var file in targetDir.GetFiles("*",SearchOption.AllDirectories))
        {
            if (!keep.Contains(file.FullName))
                file.Delete();
        }
        
    }

    private static void SyncFiles(string src, string target, Func<FileInfo, bool> accept, HashSet<string> keep)
    {
        var srcDir    = new DirectoryInfo(src);
        var targetDir = new DirectoryInfo(target);
        if (!targetDir.Exists)
            targetDir.Create();

        // Copy files
        foreach (var srcFile in srcDir.GetFiles())
        {
            if (!accept(srcFile))
                continue;

            var targetFile = new FileInfo(Path.Combine(targetDir.FullName, srcFile.Name));
            srcFile.CopyTo(targetFile.FullName, true);
            keep.Add(targetFile.FullName);
        }

        // Delete files in target that don't exist in source or don't pass the filter
        foreach (var targetFile in targetDir.GetFiles())
        {
            var srcFile = new FileInfo(Path.Combine(srcDir.FullName, targetFile.Name));
            if (!srcFile.Exists || !accept(srcFile))
                targetFile.Delete();
        }

        // Recursively sync subdirectories
        foreach (var srcSubDir in srcDir.GetDirectories())
        {
            var targetSubDir = Path.Combine(targetDir.FullName, srcSubDir.Name);
            SyncFiles(srcSubDir.FullName, targetSubDir, accept, keep);
        }

        // Delete subdirectories in target that don't exist in source
        foreach (var targetSubDir in targetDir.GetDirectories())
        {
            var srcSubDir = new DirectoryInfo(Path.Combine(srcDir.FullName, targetSubDir.Name));
            if (!srcSubDir.Exists)
                targetSubDir.Delete(true);
        }
    }
}