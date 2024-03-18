using System;
using System.Linq;
using System.Xml.Linq;

namespace iSukces.Build;

public class CsProjFile
{
    private CsProjFile(XDocument doc) => Doc = doc;


    public static int GetDayNumber()
    {
        var d = DateTime.Now.Date;

        return d.Month * 100 + d.Day;
        /*

        var first = new DateTime(d.Year, 1, 1);
        return (d.Year - 2000) * 1000 + (int)d.Subtract(first).TotalDays + 1;
        */
    }

    public static CsProjFile Load(string iFullName) => new(XDocument.Load(iFullName));


    public static string UpdateVersion(string? x)
    {
        x = x?.Trim();
        if (string.IsNullOrEmpty(x))
            x = "1.0.0.0";
        var numbers = (x + ".0.0.0.0").Split('.')
            .Select(q => int.TryParse(q.Trim(), out var nr1) ? nr1 : 0)
            .Take(4).ToArray();
        numbers[1] = DateTime.Now.Year - 2000;
        numbers[2] = GetDayNumber();
        numbers[3] = (numbers[3] + 1) % 65536;
        return string.Join(".", numbers);
    }

    public string GetVersion(string name)
    {
        var propertyGroups = Doc.Root.Descendants("PropertyGroup").ToArray();
        foreach (var i in propertyGroups)
        {
            var version = i.Element(name);
            if (version is null)
                continue;
            return version.Value.Trim();
        }

        return null;
    }

    public void Save(string fileName)
    {
        Doc.Save(fileName);
    }

    public void SetAllVersions(string version)
    {
        SetProperty("AssemblyVersion", version);
        SetProperty("FileVersion", version);
        SetProperty("AssemblyFileVersion", version);
        SetProperty("Version", version);
    }

    public void SetProperty(string name, string version)
    {
        var propertyGroups = Doc.Root.Descendants("PropertyGroup").ToArray();
        var isSet          = false;
        foreach (var node in propertyGroups)
        {
            var el = node.Element(name);
            if (el == null) continue;
            el.Value = version;
            isSet    = true;
        }

        if (isSet)
            return;
        var pNode = Doc.Root.Element("PropertyGroup");
        if (pNode == null)
        {
            pNode = new XElement("PropertyGroup");
            Doc.Root.Add(pNode);
        }

        pNode.Add(new XElement(name, version));
    }

    #region Properties

    public XDocument Doc { get; }

    #endregion
}
