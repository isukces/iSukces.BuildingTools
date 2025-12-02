using System.Collections.Generic;
using System.Linq;

namespace iSukces.Build;

public class SlnProject
{
    public string GetFirstLine()
    {
        var args = new List<string>
        {
            Name,
            File,
            ProjectUid.ToString()
        };
        args.AddRange(OtherArguments);

        var def = string.Join(", ", args.Select(a => a.CliQuote()));
        var id  = Kind.ToString().CliQuote();
        return $"Project({id}) = {def}";
    }

    public SlnProject WithDef(List<string> parseArgs)
    {
        Name           = parseArgs[0];
        File           = parseArgs[1];
        ProjectUid     = new SlnProjectId(parseArgs[2]);
        OtherArguments = parseArgs.Skip(3).ToArray();
        return this;
    }

    public bool                  IsFolder       => Kind == SlnProjectId.Folder;
    public SlnProjectId          Kind           { get; set; }
    public IReadOnlyList<string> OtherArguments { get; private set; }
    public List<string>          Lines          { get; } = new();


    public string       Name       { get; set; }
    public string       File       { get; set; }
    public SlnProjectId ProjectUid { get; set; }
}
