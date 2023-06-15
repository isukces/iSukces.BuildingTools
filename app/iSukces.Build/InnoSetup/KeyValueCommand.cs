using System.Text.RegularExpressions;

namespace iSukces.Build.InnoSetup;

public sealed class KeyValueCommand : Command
{
    // parse string like Key=Value using regex ignore spaces
    public static KeyValueCommand Parse(string s)
    {
        var m = Filter.Match(s);
        if (!m.Success)
            return null;
        return new KeyValueCommand
        {
            Key   = m.Groups["key"].Value.Trim(),
            Value = m.Groups["value"].Value.Trim()
        };
    }

    public override string ToString()
    {
        return Key + "=" + Value;
    }

    public string Key   { get; set; }
    public string Value { get; set; }

    private static readonly Regex
        Filter = new Regex(@"^\s*(?<key>[^=]+)\s*=\s*(?<value>.*)\s*$", RegexOptions.Compiled);
}
