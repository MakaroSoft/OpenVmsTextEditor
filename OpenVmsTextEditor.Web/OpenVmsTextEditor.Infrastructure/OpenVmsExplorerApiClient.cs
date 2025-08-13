using System.Text;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<OpenVmsExplorerApiClient> _logger;

    public OpenVmsExplorerApiClient(HttpClient http, ILogger<OpenVmsExplorerApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> GetDirectoryAsync(string path, bool showHistory, string? include, string? exclude, CancellationToken ct)
    {
        var url = $"/api/directory/{Uri.EscapeDataString(path)}?showHistory={showHistory}";
        if (!string.IsNullOrEmpty(include)) url += $"&include={Uri.EscapeDataString(include)}";
        if (!string.IsNullOrEmpty(exclude)) url += $"&exclude={Uri.EscapeDataString(exclude)}";
        var absolute = _http.BaseAddress != null ? new Uri(_http.BaseAddress, url) : new Uri(url, UriKind.RelativeOrAbsolute);
        _logger.LogDebug("GET {RequestUri}", absolute);
        return await _http.GetAsync(url, ct);
    }

    public async Task<HttpResponseMessage> GetDisksAsync(CancellationToken ct)
    {
        var url = "/api/disk";
        var absolute = _http.BaseAddress != null ? new Uri(_http.BaseAddress, url) : new Uri(url, UriKind.RelativeOrAbsolute);
        _logger.LogDebug("GET {RequestUri}", absolute);
        return await _http.GetAsync(url, ct);
    }

    public async Task<HttpResponseMessage> GetFileAsync(string fullFileName, CancellationToken ct)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("accept", "text/plain");
        var url = $"/api/file/{fullFileName}";
        var absolute = _http.BaseAddress != null ? new Uri(_http.BaseAddress, url) : new Uri(url, UriKind.RelativeOrAbsolute);
        _logger.LogDebug("GET {RequestUri}", absolute);
        return await _http.GetAsync(url, ct);
    }

    public async Task<HttpResponseMessage> SaveFile(string fullFileName, string fileData, CancellationToken ct)
    {
        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("accept", "text/plain");

        var iso = Encoding.GetEncoding("ISO-8859-1");
        var dflt = Encoding.Default;
        var utfBytes = dflt.GetBytes(fileData);
        var isoBytes = Encoding.Convert(dflt, iso, utfBytes);
        var url = $"/api/file/{fullFileName}";
        var absolute = _http.BaseAddress != null ? new Uri(_http.BaseAddress, url) : new Uri(url, UriKind.RelativeOrAbsolute);
        _logger.LogDebug("POST {RequestUri}", absolute);
        return await _http.PostAsync(url, new ByteArrayContent(isoBytes), ct);
    }
}