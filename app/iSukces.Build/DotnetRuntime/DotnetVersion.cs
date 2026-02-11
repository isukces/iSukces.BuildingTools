namespace iSukces.Build.DotnetRuntime;

public sealed class DotnetVersion : IEquatable<DotnetVersion>
{
    public static bool operator ==(DotnetVersion? left, DotnetVersion? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DotnetVersion? left, DotnetVersion? right)
    {
        return !Equals(left, right);
    }

    public bool Equals(DotnetVersion? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return LatestVersion == other.LatestVersion 
               && WindowsX64Url == other.WindowsX64Url 
               && WindowsX86Url == other.WindowsX86Url;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is DotnetVersion other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LatestVersion, WindowsX64Url, WindowsX86Url);
    }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(LatestVersion))
            return false;
        if (string.IsNullOrWhiteSpace(WindowsX64Url))
            return false;
        if (string.IsNullOrWhiteSpace(WindowsX86Url))
            return false;
        if (!WindowsX64Url.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            return false;
        if (!WindowsX86Url.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
    }

    public string LatestVersion { get; init; } = string.Empty;
    public string WindowsX64Url { get; init; } = string.Empty;
    public string WindowsX86Url { get; init; } = string.Empty;
}
