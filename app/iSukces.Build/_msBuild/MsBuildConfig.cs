namespace iSukces.Build;

public class MsBuildConfig
{

    public string           Configuration { get; set; } = "RELEASE";
    public string           NoWarn        { get; set; } = "";
    public bool             Multiple      { get; set; }
    public MsBuildLogLevel? LogLevel      { get; set; }


    public bool? UseSharedCompilation { get; set; }
    public bool? NodeReuse            { get; set; }
    public int?  MaxCpuCount          { get; set; }


    public MsBuildConfig NoParallelBuild()
    {
        NodeReuse            = false;
        MaxCpuCount          = 1;
        UseSharedCompilation = false;
        return this;
    }
}
