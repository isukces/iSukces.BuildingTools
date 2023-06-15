using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace iSukces.Build;

public sealed class CompilerDirectiveUpdater
{
    public CompilerDirectiveUpdater(CommandLine cl, DirectoryInfo solutionDir)
    {
        _cl          = cl;
        _solutionDir = solutionDir;
    }

    private static bool FixDocumentation(XElement i, string path)
    {
        var ns                = i.Name.Namespace;
        var documentationFile = i.Element(ns + "DocumentationFile");
        if (!string.IsNullOrEmpty(documentationFile?.Value)) return false;

        {
            var q = ((string)i.Attribute("Condition"))?.Trim();
            if ( /*!string.IsNullOrEmpty(q) && */ q != "'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'")
                return false;
        }
        var op = i.Element(ns + "OutputPath")?.Value;
        if (string.IsNullOrEmpty(op))
            return false;
        if (documentationFile == null)
        {
            documentationFile = new XElement(ns + "DocumentationFile");
            i.Add(documentationFile);
        }

        documentationFile.Value = op + path;
        return true;
    }

    private static bool HasAmmy(XElement root)
    {
        foreach (var i in root.Elements(root.Name.Namespace + "ItemGroup"))
        {
            foreach (var j in i.Elements(root.Name.Namespace + "None"))
            {
                var file = (string)j.Attribute("Include");
                if (file != null)
                    if (file.ToLower().EndsWith(".ammy", StringComparison.OrdinalIgnoreCase))
                        return true;
            }
        }

        return false;
    }

    public void Modify()
    {
        Modify(_solutionDir, 0);
    }

    private void Modify(DirectoryInfo dir, int level)
    {
        if (string.Equals(dir.Name, "bin", StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(dir.Name, "obj", StringComparison.CurrentCultureIgnoreCase))
            return;
        foreach (var fi in dir.GetFiles("*.csproj"))
            Update(fi, true);
        if (level == 0)
            foreach (var fi in dir.GetFiles("Directory.Build.props"))
                Update(fi, false);
        foreach (var i in dir.GetDirectories()) Modify(i, level + 1);
    }

    private void Update(FileInfo fi, bool isCsProj)
    {
        var xml  = XDocument.Load(fi.FullName);
        var root = xml.Root;
        if (root == null)
            return;
        var hasAmmy = HasAmmy(root);
        var save    = false;
        if (!isCsProj)
        {
            const string expect = "$(SolutionDir)PdDefines.targets";
            foreach (var i in root.Elements(root.Name.Namespace + "Import"))
            {
                var proj = (string)i.Attribute("Project");
                if (proj != expect) continue;
                i.Remove();
                save = true;
            }
        }

        var skip = string.Equals(fi.Name, "Pd.Tools.ProjectManager", StringComparison.CurrentCultureIgnoreCase);

        foreach (var i in root.Elements(root.Name.Namespace + "PropertyGroup"))
        {
            if (_cl.Directives.Any() && !skip)
                foreach (var j in i.Elements(root.Name.Namespace + "DefineConstants"))
                {
                    var x    = j.Value;
                    var list = x.Split(';', ' ').Where(a => a != "").ToHashSet();
                    UpdateDefineConstants(list, hasAmmy, isCsProj);
                    var y = string.Join(";", list.OrderBy(a => a, new CompilerDirectiveComparer()));
                    if (x == y)
                        continue;
                    Console.WriteLine(fi.Name + " " + x + " => " + y);
                    j.Value = y;
                    save    = true;
                }

            var n = fi.GetShortFileNameWithoutExtension() + ".xml";
            if (FixDocumentation(i, n))
                save = true;
        }

        if (save)
            xml.Save(fi.FullName);
    }

    private void UpdateDefineConstants(HashSet<string> list, bool hasAmmy, bool isCsProj)
    {
        const string noAmmyUpdate = "NO_AMMY_UPDATE";
        foreach (var i in _cl.Directives)
            list.Set(i.Key, !isCsProj && i.Value);

        if (!hasAmmy)
            list.Set(noAmmyUpdate, false);
        list.Add("$(DefineConstants)");
        //list = list.Where(a => !a.Contains(noAmmyUpdate)).ToHashSet();
    }

    private readonly CommandLine _cl;
    private readonly DirectoryInfo _solutionDir;
}
