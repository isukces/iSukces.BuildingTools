using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace iSukces.Build;

internal class SlnFileParser
{
    public static List<string> ParseArgs(string x)
    {
        List<string> result = new List<string>();
        var          a1     = ArgParseStatus.WaitingForText;
        var          sb     = new StringBuilder();
        foreach (var i in x)
        {
            if (a1 == ArgParseStatus.WaitingForText)
            {
                if (i == ' ') continue;
                if (i == '"')
                {
                    a1 = ArgParseStatus.InText;
                    sb.Clear();
                    continue;
                }

                throw new NotSupportedException();
            }

            if (a1 == ArgParseStatus.InText)
            {
                if (i == '"')
                {
                    a1 = ArgParseStatus.AfterText;
                    result.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }

                sb.Append(i);
                continue;
            }

            if (a1 == ArgParseStatus.AfterText)
            {
                if (i == ' ') continue;
                if (i == ',')
                {
                    a1 = ArgParseStatus.WaitingForText;
                    continue;
                }

                throw new NotSupportedException();
            }

            throw new NotSupportedException();
        }

        return result;
    }

    public SlnFile Parse(IEnumerable<string> lines)
    {
        SlnFile result = new();
        status = SlnParseState.Before;

        SlnProject slnProject = null;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (status == SlnParseState.Before)
            {
                if (line == "Global")
                {
                    status = SlnParseState.Global;
                    continue;
                }

                var m = ProjectRegex.Match(line);
                if (m.Success)
                {
                    slnProject = new SlnProject
                    {
                        Kind = new SlnProjectId(m.Groups[1].Value),
                    }.WithDef(ParseArgs(m.Groups[2].Value));
                    result.Projects.Add(slnProject);
                    status = SlnParseState.Project;
                    continue;
                }

                result.Header.Add(line);
                continue;
            }

            if (status == SlnParseState.Project)
            {
                if (line.Trim() == "EndProject")
                {
                    status = SlnParseState.Before;
                    continue;
                }

                slnProject.Lines.Add(line);
                continue;
            }

            if (status == SlnParseState.Global)
            {
                var m = GlobalSectionBeginRegex.Match(line);
                if (m.Success)
                {
                    var section = new SlnGlobalSection
                    {
                        Name  = m.Groups[1].Value,
                        Value = m.Groups[2].Value
                    };
                    result.Global.Sections.Add(section);
                    status = SlnParseState.GlobalSection;
                    continue;
                }

                if (line.Trim() == "EndGlobal")
                {
                    status = SlnParseState.Before;
                    continue;
                }

                throw new NotImplementedException();
            }

            if (status == SlnParseState.GlobalSection)
            {
                if (line.Trim() == "EndGlobalSection")
                {
                    status = SlnParseState.Global;
                    continue;
                }

                var section = result.Global.Sections.Last();
                var item    = SlnGlobalSectionItem.Parse(line);
                section.Items.Add(item);
                continue;
            }

            throw new NotImplementedException();
        }

        if (status != SlnParseState.Before)
            throw new NotSupportedException();
        return result;
    }

    const string GlobalSectionBeginFilter = @"GlobalSection\(([^)]*)\)\s*=\s*(.*)";

    private const string ProjectFilter = @"Project\(""{([^\}]+)}""\)\s*=\s*(.*)$";

    static readonly Regex GlobalSectionBeginRegex =
        new Regex(GlobalSectionBeginFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ProjectRegex =
        new Regex(ProjectFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private SlnParseState status;

    private enum ArgParseStatus
    {
        WaitingForText,
        InText,
        AfterText,
    }

    private enum SlnParseState
    {
        Before,
        Project,
        Global,
        GlobalSection
    }
}
