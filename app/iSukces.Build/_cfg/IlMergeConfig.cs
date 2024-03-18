using System;

namespace iSukces.Build;

public struct IlMergeConfig
{
    /// <summary>
    ///     Relative to solution dir
    /// </summary>
    public string IlMergeExe { get; set; }

    public StringList    Exclude            { get; set; }
    public string?       OutputExe          { get; set; }
    public string        LogFileName        { get; set; }
    public IlMergeFlags  Flags              { get; set; }
    public string        InternalizeExclude { get; set; }
    public IlMergeTarget Target             { get; set; }
}

[Flags]
public enum IlMergeFlags
{
    None = 0,
    Closed = 1,
    Internalize = 2,
    Log = 4,
    DeleteExcludedFiles = 16
}

public enum IlMergeTarget
{
    WinExe,
    Exe,
    Library
    //target:(library|exe|winexe)
}
