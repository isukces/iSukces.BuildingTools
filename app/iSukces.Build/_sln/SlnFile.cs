using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iSukces.Build;

public class SlnFile
{
    public static SlnFile Load(string fileName)
    {
        var lines = File.ReadLines(fileName);

        return new SlnFileParser().Parse(lines);
    }

    public HashSet<SlnProjectId> FindNestedProjects(SlnProject folder)
    {
        HashSet<SlnProjectId> result = new();
        foreach (var i in Global.Sections)
        {
            if (i.SectionType == GlobalSectionType.NestedProjects)
            {
                foreach (var j in i.Items)
                {
                    var value = new SlnProjectId(j.Value);
                    if (value == folder.ProjectUid)
                    {
                        var key = new SlnProjectId(j.Key);
                        result.Add(key);
                    }
                }
            }
        }

        return result;
    }

    public SlnProject? GetProjectById(SlnProjectId id)
    {
        return Projects
            .FirstOrDefault(a => a.ProjectUid == id);
    }

    public SlnProject? GetProjectByName(string name)
    {
        return Projects
            .FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public void Remove(SlnProject slnProject)
    {
        Projects.Remove(slnProject);
        var serach   = slnProject.ProjectUid.ToString();
        var sections = Global.Sections;
        for (var sectionIdx = sections.Count - 1; sectionIdx >= 0; sectionIdx--)
        {
            var section = sections[sectionIdx];
            for (var itemIdx = section.Items.Count - 1; itemIdx >= 0; itemIdx--)
            {
                var i = section.Items[itemIdx];
                if (ShouldRemove(i, serach, section))
                    section.Items.RemoveAt(itemIdx);
            }

            if (section.Items.Count == 0)
                sections.RemoveAt(sectionIdx);
        }

        static bool ShouldRemove(SlnGlobalSectionItem item, string serach, SlnGlobalSection section)
        {
            if (item.Key.Contains(serach))
                return true;
            if (section.SectionType == GlobalSectionType.NestedProjects)
                if (item.Value.Contains(serach))
                {
                    return true;
                }

            return false;
        }
    }


    public void Save(string file)
    {
        var lines = new List<string>();
        lines.AddRange(Header);
        foreach (var i in Projects)
        {
            lines.Add(i.GetFirstLine());
            lines.AddRange(i.Lines);
            lines.Add("EndProject");
        }

        lines.AddRange(Global.GetFileLines());
        File.WriteAllLines(file, lines);
    }

    public List<string>     Header   { get; } = new();
    public List<SlnProject> Projects { get; } = new();
    public SlnGlobal        Global   { get; } = new();
}
