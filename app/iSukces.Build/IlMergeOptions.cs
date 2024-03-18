using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace iSukces.Build;

public class IlMergeOptions
{
    private static string Encode(string value)
    {
        if (value.Contains(' '))
            value = "\"" + value + "\"";
        return value;
    }

    private static void SureEmptyDir(string targetDir)
    {
        var directoryInfo = new DirectoryInfo(targetDir);
        directoryInfo.Create();
        BuildUtils.Clear(directoryInfo);
    }

    private bool Accept(string name) => !Exclude.Contains(name);

    public void Add(string fileName)
    {
        Files.Add(fileName);
    }

    public string[] AddIlMergedBinaries(string mainExe, string dir)
    {
        Dir = dir;
        Add(mainExe);
        foreach (var i in new DirectoryInfo(dir).GetFiles("*.dll"))
        {
            var name   = i.Name;
            var accept = Accept(name);
            if (accept)
            {
                ExConsole.WriteLine("Add to IL merge {0}", new FileInfo(name));
                Add(name);
                foreach (var ui in BuildUtils.AssemblyRelatedFiles(name, false))
                {
                    var f = new FileInfo(Path.Combine(dir, ui));
                    if (f.Exists)
                        Remove.Add(ui);
                }
            }
            else
            {
                ExConsole.WriteLine("SKIP IL merge {0}", new FileInfo(name));
                TryReplace(name);
            }
        }

        return Files.Skip(1).ToArray();
    }

    public string GetCommandLine(string out1) => string.Join(" ", GetCommandLineArguments(out1));

    public string[] GetCommandLineArguments(string outputFilename, bool newLine = true, bool addIlMergeExe = true)
    {
        var l        = new List<string>();
        var nextLine = newLine ? "^\r\n   " : "";

        void AddParam(string name, string value, bool allowEmpty = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (allowEmpty)
                    l.Add($"{nextLine}/{name}");
                return;
            }

            value = Encode(value);
            l.Add($"{nextLine}/{name}:{value}");
        }

        if (addIlMergeExe)
            l.Add(Encode(new FileInfo(IlmergeExe).Name));

        AddParam("keyfile", KeyFile);

        AddParam("target", Target.ToString().ToLower());
        AddParam("targetplatform", TargetPlatform);

        if ((Flags & IlMergeFlags.Log) != 0)
            AddParam("log", LogFileName, true);
        if ((Flags & IlMergeFlags.Closed) != 0)
            l.Add("/closed");
        if ((Flags & IlMergeFlags.Internalize) != 0)
            AddParam("internalize", InternalizeExclude, true);

        AddParam("out", outputFilename);

        void AddFile(string fileName)
        {
            foreach (var i in _replaceFrom)
            {
                var fn = Path.Combine(i, fileName);
                if (File.Exists(fn))
                {
                    l.Add(Encode(fn));
                    return;
                }
            }

            l.Add(nextLine + Encode(fileName));
        }

        foreach (var i in Files)
        {
            AddFile(i);
        }

        return l.ToArray();
    }

    public IReadOnlyList<string> GetFilesToDelete() => Files.Concat(Remove).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

    public void Run()
    {
        ExeRunner.WorkingDir = Dir;

        const string tmpFolder = "__tmp";
        var          targetDir = Path.Combine(Dir, tmpFolder);
        SureEmptyDir(targetDir);
        try
        {
            {
                if (!string.IsNullOrEmpty(LogFileName))
                {
                    FileExtensions.DeleteFileAndLog(Path.Combine(Dir, LogFileName));
                }
            }
            var commandLineArguments = GetCommandLineArguments(Path.Combine(tmpFolder, OutputExe), false, false);
            var parameters           = string.Join(" ", commandLineArguments);
            ExeRunner.Execute(IlmergeExe, commandLineArguments);

            void Move(string file)
            {
                var src = Path.Combine(tmpFolder, file);
                ExConsole.WriteLine("Move {0} {1}", new FileInfo(src), new FileInfo(file));
                if (File.Exists(file))
                    File.Delete(file);
                File.Move(src, file);
            }

            foreach (var i in GetFilesToDelete())
                FileExtensions.DeleteFileAndLog(i);
            if ((Flags & IlMergeFlags.DeleteExcludedFiles) != 0)
                foreach (var i in Exclude.OrderBy(a => a))
                    FileExtensions.DeleteFileAndLog(i);

            Move(OutputExe);
            Move(BuildUtils.ChangeFileExtension(OutputExe, "pdb"));
        }
        finally
        {
            try
            {
                ExConsole.WriteLine("Delete folder {0}", targetDir);
                new DirectoryInfo(targetDir).Delete(true);
            }
            catch
            {
                ExConsole.WriteLine(ExConsole.Foreground(ConsoleColor.Red) + "Failed");
            }
        }
    }

    public void SaveBatch(string dir)
    {
        var sb = new StringBuilder();
        {
            var fi = new FileInfo(IlmergeExe);
            sb.AppendLine("@echo off");
            sb.AppendLine("PATH=%PATH%;" + Encode(fi.Directory.FullName));
        }

        foreach (var i in _replaceFiles)
            sb.AppendLine($"copy {Encode(i.Value)} {Encode(i.Key)} /y");

        sb.AppendLine("mkdir temp");
        var temporatyOutput = Path.Combine("temp", Files[0]);
        {
            sb.AppendLine(GetCommandLine(temporatyOutput));
        }
        sb.AppendLine("timeout 5 > NUL");
        {
            foreach (var i in Files)
                sb.AppendLine("DEL " + Encode(i));

            foreach (var i in Remove)
                sb.AppendLine("DEL " + Encode(i));
        }
        sb.AppendLine("move " + temporatyOutput + " " + Files.First());
        sb.AppendLine("move " + BuildUtils.ChangeFileExtension(temporatyOutput, "pdb") + " " +
                      BuildUtils.ChangeFileExtension(Files.First(), "pdb"));
        sb.AppendLine("timeout 3 > NUL");
        File.WriteAllText(Path.Combine(dir, "runIlMerge.bat"), sb.ToString());
    }

    public void TryReplace(string name)
    {
        foreach (var i in _replaceFrom)
        {
            var f = Path.Combine(i, name);
            if (File.Exists(f))
            {
                _replaceFiles[name] = f;
                return;
            }
        }
    }

    #region Properties

    public string InternalizeExclude { get; set; }

    public string OutputExe { get; set; }

    public HashSet<string> Exclude { get; set; }

    public string       InputExe       { get; set; }
    public string       TargetPlatform { get; set; }
    public string       IlmergeExe     { get; set; }
    public List<string> Remove         { get; } = new();

    /// <summary>
    ///     Star or comma separated types names
    /// </summary>
    public string AllowDup { get; set; }

    public string        LogFileName { get; set; }
    public IlMergeFlags  Flags       { get; set; }
    public IlMergeTarget Target      { get; set; }
    public string        KeyFile     { get; set; }

    #endregion

    #region Fields

    protected string Dir;

    protected readonly List<string> Files = new();
    private readonly Dictionary<string, string> _replaceFiles = new(StringComparer.InvariantCulture);
    private readonly List<string> _replaceFrom = new();

    #endregion
}
