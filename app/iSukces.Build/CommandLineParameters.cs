using System.Collections.Generic;

namespace iSukces.Build;

public class CommandLineParameters
{
    public void Add(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return;
        _list.Add(BuildUtils.Encode(s));
    }

    public string[] ToArray() => _list.ToArray();

    #region Fields

    private readonly List<string> _list = new();

    #endregion
}
