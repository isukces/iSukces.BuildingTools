using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace iSukces.Build;

public class SlnFilePrepare
{
    public Remover GetRemover()
    {
        return new Remover(Solution);
    }

    public void Load()
    {
        Solution = SlnFile.Load(Path.Combine(SlnDir.FullName, SolutionName));
    }


    public void Save()
    {
        Solution.Save(Path.Combine(SlnDir.FullName, OutputSolution));
    }

    public Assembly SlnSearchAssembly { get; set; }

    public DirectoryInfo SlnDir
    {
        get { return _slnDir ??= SlnSearchAssembly.ScanSolutionDir(SolutionName); }
    }

    public string SolutionName   { get; set; }
    public string OutputSolution { get; set; }

    public SlnFile Solution { get; private set; }

    private DirectoryInfo _slnDir;


    public class Remover
    {
        public Remover(SlnFile slnFile)
        {
            _slnFile = slnFile;
        }

        public Remover EmptyFolders()
        {
            bool canExit = true;
            while (true)
            {
                var projects = _slnFile.Projects.ToArray();
                foreach (var project in projects)
                {
                    if (!project.IsFolder) continue;
                    var nestedProjects = _slnFile.FindNestedProjects(project);
                    if (nestedProjects.Count == 0)
                    {
                        Remove(project);
                        canExit = false;
                    }
                }

                if (canExit)
                    break;
                canExit = true;
            }

            return this;
        }

        public Remover FileStartsWith(string prefix)
        {
            RemoveProjects(proj =>
            {
                return proj.File.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            });
            return this;
        }

        public Remover Folder(string projectName)
        {
            var project = _slnFile.GetProjectByName(projectName);
            if (project is null)
                return this;
            return Folder(project);
        }

        public Remover Folder(SlnProject folder)
        {
            if (!folder.IsFolder)
                return null;
            var nestedProjects = _slnFile.FindNestedProjects(folder);
            if (nestedProjects.Count > 0)
            {
                foreach (var i in nestedProjects)
                {
                    var nestedProject = _slnFile.GetProjectById(i);
                    if (nestedProject is null)
                        continue;
                    if (nestedProject.IsFolder)
                        Folder(nestedProject);
                    else
                        RemoveProjects(a => a.ProjectUid == nestedProject.ProjectUid);
                }
            }

            Remove(folder);
            return this;
        }

        public Remover Folders(params string[] projectNames)
        {
            foreach (var i in projectNames)
                Folder(i);
            return this;
        }

        public Remover Names(params string[] projects)
        {
            var toRemove = projects.ToHashSet(StringComparer.OrdinalIgnoreCase);
            RemoveProjects(p =>
            {
                return toRemove.Contains(p.Name);
            });
            return this;
        }

        public Remover NameStartsWith(string prefix)
        {
            RemoveProjects(proj =>
            {
                return proj.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            });
            return this;
        }

        public Remover Remove(SlnProject project)
        {
            return RemoveProjects(a => a.ProjectUid == project.ProjectUid);
        }

        private Remover RemoveProjects(Predicate<SlnProject> predicate)
        {
            var projects = _slnFile.Projects.ToArray();
            foreach (var project in projects)
            {
                if (predicate(project))
                    _slnFile.Remove(project);
            }

            return this;
        }

        private readonly SlnFile _slnFile;
    }
}
