using System;

namespace iSukces.Build;

internal static class TextExt
{
    public static string Curly(this string s) => $"{{{s}}}";

    [Obsolete("Use CliQuote instead", true)]
    public static string Quote(this string s) => $"\"{s}\"";

    public static string CliQuote(this string s) => $"\"{s}\"";

    public static string CliQuoteIfNecessary(this string s)
    {
        return s.Contains(' ')
            ? CliQuote(s)
            : s;
    }
}
