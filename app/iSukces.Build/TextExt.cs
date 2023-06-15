namespace iSukces.Build;

internal static class TextExt
{
    public static string Curly(this string s)
    {
        return $"{{{s}}}";
    }

    public static string Quote(this string s)
    {
        return $"\"{s}\"";
    }
}
