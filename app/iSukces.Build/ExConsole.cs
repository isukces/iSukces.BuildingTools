using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace iSukces.Build;

public static class ExConsole
{
    private static string Background(ConsoleColor c)
    {
        return Escape + "b" + ((int)c).ToString("X2");
    }

    public static string Foreground(ConsoleColor c)
    {
        return Escape + "f" + ((int)c).ToString("X2");
    }

    private static string? GetColor(object value)
    {
        if (value is FileInfo fi)
            value = new Filename(fi.FullName);
        if (value is Filename fn)
        {
            if (fn.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                return Foreground(ConsoleColor.Magenta);
            return Foreground(ConsoleColor.Blue);
        }

        if (value is TimeSpan)
            return Background(ConsoleColor.DarkGray) + Foreground(ConsoleColor.Cyan);

        return null;
    }

    private static ConsoleColor ParseColor(ref string text)
    {
        var number = text.Substring(1, 2);
        text = text.Substring(3);
        var i = int.Parse(number, NumberStyles.HexNumber);
        return (ConsoleColor)i;
    }

    public static void WriteException(Exception? exception)
    {
        if (exception is null)
            return;
        WriteLine($"{ForegroundRed}Exception {Reset}: {{0}}{{1}}", exception.GetType(), exception.Message);
        WriteLine("Stack:");
        WriteLine(exception.StackTrace);
    }

    public static void WriteLine(string format, params object[] args)
    {
        format = ParameterRegex.Replace(format, m =>
        {
            var value = m.Groups[1].Value;
            if (int.TryParse(value, out var idx))
            {
                var v            = args[idx];
                var colorSetting = GetColor(v);
                if (colorSetting is not null)
                    return colorSetting + m.Value + Reset;
            }

            return m.Value;
        });
        var text = string.Format(format, args);
        WriteLine(text);
    }

    public static void WriteLine(string text)
    {
        while (true)
        {
            var i = text.IndexOf(Escape);
            if (i < 0)
            {
                Console.WriteLine(text);
                Console.ResetColor();
                return;
            }

            var a = text.Substring(0, i);
            Console.Write(a);
            text = text.Substring(i + 1);
            if (text[0] == 'f')
            {
                Console.ForegroundColor = ParseColor(ref text);
                continue;
            }

            if (text[0] == 'b')
            {
                Console.BackgroundColor = ParseColor(ref text);
                continue;
            }

            if (text[0] == 'r')
            {
                Console.ResetColor();
                text = text.Substring(1);
                continue;
            }

            throw new NotImplementedException();
        }
    }

    public static string Reset => Escape + "r";

    public static string ForegroundRed => Foreground(ConsoleColor.Red);

    const string ParameterFilter = @"\{([^}]+)}";

    public const char Escape = (char)65533;

    static readonly Regex ParameterRegex =
        new Regex(ParameterFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
