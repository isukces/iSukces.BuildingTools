namespace iSukces.Build.InnoSetup;

public sealed class CommentCommand : Command
{
    public CommentCommand(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return ("; " + Text.Trim()).Trim();
    }

    public string Text { get; }
}
