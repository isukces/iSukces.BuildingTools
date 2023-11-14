using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace iSukces.Build.InnoSetup;

public class InnoSetupFile
{
    public static InnoSetupFile Load(string fn, Encoding? encoding = null)
    {
        var b = File.ReadAllBytes(fn);
        var s = encoding?.GetString(b) ?? Encoding.UTF8.GetString(b);
        s = s.Replace("\r\n", "\n");
        var lines = s.Split('\n');
        return Parse(lines);
    }

    private static InnoSetupFile Parse(string[] lines)
    {
        var     a       = new InnoSetupFile();
        Section current = null;
        foreach (var i in lines)
        {
            var m = SectionRegex.Match(i);
            if (m.Success)
            {
                current = new Section { Name = m.Groups[1].Value.Trim() };
                a.Sections.Add(current);
                continue;
            }

            if (current == null)
            {
                current = new Section { Name = "" };
                a.Sections.Add(current);
            }

            current.Add(i);
        }

        return a;
    }

    public Section GetOrCreateSection(string name)
    {
        var tmp = Sections.FirstOrDefault(a => a.Name == name);
        if (tmp is null)
        {
            tmp = new Section { Name = name };
            Sections.Add(tmp);
        }

        return tmp;
    }

    public void Save(string fileName, Encoding encoding)
    {
        var sb = new StringBuilder();
        foreach (var section in Sections)
        {
            if (sb.Length > 0)
                sb.AppendLine();
            if (!string.IsNullOrEmpty(section.Name))
                sb.AppendLine("[" + section.Name + "]");

            var lines       = section.Commands.Select(a => a.ToString()).ToList();
            var acceptEmpty = false;
            for (var index = lines.Count - 1; index >= 0; index--)
            {
                var line = lines[index];
                if (string.IsNullOrEmpty(line))
                {
                    if (!acceptEmpty)
                        lines.RemoveAt(index);
                    acceptEmpty = false;
                    continue;
                }

                acceptEmpty = true;
            }

            acceptEmpty = false;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) && !acceptEmpty)
                    continue;
                acceptEmpty = true;
                sb.AppendLine(line);
            }
        }

        var s = sb.ToString();
        File.WriteAllText(fileName, s, encoding);
    }

    public string SetVersionsAndOutputBaseFilename(string version, string prefix, string outFolder, string versionSeparator = "_")
    {
        this["Setup", "VersionInfoVersion"]        = version;
        this["Setup", "VersionInfoProductVersion"] = version;
        this["Setup", "AppVersion"]                = version;
        var version2 = versionSeparator == "." ? version : version.Replace(".", versionSeparator);
        var output   = prefix + version2;
        this["Setup", "OutputBaseFilename"] = output;
        this["Setup", "OutputDir"]          = outFolder;
        return Path.Combine(outFolder, output + ".exe");
    }

    public List<Section> Sections { get; } = new();

    public string this[string section, string name]
    {
        get
        {
            var f = Sections.FirstOrDefault(a => a.Name == section);
            if (f is null)
                return null;
            return f[name];
        }
        set
        {
            var f = GetOrCreateSection(section);
            f[name] = value;
        }
    }

    private const string SectionFilter = @"^\s*\[([^]]+)]\s*$";

    private static readonly Regex SectionRegex =
        new Regex(SectionFilter, RegexOptions.Multiline | RegexOptions.Compiled);

    public sealed class Section
    {
        public void Add(string s)
        {
            var command = Command.Parse(s, this);
            Commands.Add(command);
        }

        public override string ToString()
        {
            return "Section " + Name;
        }

        public string        Name     { get; set; }
        public List<Command> Commands { get; } = new();

        public string this[string name]
        {
            get
            {
                var tmp = Commands.OfType<KeyValueCommand>().FirstOrDefault(a => a.Key == name);
                if (tmp is null)
                    return null;
                return tmp.Value;
            }
            set
            {
                var tmp = Commands.OfType<KeyValueCommand>().FirstOrDefault(a => a.Key == name);
                if (tmp is null)
                {
                    tmp = new KeyValueCommand { Key = name };
                    Commands.Add(tmp);
                }
                else
                    tmp.Value = value;
            }
        }
    }

    // a method that compares a directory with another directory and returns a list of files that are not in the first directory
}
