using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenVmsTextEditor.Domain;
using OpenVmsTextEditor.Domain.Interfaces;
using File = OpenVmsTextEditor.Domain.Models.File;

namespace OpenVmsTextEditor.Infrastructure;

public class VmsIo : IOperatingSystemIo
{
    private readonly ILogger<VmsIo> _logger;
    private readonly VmsEditorSettings _vmsEditorSettings;

    // this signature constructor must not change
    public VmsIo(ILoggerFactory loggerFactory, IOptions<VmsEditorSettings> vmsEditorSettings)
    {
        _logger = loggerFactory.CreateLogger<VmsIo>();
        _vmsEditorSettings = vmsEditorSettings.Value;
    }

    public async Task<IList<string>> GetDisks()
    {
        _logger.LogDebug("GetDisks()");

        using var client = new HttpClient();
        try
        {
            var vmsExplorerApiUrl = _vmsEditorSettings.VmsExplorerApiUrl;

            _logger.LogDebug("vmsExplorerApiUrl = {uri}", vmsExplorerApiUrl);

            client.DefaultRequestHeaders.Add("accept", "application/json");
            var requestUri = $"{vmsExplorerApiUrl}/api/disk";
            _logger.LogDebug("requestUri = {uri}", requestUri);

            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<string>>(result) ?? new List<string>();
            }

            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            _logger.LogError(e,e.Message);
            throw;
        }
    }


    public async Task<IList<File>> GetDirectoryFiles(string filter, string fullFolderName)
    {
        _logger.LogDebug("GetDirectoryFiles({0})", fullFolderName);

        using var client = new HttpClient();
        try
        {
            var vmsExplorerApiUrl = _vmsEditorSettings.VmsExplorerApiUrl;

            _logger.LogDebug("vmsExplorerApiUrl = {0}", vmsExplorerApiUrl);

            client.DefaultRequestHeaders.Add("accept", "application/json");
            var requestUri = $"{vmsExplorerApiUrl}/api/directory/{fullFolderName}";
            if (!string.IsNullOrWhiteSpace(filter)) requestUri += $"?filter={filter}";
            _logger.LogDebug("requestUri = {0}", requestUri);

            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<File>>(result) ?? new List<File>();
            }

            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<string> GetFile(string fullFileName)
    {
        _logger.LogDebug("GetFile({0})", fullFileName);

        using var client = new HttpClient();
        try
        {
            var vmsExplorerApiUrl = _vmsEditorSettings.VmsExplorerApiUrl;

            _logger.LogDebug("vmsExplorerApiUrl = {0}", vmsExplorerApiUrl);

            // todo, yikes, I may need to know if it is vms or windows to choose the charset

            client.DefaultRequestHeaders.Add("accept", "text/plain");
            var requestUri = $"{vmsExplorerApiUrl}/api/file/{fullFileName}";
            _logger.LogDebug("requestUri = {0}", requestUri);

            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<string> SaveFile(string fullFileName, string fileData)
    {
        _logger.LogDebug("SaveFile({0})", fullFileName);
        using var client = new HttpClient();
        try
        {
            var vmsExplorerApiUrl = _vmsEditorSettings.VmsExplorerApiUrl;

            _logger.LogDebug("vmsExplorerApiUrl = {url}", vmsExplorerApiUrl);

            // todo, yikes, I may need to know if it is vms or windows to choose the charset

            client.DefaultRequestHeaders.Add("accept", "text/plain");
            var requestUri = $"{vmsExplorerApiUrl}/api/file/{fullFileName}";
            _logger.LogDebug("requestUri = {0}", requestUri);


            var iso = Encoding.GetEncoding("ISO-8859-1");
            var dflt = Encoding.Default;
            var utfBytes = dflt.GetBytes(fileData);
            var isoBytes = Encoding.Convert(dflt, iso, utfBytes);
            //string msg = iso.GetString(isoBytes);


            var response = await client.PostAsync(requestUri, new ByteArrayContent(isoBytes));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}
