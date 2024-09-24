using System;
using System.Collections.Generic;
using System.IO;

namespace iSukces.Build;

public static class BuildUtils
{
    public static IEnumerable<string> AssemblyRelatedFiles(string baseName, bool addBaseName)
    {
        if (addBaseName)
            yield return baseName;

        string AddAlso(string ext, bool cut)
        {
            if (cut)
                return ChangeFileExtension(baseName, ext);
            return baseName + "." + ext;
        }

        yield return AddAlso("pdb", true);
        yield return AddAlso("config", false);
        yield return AddAlso("xml", true);
    }

    public static string ChangeFileExtension(string file, string ext)
    {
        var f = new FileInfo(file).Extension;
        if (!ext.StartsWith(".", StringComparison.Ordinal))
            ext = $".{ext}";
        if (string.IsNullOrEmpty(f))
            return file + ext;
        return file.Substring(0, file.Length - f.Length) + ext;
    }

    public static void Clear(DirectoryInfo directory)
    {
        if (directory is null || !directory.Exists)
            return;
        foreach (var i in directory.GetFiles())
            i.Delete();
        foreach (var i in directory.GetDirectories())
            i.Delete(true);
    }

    public static void ClearBinObj(DirectoryInfo dir, HashSet<string> skipClearBinObj = null)
    {
        ClearBinObj(dir, false, skipClearBinObj);
    }

    private static void ClearBinObj(DirectoryInfo dir, bool delete, HashSet<string> skipClearBinObj)
    {
        if (skipClearBinObj is not null && skipClearBinObj.Contains(dir.FullName))
            return;

        foreach (var i in dir.GetDirectories())
        {
            var isBinObj = IsBinObj(i);
            ClearBinObj(i, isBinObj || delete, skipClearBinObj);
        }

        if (!delete) return;
        foreach (var i in dir.GetFiles())
        {
            if (DisplayDeletedFiles)
                Console.WriteLine("Delete " + i.FullName);
            i.Delete();
        }

        if (IsBinObj(dir)) return;
        dir.Delete();
        Console.WriteLine("Delete " + dir.FullName);
    }

    public static string Encode(string parameter)
    {
        if (ShouldBeEncoded(parameter))
            return Quote(parameter);
        return parameter;
    }

    private static bool IsBinObj(DirectoryInfo directory)
    {
        return directory.Name.ToLower() is "bin" or "obj";
    }

    public static string Quote(string parameter)
    {
        return $"\"{parameter}\"";
    }

    private static bool ShouldBeEncoded(string parameter)
    {
        return parameter.Contains(' ', StringComparison.Ordinal);
    }

    public static bool DisplayDeletedFiles { get; set; } = true;
}