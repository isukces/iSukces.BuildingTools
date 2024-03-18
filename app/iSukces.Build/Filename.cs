using System;

namespace iSukces.Build;

public class Filename
{
    public Filename(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public override string ToString()
    {
        return Name;
    }

    public string Name { get; }
}
