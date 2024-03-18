using System;

namespace iSukces.Build;

public static class Repeat
{
    public static void RunTwice(Action action)
    {
        try
        {
            action();
            return;
        }
        catch
        {
        }

        action();
    }
}
