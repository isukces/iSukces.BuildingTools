namespace iSukces.Build;

internal static class TextExt
{
    public static string Curly(this string s) => $"{{{s}}}";

    public static string Quote(this string s) => $"\"{s}\"";
}
