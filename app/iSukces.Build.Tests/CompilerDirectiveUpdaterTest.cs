namespace iSukces.Build.Tests;

public class CompilerDirectiveUpdaterTest
{
    [Theory]
    [InlineData("", "")]
    //[InlineData("UNSET", "")]
    public void Test1(string input, string expected)
    {
        var op = new CommandLine
        {
            Directives =
            {
                ["UNSET"] = false,
                ["SET"]   = true,
            }
        };

        var list = input.Split(',').Where(a => a.Length > 0).ToHashSet();
        CompilerDirectiveUpdaterBase.UpdateDefineConstants(list, false, op);
        var result = string.Join(",", list.OrderBy(a => a));
        Assert.Equal(expected, result);
    }
}
