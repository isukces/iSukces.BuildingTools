using System;

namespace iSukces.Build;

public struct SlnProjectId : IEquatable<SlnProjectId>
{
    public SlnProjectId(string value) => Value = Guid.Parse(value.Trim());

    public SlnProjectId(Guid value) => Value = value;

    public static bool operator ==(SlnProjectId left, SlnProjectId right) => left.Equals(right);

    public static bool operator !=(SlnProjectId left, SlnProjectId right) => !left.Equals(right);

    public bool Equals(SlnProjectId other) => Value.Equals(other.Value);

    public override bool Equals(object obj) => obj is SlnProjectId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString("B").ToUpper();

    #region Properties

    public Guid Value { get; }

    #endregion

    #region Fields

    public static readonly SlnProjectId Folder = new SlnProjectId("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");

    #endregion
}
