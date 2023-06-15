using System.Collections.Generic;
using System.Linq;

namespace iSukces.Build;

public class SlnGlobalSection : SlnBaseObject
{
    public SlnGlobalSection()
        : base("GlobalSection")
    {
    }

    public override string GetDeclarartionArgs()
    {
        return $"({Name}) = {Value}";
    }

    public override IEnumerable<string> GetFileLinesInternal()
    {
        var els = Items.Select(a => a.GetFileLine()).ToArray();
        return els;
    }

    public List<SlnGlobalSectionItem> Items { get; } = new();
    public string                     Name  { get; set; }
    public string                     Value { get; set; }

    public GlobalSectionType SectionType
    {
        get
        {
            return Name switch
            {
                "NestedProjects" => GlobalSectionType.NestedProjects,
                "ExtensibilityGlobals" => GlobalSectionType.ExtensibilityGlobals,
                "SolutionProperties" => GlobalSectionType.SolutionProperties,
                "ProjectConfigurationPlatforms" => GlobalSectionType.ProjectConfigurationPlatforms,
                "SolutionConfigurationPlatforms" => GlobalSectionType.SolutionConfigurationPlatforms,
                _ => GlobalSectionType.Unknown
            };
        }
    }
}

public enum GlobalSectionType
{
    Unknown,
    NestedProjects,
    ExtensibilityGlobals,
    SolutionProperties,
    ProjectConfigurationPlatforms,
    SolutionConfigurationPlatforms
}
