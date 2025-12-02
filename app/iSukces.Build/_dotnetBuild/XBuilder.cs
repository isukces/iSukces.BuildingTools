using System.Collections.Generic;

namespace iSukces.Build;

public sealed class XBuilder : List<string>
{
    public void Add2(bool flag, string value)
    {
        if (flag)
            Add(value);
    }

    public void AddNotNull(string name, bool? value)
    {
        if (value is not null)
            Add(name + value);
    }

    public void AddNotNull(string name, int? value)
    {
        if (value is not null)
            Add(name + value);
    }

    public void Add(string name, string? value, bool doubleDot = false)
    {
        if (string.IsNullOrEmpty(value)) return;
        value = value.CliQuoteIfNecessary();
        if (doubleDot)
            Add(name + ":" + value);
        else
        {
            Add(name);
            Add(value);
        }
    }
}
