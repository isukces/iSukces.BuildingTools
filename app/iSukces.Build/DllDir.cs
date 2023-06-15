using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iSukces.Build;

public sealed class DllDir
{
    public DllDir(string folder)
    {
        Folder = folder;
    }

    public static StringList operator +(DllDir a, DllDir b)
    {
        var items = a.Dlls.Concat(b.Dlls).ToList();
        return new StringList(items);
    }

    public override string ToString()
    {
        return Folder;
    }

    public string Folder { get; }

    public HashSet<string> Dlls => new DirectoryInfo(Folder).GetFiles("*.dll")
        .Select(a => a.Name)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
}

public sealed class StringList
{
    public StringList(List<string> items)
    {
        Items = items;
    }

    public static StringList operator +(StringList a, string x)
    {
        var items = a.Items.ToList();
        items.Add(x);
        return new StringList(items);
    }

    public static StringList operator +(StringList a, string[] x)
    {
        var items = a.Items.ToList();
        items.AddRange(x);
        return new StringList(items);
    }

    public static implicit operator List<string>(StringList x)
    {
        return x.Items;
    }


    public HashSet<string> ToHashSet()
    {
        return Items.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public List<string> Items { get; }
}
