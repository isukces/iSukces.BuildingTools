using System.Collections.Generic;

namespace iSukces.Build;

public abstract class SlnBaseObject
{
    protected SlnBaseObject(string elementName)
    {
        ElementName = elementName;
    }

    public abstract string GetDeclarartionArgs();

    public IEnumerable<string> GetFileLines()
    {
        var header = ElementName + GetDeclarartionArgs();
        yield return header;
        foreach (var i in GetFileLinesInternal())
        {
            yield return "\t" + i;
        }

        yield return "End" + ElementName;
    }

    public abstract IEnumerable<string> GetFileLinesInternal();

    public string ElementName { get; }
}
