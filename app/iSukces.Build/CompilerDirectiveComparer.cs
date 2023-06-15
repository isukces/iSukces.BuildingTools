using System;
using System.Collections.Generic;

namespace iSukces.Build;

internal sealed class CompilerDirectiveComparer : IComparer<string>
{
    public int Compare(string a, string b)
    {
        int Group(string x)
        {
            if (x.StartsWith("$(", StringComparison.Ordinal))
                return -1;
            switch (x)
            {
                case "DEBUG": return 0;
                case "TRACE": return 1;
                default:
                    return 999;
            }
        }

        var ga = Group(a);
        var gb = Group(b);
        var c  = ga.CompareTo(gb);
        if (c != 0)
            return c;
        var aa = a.TrimStart('_');
        var bb = b.TrimStart('_');
        c = aa.CompareTo(bb);
        if (c != 0)
            return c;
        return string.Compare(a, b, StringComparison.Ordinal);
    }
}
