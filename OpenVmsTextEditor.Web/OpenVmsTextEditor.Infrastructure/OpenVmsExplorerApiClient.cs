using System.Text;

namespace OpenVmsTextEditor.Infrastructure;

public interface IOpenVmsExplorerApiClient
{
    Task<HttpResponseMessage> GetDirectoryAsync(string path, bool showHistory, string? include, string? exclude, CancellationToken ct);
    Task<HttpResponseMessage> GetDisksAsync(CancellationToken ct);
    Task<HttpResponseMessage> GetFileAsync(string fullFileName, CancellationToken ct);
    Task<HttpResponseMessage> SaveFile(string fullFileName, string fileData, CancellationToken ct);
}

public class OpenVmsExplorerApiClient : IOpenVmsExplorerApiClient
{
    private readonly HttpClient _http;

    public OpenVmsExplorerApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<HttpResponseMessage> GetDirectoryAsync(string path, bool showHistory, string? include, string? exclude, CancellationToken ct)
    {
        var url = $"/api/directory/{Uri.EscapeDataString(path)}?showHistory={showHistory}";
        if (!string.IsNullOrEmpty(include)) url += $"&include={Uri.EscapeDataString(include)}";
        if (!string.IsNullOrEmpty(exclude)) url += $"&exclude={Uri.EscapeDataString(exclude)}";

        return await _http.GetAsync(url, ct);
    }

    public async Task<HttpResponseMessage> GetDisksAsync(CancellationToken ct)
    {
        return await _http.GetAsync("/api/disk", ct);
    }

    public async Task<HttpResponseMessage> GetFileAsync(string fullFileName, CancellationToken ct)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("accept", "text/plain");
        
        return await _http.GetAsync($"/api/file/{fullFileName}", ct);
    }

    public async Task<HttpResponseMessage> SaveFile(string fullFileName, string fileData, CancellationToken ct)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("accept", "text/plain");

        var iso = Encoding.GetEncoding("ISO-8859-1");
        var dflt = Encoding.Default;
        var utfBytes = dflt.GetBytes(fileData);
        var isoBytes = Encoding.Convert(dflt, iso, utfBytes);

        return await _http.PostAsync($"/api/file/{fullFileName}", new ByteArrayContent(isoBytes), ct);
    }
}