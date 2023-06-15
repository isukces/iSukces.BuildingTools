using System;

namespace iSukces.Build;

public class SlnGlobalSectionItem
{
    public static SlnGlobalSectionItem Parse(string line)
    {
        var tmp = line.Split('=');
        if (tmp.Length != 2)
            throw new NotSupportedException();
        return new SlnGlobalSectionItem
        {
            Key   = tmp[0].Trim(),
            Value = tmp[1].Trim()
        };
    }

    public string GetFileLine()
    {
        return $"{Key} = {Value}";
    }

    public string Value { get; set; }

    public string Key { get; set; }
}
