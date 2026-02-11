namespace iSukces.Build.DotnetRuntime;

public readonly record struct DotnetChoose(
    string Name,
    string VersionRegexPattern,
    string ThankYouPathRegexTemplate,
    string DownloadUrlRegexTemplate,
    string CacheFileName)
{
    public static readonly DotnetChoose AspNetCoreRuntime = new(
        Name: "ASP.NET Core Runtime",
        VersionRegexPattern:
        @"<h3\s+id=""runtime-aspnetcore-(?<version>\d+\.\d+\.\d+)""[^>]*>\s*ASP\.NET Core Runtime\s+\k<version>\s*</h3>",
        ThankYouPathRegexTemplate:
        @"href=""(?<url>/en-us/download/dotnet/thank-you/runtime-aspnetcore-{version}-windows-{architecture}-installer)""",
        DownloadUrlRegexTemplate:
        @"https://[^\s""'<>]+/aspnetcore-runtime-{version}-win-{architecture}\.exe",
        CacheFileName: "dotnet-aspnetcore-runtime-10.0.json");

    public static readonly DotnetChoose DesktopRuntime = new(
        Name: ".NET Desktop Runtime",
        VersionRegexPattern:
        @"<h3\s+id=""runtime-desktop-(?<version>\d+\.\d+\.\d+)""[^>]*>\s*\.NET Desktop Runtime\s+\k<version>\s*</h3>",
        ThankYouPathRegexTemplate:
        @"href=""(?<url>/en-us/download/dotnet/thank-you/runtime-desktop-{version}-windows-{architecture}-installer)""",
        DownloadUrlRegexTemplate:
        @"https://[^\s""'<>]+/windowsdesktop-runtime-{version}-win-{architecture}\.exe",
        CacheFileName: "dotnet-desktop-runtime-10.0.json");

    public static readonly DotnetChoose Runtime = new(
        Name: ".NET Runtime",
        VersionRegexPattern:
        @"<h3\s+id=""runtime-(?<version>\d+\.\d+\.\d+)""[^>]*>\s*\.NET Runtime\s+\k<version>\s*</h3>",
        ThankYouPathRegexTemplate:
        @"href=""(?<url>/en-us/download/dotnet/thank-you/runtime-{version}-windows-{architecture}-installer)""",
        DownloadUrlRegexTemplate:
        @"https://[^\s""'<>]+/dotnet-runtime-{version}-win-{architecture}\.exe",
        CacheFileName: "dotnet-runtime-10.0.json");

    public static readonly DotnetChoose Sdk = new(
        Name: ".NET SDK",
        VersionRegexPattern:
        @"href=""/en-us/download/dotnet/thank-you/sdk-(?<version>\d+\.\d+\.\d+)-windows-x64-installer""",
        ThankYouPathRegexTemplate:
        @"href=""(?<url>/en-us/download/dotnet/thank-you/sdk-{version}-windows-{architecture}-installer)""",
        DownloadUrlRegexTemplate:
        @"https://[^\s""'<>]+/dotnet-sdk-{version}-win-{architecture}\.exe",
        CacheFileName: "dotnet-sdk-10.0.json");
}
