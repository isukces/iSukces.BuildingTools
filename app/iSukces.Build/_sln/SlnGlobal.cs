using System.Collections.Generic;

namespace iSukces.Build;

public class SlnGlobal : SlnBaseObject
{
    public SlnGlobal()
        : base("Global")
    {
    }

    public override string GetDeclarartionArgs()
    {
        return "";
    }


    public override IEnumerable<string> GetFileLinesInternal()
    {
        foreach (var section in Sections)
        {
            foreach (var i in section.GetFileLines())
            {
                yield return i;
            }
        }
    }

    public List<SlnGlobalSection> Sections { get; } = new();
}
