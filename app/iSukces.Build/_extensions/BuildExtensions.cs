using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace iSukces.Build;

public static class BuildExtensions
{
    public static DirectoryInfo CreateSubdirectory(this DirectoryInfo dir, params string[] paths)
    {
        var a = paths.ToList();
        a.Insert(0, dir.FullName);

        var path = Path.Combine(a.ToArray());
        return new DirectoryInfo(path);
    }


    public static void Set(this HashSet<string> list, string compilerConstant, bool add)
    {
        if (add)
        {
            list.Add(compilerConstant);
            // list.Remove("_" + compilerConstant);
        }
        else
        {
            //list.Add("_" + compilerConstant);
            list.Remove(compilerConstant);
        }
    }

    public static string ToInv(this int x)
    {
        return x.ToString(CultureInfo.InvariantCulture);
    }
}
