namespace iSukces.Build.InnoSetup;

public sealed class DefineCommand : Command
{
    public DefineCommand(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return ("#define " + Text.Trim()).Trim();
    }

    public string Text { get; }
}
