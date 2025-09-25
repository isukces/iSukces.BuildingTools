using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace iSukces.Build.InnoSetup;

public sealed class InnoSetupFileBuilder
{
    public InnoSetupFileBuilder(InnoSetupFile setupFile)
    {
        SetupFile = setupFile;
    }

    static string AddApp(string a)
    {
        var tmp = "{app}\\" + a;
        tmp = tmp.TrimEnd('\\');
        return tmp;
    }


    public static string GetRelativeFilePath(string fn, string relativeToFile)
    {
        if (string.IsNullOrEmpty(relativeToFile))
            return fn;
        var a = Path.GetDirectoryName(relativeToFile);
        if (string.IsNullOrEmpty(a))
            return fn;
        var b = Path.GetDirectoryName(fn);
        if (string.IsNullOrEmpty(b))
            return fn;
        var c = GetRelativePath(a, b);
        return Path.Combine(c, Path.GetFileName(fn));
    }

    private static string GetRelativePath(string a, string b)
    {
        a = a.Replace('/', '\\');
        b = b.Replace('/', '\\');
        var a1 = a.Split('\\');
        var b1 = b.Split('\\');
        var i  = 0;
        while (i < a1.Length && i < b1.Length && a1[i] == b1[i])
            i++;
        var sb = new StringBuilder();
        for (var j = i; j < a1.Length; j++)
        {
            if (sb.Length > 0)
                sb.Append('\\');
            sb.Append("..");
        }

        for (var j = i; j < b1.Length; j++)
        {
            if (sb.Length > 0)
                sb.Append('\\');
            sb.Append(b1[j]);
        }

        return sb.ToString();
    }

    public void Add(List<FileInfo> files, Func<FileInfo, FileCommand, FileCommand> action)
    {
        var s = SetupFile.GetOrCreateSection("Files");

        foreach (var fileInfo in files)
        {
            var command = new FileCommand
            {
                Source  = GetRelativeFilePath(fileInfo.FullName, RelativeToFile),
                DestDir = AddApp(GetRelativePath(BinaryDir, fileInfo.Directory.FullName))
            };
            command = action(fileInfo, command);
            if (command is not null)
                s.Commands.Add(command);
        }
    }


    public void AddNotNeccesaryFiles(string publishOutputDir, string installationFolder, Func<FileInfo, bool> action)
    {
        var a = new DirectoryInfo(publishOutputDir);
        var b = new DirectoryInfo(installationFolder);

        Compare(a, b);
        return;

        void Compare(DirectoryInfo pub, DirectoryInfo inst)
        {
            if (!inst.Exists)
                return;

            foreach (var instNested in inst.GetDirectories())
            {
                var pubNested = new DirectoryInfo(Path.Combine(pub.FullName, instNested.Name));
                Compare(pubNested, instNested);
            }

            foreach (var instFile in inst.GetFiles("*.*"))
            {
                var pubFile = new FileInfo(Path.Combine(pub.FullName, instFile.Name));
                if (pubFile.Exists) continue;
                if (action(instFile)) continue;

                var fn = GetRelativeFilePath(instFile.FullName, Path.Combine(installationFolder, "anyFile.txt"));
                fn = AddApp(fn);
                var s   = SetupFile.GetOrCreateSection("InstallDelete");
                var cmd = s.Commands.OfType<InstallDeleteCommand>().FirstOrDefault(a => a.MatchFile(fn));
                if (cmd is null)
                {
                    cmd = new InstallDeleteCommand { Type = "files", Name = fn };
                    s.Commands.Add(cmd);
                }
            }
        }
    }

    public void DeleteAllFiles()
    {
        var s = SetupFile.GetOrCreateSection("Files");
        foreach (var command in s.Commands.OfType<FileCommand>().ToArray())
            s.Commands.Remove(command);
    }

    public InnoSetupFile SetupFile      { get; }
    public string        RelativeToFile { get; set; }
    public string        BinaryDir      { get; set; }
}
