using System;

namespace iSukces.Build.InnoSetup;

public abstract class Command
{
    public static Command Parse(string s, InnoSetupFile.Section section)
    {
        if (string.IsNullOrEmpty(s))
            return new EmptyLineCommand();
        var st = s.Trim();
        if (st.StartsWith(";", StringComparison.Ordinal)) return new CommentCommand(st.Substring(1));
        if (st.StartsWith("#define", StringComparison.Ordinal))
            return new DefineCommand(st.Substring("#define".Length));
        switch (section.Name)
        {
            case "Files":
            {
                var tmp = FileCommand.Parse(st);
                if (tmp is not null)
                    return tmp;
                break;
            }
            case "InstallDelete":
            {
                var tmp = InstallDeleteCommand.Parse(st);
                if (tmp is not null)
                    return tmp;
                break;
            }
            case "Setup":
            {
                var tmp = KeyValueCommand.Parse(st);
                if (tmp is not null)
                    return tmp;
                break;
            }
        }

        return new LineCommand(s);
    }
}
