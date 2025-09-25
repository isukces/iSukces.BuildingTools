using System;
using System.Text;

namespace iSukces.Build.InnoSetup;

public sealed partial class InstallDeleteCommand : Command
{
    public static InstallDeleteCommand Parse(string s)
    {
        return Parser.ParseAll(s);
    }

    public bool MatchFile(string fn)
    {
        return Type == "files" && string.Equals(Name, fn, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        AddCommand("Type", Type);
        AddCommand("Name", Name, true);
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

    public string Type { get; set; }
    public string Name { get; set; }

    [Flags]
    public enum FileFlags
    {
        None = 0,
        ReplaceSameversion = 1,
        IgnoreVersion = 2,
        DontCopy = 4,
        NoEncryption = 8,
        OnlyIfDoesntExist = 16
    }
}
