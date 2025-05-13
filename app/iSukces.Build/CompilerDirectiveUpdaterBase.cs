using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace iSukces.Build;

public class CompilerDirectiveUpdaterBase
{
    private static bool FixDocumentation(XElement i, string path)
    {
        var ns                = i.Name.Namespace;
        var documentationFile = i.Element(ns + "DocumentationFile");
        if (!string.IsNullOrEmpty(documentationFile?.Value)) return false;

        {
            var q = ((string?)i.Attribute("Condition"))?.Trim();
            if (q != "'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'")
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
    
    public event EventHandler<BeforeSaveEventArgs>? OnBeforeSave;

    protected void Modify(DirectoryInfo dir, int level, CommandLine commandLine)
    {
        if (string.Equals(dir.Name, "bin", StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(dir.Name, "obj", StringComparison.CurrentCultureIgnoreCase))
            return;
        foreach (var fi in dir.GetFiles("*.csproj"))
            UpdateOneFile(fi, true, commandLine);
        if (level == 0)
            foreach (var fi in dir.GetFiles("Directory.Build.props"))
                UpdateOneFile(fi, false, commandLine);
        foreach (var i in dir.GetDirectories())
            Modify(i, level + 1, commandLine);
    }

    public static void UpdateDefineConstants(HashSet<string> list, bool isCsProj, CommandLine commandLine)
    {
        foreach (var i in commandLine.Directives)
            list.Set(i.Key, !isCsProj && i.Value);
        if (list.Count > 0)
            list.Add(DefineConstantsExpression);
    }

    protected void UpdateOneFile(FileInfo fi, bool isCsProj, CommandLine commandLine)
    {
        var xml  = XDocument.Load(fi.FullName);
        var root = xml.Root;
        if (root == null)
            return;
        var save = false;
        if (!isCsProj)
        {
            const string expect = "$(SolutionDir)PdDefines.targets";
            foreach (var i in root.Elements(root.Name.Namespace + "Import"))
            {
                var proj = (string?)i.Attribute("Project");
                if (proj != expect) continue;
                i.Remove();
                save = true;
            }
        }

        var skip = string.Equals(fi.Name, "Pd.Tools.ProjectManager", StringComparison.CurrentCultureIgnoreCase);

        foreach (var i in root.Elements(root.Name.Namespace + "PropertyGroup"))
        {
            OnePropertyGroup(i);
            var n = fi.GetShortFileNameWithoutExtension() + ".xml";
            if (FixDocumentation(i, n))
                save = true;
        }

        if (save)
        {
            var args = new BeforeSaveEventArgs(fi.FullName);
            OnBeforeSave?.Invoke(null, args);
            xml.Save(fi.FullName);
        }

        return;

        void OnePropertyGroup(XElement propGroup)
        {
            if (commandLine.Directives.Count == 0 || skip) return;
            var nameDefineConstants = root.Name.Namespace + "DefineConstants";
            var defineConstantsNode = propGroup.Elements(nameDefineConstants).ToArray();
            if (!isCsProj)
            {
                //Console.WriteLine(fi.FullName+" "+defineConstantsNode.Length);
            }

            if (isCsProj)
            {
                foreach (var j in defineConstantsNode)
                {
                    var x    = j.Value;
                    var list = x.Split(';', ' ').Where(a => a != "").ToHashSet();
                    UpdateDefineConstants(list, isCsProj, commandLine);
                    var y = string.Join(";", list.OrderBy(a => a, new CompilerDirectiveComparer()));
                    if (x == y)
                        continue;
                    Console.WriteLine(fi.Name + " " + x + " => " + y);
                    j.Value = y;
                    save    = true;
                }
            }
            else
            {
                var             compare = propGroup.ToString();
                HashSet<string> hashSet = new();
                foreach (var j in defineConstantsNode)
                {
                    var x = j.Value;
                    // Console.WriteLine($"z1 = '{x}'");
                    foreach (var i in x.Split(';', ' '))
                        hashSet.Add(i.Trim());
                    j.Remove();
                }

                UpdateDefineConstants(hashSet, isCsProj, commandLine);
                hashSet.Remove(DefineConstantsExpression);
                hashSet.Remove("");
                // Console.WriteLine("z2 = " + string.Join(";", hashSet));
                foreach (var directive in hashSet.OrderBy(a => a, new CompilerDirectiveComparer()))
                {
                    var value    = DefineConstantsExpression + ";" + directive;
                    var xElement = new XElement(nameDefineConstants, value);
                    propGroup.Add(xElement);
                }

                var compare2 = propGroup.ToString();
                if (compare2 != compare)
                    save = true;
            }
        }
    }

    private const string DefineConstantsExpression = "$(DefineConstants)";
}

public class BeforeSaveEventArgs(string fileName)
{
    public string FileName { get; } = fileName;
}
