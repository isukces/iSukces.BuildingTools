using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iSukces.Build.DotnetRuntime;

public static class DotnetVersionChecker
{
    public static string DownloadPageUrl { get; set; } =
        "https://dotnet.microsoft.com/en-us/download/dotnet/10.0";

    private const string DotnetHost = "https://dotnet.microsoft.com";

    public static TimeSpan CacheMaxAge { get; set; }
        = TimeSpan.FromMinutes(30);

    public static string CacheDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DotnetRuntimeCache");

    private static readonly HttpClient HttpClient = CreateHttpClient();

    public static async Task<DotnetVersion> CheckDotnetVersion(DotnetChoose choose)
    {
        var cacheFile = Path.Combine(CacheDirectory, choose.CacheFileName);
        var cached    = TryReadFromCache(cacheFile);
        if (cached is not null)
            return cached;

        var html         = await HttpClient.GetStringAsync(DownloadPageUrl);
        var versionMatch = Regex.Match(html, choose.VersionRegexPattern, RegexOptions.IgnoreCase);
        if (!versionMatch.Success)
            throw new InvalidOperationException($"Nie udało się znaleźć najnowszej wersji '{choose.Name}' na stronie.");

        var version = versionMatch.Groups["version"].Value;
        var x64Url  = await ExtractRuntimeInstallerUrl(choose, html, version, "x64");
        var x86Url  = await ExtractRuntimeInstallerUrl(choose, html, version, "x86");

        var result = new DotnetVersion
        {
            LatestVersion = version,
            WindowsX64Url = x64Url,
            WindowsX86Url = x86Url
        };

        SaveToCache(cacheFile, result);
        return result;
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            // Strona potrafi zwracać niepoprawny atrybut Domain w Set-Cookie.
            // Wyłączenie obsługi cookies eliminuje CookieException.
            UseCookies = false
        };
        return new HttpClient(handler, disposeHandler: true);
    }

    private static async Task<string> ExtractRuntimeInstallerUrl(DotnetChoose choose, string html, string version, string architecture)
    {
        var pattern = choose.ThankYouPathRegexTemplate
            .Replace("{version}", Regex.Escape(version), StringComparison.Ordinal)
            .Replace("{architecture}", architecture, StringComparison.Ordinal);
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
        if (!match.Success)
            throw new InvalidOperationException(
                $"Nie udało się znaleźć URL dla '{choose.Name} {version}' i architektury windows-{architecture}.");

        var thankYouUrl  = DotnetHost + match.Groups["url"].Value;
        var thankYouHtml = await HttpClient.GetStringAsync(thankYouUrl);
        var downloadUrlPattern = choose.DownloadUrlRegexTemplate
            .Replace("{version}", Regex.Escape(version), StringComparison.Ordinal)
            .Replace("{architecture}", architecture, StringComparison.Ordinal);
        var downloadUrlMatch = Regex.Match(thankYouHtml, downloadUrlPattern, RegexOptions.IgnoreCase);
        if (!downloadUrlMatch.Success)
            throw new InvalidOperationException(
                $"Nie udało się znaleźć bezpośredniego URL .exe dla '{choose.Name} {version}' i architektury windows-{architecture}.");

        return downloadUrlMatch.Value.Replace(@"\/", "/");
    }

    private static DotnetVersion? TryReadFromCache(string cacheFile)
    {
        if (!File.Exists(cacheFile))
            return null;
        try
        {
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(cacheFile);
            if (DateTime.UtcNow - lastWriteTimeUtc > CacheMaxAge)
                return null;

            var json = File.ReadAllText(cacheFile);
            var obj  = JsonConvert.DeserializeObject<DotnetVersion>(json);
            return obj is not null && obj.IsValid() ? obj : null;
        }
        catch
        {
            return null;
        }
    }

    private static void SaveToCache(string cacheFile, DotnetVersion version)
    {
        try
        {
            Directory.CreateDirectory(CacheDirectory);
            var json = JsonConvert.SerializeObject(version, Formatting.Indented);
            File.WriteAllText(cacheFile, json);
        }
        catch
        {
            // Brak cache nie powinien blokować głównej funkcji.
        }
    }
}
