using System.IO;
using System.Reflection;

namespace iSukces.Build;

public static class FileExtensions
{
    public static FileInfo CreateFile(this DirectoryInfo dir, string fileName)
    {
        var path = Path.Combine(dir.FullName, fileName);
        return new FileInfo(path);
    }

    public static void DeleteFileAndLog(string fileName)
    {
        var filename = new Filename(fileName);
        var fi       = new FileInfo(fileName);
        if (!fi.Exists)
        {
            ExConsole.WriteLine("File {0} doesn't exist", filename);
            return;
        }

        ExConsole.WriteLine("DEL {0}", filename);
        fi.Delete();
    }

    public static string? GetShortFileNameWithoutExtension(this FileInfo? fi)
    {
        return fi?.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
    }

    public static string? GetShortFileNameWithoutExtension(this string? fi)
    {
        return fi is null
            ? null
            : new FileInfo(fi).GetShortFileNameWithoutExtension();
    }

    public static DirectoryInfo ScanSolutionDir(this Assembly assembly, string solutionName)
    {
        var a = SearchFoldersUntilFileExists(new FileInfo(assembly.Location).Directory);
        if (a is null)
            throw new FileNotFoundException($"File {solutionName} not found.");
        return a;

        DirectoryInfo? SearchFoldersUntilFileExists(DirectoryInfo? di)
        {
            while (di is not null)
            {
                if (!di.Exists)
                    return null;
                var fi = Path.Combine(di.FullName, solutionName);
                if (File.Exists(fi))
                    return di;
                di = di.Parent;
            }

            return null;
        }
    }
}
