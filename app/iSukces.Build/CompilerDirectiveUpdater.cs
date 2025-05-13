using System.IO;

namespace iSukces.Build;

public sealed class CompilerDirectiveUpdater : CompilerDirectiveUpdaterBase
{
    public CompilerDirectiveUpdater(CommandLine cl, DirectoryInfo solutionDir)
    {
        _cl          = cl;
        _solutionDir = solutionDir;
    }

    public void Modify()
    {
        Modify(_solutionDir, 0, _cl);
    }

    private readonly CommandLine _cl;
    private readonly DirectoryInfo _solutionDir;
}
