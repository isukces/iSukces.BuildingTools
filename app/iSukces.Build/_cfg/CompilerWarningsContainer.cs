using System.Collections.Generic;
using System.Linq;

namespace iSukces.Build;

public class CompilerWarningsContainer
{
    public HashSet<string> Items { get; } = new HashSet<string>();

    public string Separator { get; set; } = ";";


    public string AsDotnetBuild
    {
        get
        {
            return string.Join(";", Items.OrderBy(a => a).Select(a =>
            {
                if (IsAnumber(a))
                {
                    a = a.PadLeft(4, '0');
                    return "CS" + a;
                }

                return a;
            }));
        }
    }

    static bool IsAnumber(string s)
    {
        if (string.IsNullOrEmpty(s))
            return false;
        foreach (var c in s)
            if (c is < '0' or > '9')
                return false;
        return true;
    }

    public override string ToString() =>
        string.Join(";", Items.OrderBy(a => a));

    public void Add(string s)
    {
        Items.Add(s);
    }
}
