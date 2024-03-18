namespace iSukces.Build;

public sealed class DirectorySynchronizeItem
{
    public DirectorySynchronizeItem(string folder, SyncFlags flags)
    {
        Folder = folder;
        Flags  = flags;
    }

    #region Properties

    public string    Folder { get; }
    public SyncFlags Flags  { get; }

    #endregion
}
