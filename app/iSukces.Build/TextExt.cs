using System;

namespace iSukces.Build;

public static class TextExt
{
    public static string Curly(this string s)
    {
        return $"{{{s}}}";
    }

    [Obsolete("Use CliQuote instead", true)]
    public static string Quote(this string s)
    {
        return $"\"{s}\"";
    }

    public static string CliQuote(this string s)
    {
        return $"\"{s}\"";
    }

    public static string CliQuoteIfNecessary(this string s)
    {
        return s.Contains(' ')
            ? CliQuote(s)
            : s;
    }
}
