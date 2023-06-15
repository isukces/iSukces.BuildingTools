using System;
using System.Text.RegularExpressions;

namespace iSukces.Build;

public sealed class AssemblyCompiledManager
{
    public static string Replace(string text, DateTimeOffset date)
    {
        if (text is null)
            return null;
        var dateStr   = date.ToString("O");
        var attribute = $@"[assembly:AssemblyCompiled(""{dateStr}"")]";

        var found = false;
        text = AssemblyCompiledRegex.Replace(text, _ =>
        {
            found = true;
            return attribute;
        });
        if (found)
            return text;
        text = text + "\r\n" + attribute + "\r\n";
        return text;
    }

    const string AssemblyCompiledFilter = @"\[\s*assembly\s*:AssemblyCompiled[^\]]*\]";

    static readonly Regex AssemblyCompiledRegex =
        new Regex(AssemblyCompiledFilter, RegexOptions.Multiline | RegexOptions.Compiled);

    /* ===============
\[
\s*
assembly
\s*
:
AssemblyCompiled
[^\]]*
\]

=============== */
}
