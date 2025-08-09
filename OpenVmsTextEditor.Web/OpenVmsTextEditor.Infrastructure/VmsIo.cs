using System.Text.Json;
using Microsoft.Extensions.Logging;
using OpenVmsTextEditor.Domain.Interfaces;
using File = OpenVmsTextEditor.Domain.Models.File;

namespace OpenVmsTextEditor.Infrastructure;

public class VmsIo : IOperatingSystemIo
{
    private readonly ILogger<VmsIo> _logger;
    private readonly IOpenVmsExplorerApiClient _openVmsExplorerApiClient;

    // this signature constructor must not change
    public VmsIo(ILoggerFactory loggerFactory, 
        IOpenVmsExplorerApiClient openVmsExplorerApiClient)
    {
        _logger = loggerFactory.CreateLogger<VmsIo>();
        _openVmsExplorerApiClient = openVmsExplorerApiClient;
    }

    public async Task<IList<string>> GetDisksAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("GetDisks()");
        
        var resp = await _openVmsExplorerApiClient.GetDisksAsync( ct);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<string>>(json) ?? [];
    }
    
    public async Task<IList<File>> GetDirectoryFilesAsync(string? include, string? exclude, bool showHistory,
        string fullFolderName, CancellationToken ct = default)
    {
        var resp = await _openVmsExplorerApiClient.GetDirectoryAsync(fullFolderName, showHistory, include, exclude, ct);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<File>>(json) ?? [];
    }

    public async Task<string> GetFileAsync(string fullFileName, CancellationToken ct = default)
    {
        _logger.LogDebug("GetFile({0})", fullFileName);

        try
        {

            var response = await _openVmsExplorerApiClient.GetFileAsync(fullFileName, ct);
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

    public async Task<string> SaveFileAsync(string fullFileName, string fileData, CancellationToken ct = default)
    {
        _logger.LogDebug("SaveFile({0})", fullFileName);

        try
        {

            var response = await _openVmsExplorerApiClient.SaveFile(fullFileName, fileData, ct);
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

