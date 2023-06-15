namespace iSukces.Build.InnoSetup;

public sealed class LineCommand : Command
{
    public LineCommand(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return Text;
    }

    public string Text { get; set; }
}
