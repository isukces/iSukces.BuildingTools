using System;
using System.Collections.Generic;
using System.Text;

namespace iSukces.Build.InnoSetup;

public sealed partial class FileCommand : Command
{
    public static FileCommand Parse(string s)
    {
        return Parser.ParseAll(s);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        AddCommand("Source", Source, true);
        if ((Flags & FileFlags.DontCopy) == 0)
            AddCommand("DestDir", DestDir, true);
        if (Flags != FileFlags.None)
        {
            var l = new List<string>();
            var flagsArray = new[]
            {
                FileFlags.IgnoreVersion,
                FileFlags.ReplaceSameversion,
                FileFlags.DontCopy,
                FileFlags.NoEncryption,
                FileFlags.OnlyIfdoesntExist
            };
            foreach (var i in flagsArray)
            {
                if (Flags.HasFlag(i)) l.Add(i.ToString().ToLower());
            }

            AddCommand("Flags", string.Join(" ", l));
        }

        var code = sb.ToString();
        code = code.TrimEnd(';', ' ');
        return code;

        void AddCommand(string name, string value, bool quote = false)
        {
            if (string.IsNullOrEmpty(value))
                return;
            sb.Append(name);
            sb.Append(": ");
            if (quote)
                value = $"\"{value}\"";
            sb.Append(value);
            sb.Append("; ");
        }
    }

    public string Source { get; set; }

    public FileFlags Flags { get; set; }

    public string DestDir { get; set; }

    [Flags]
    public enum FileFlags
    {
        None = 0,
        ReplaceSameversion = 1,
        IgnoreVersion = 2,
        DontCopy = 4,
        NoEncryption = 8,
        OnlyIfdoesntExist = 16
    }
}
