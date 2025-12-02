using System.Collections.Generic;

namespace iSukces.Build;

public class CommandLineParameters
{
    public void Add(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return;
        _list.Add(s.CliQuoteIfNecessary());
    }

    public string[] ToArray() => _list.ToArray();

    private readonly List<string> _list = new();
}
