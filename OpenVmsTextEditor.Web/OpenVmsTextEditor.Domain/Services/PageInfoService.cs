using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenVmsTextEditor.Domain.Interfaces;
using OpenVmsTextEditor.Domain.Models;

namespace OpenVmsTextEditor.Domain.Services;

public interface IPageInfoService
{
    Task<VmsEditorModel> GetPageInfo(string? include, string? exclude, bool showHistory, string path);
}

public class PageInfoService : IPageInfoService
{
    private readonly ILogger<PageInfoService> _logger;
    private readonly IOperatingSystemIo _operatingSystemIo;

    public PageInfoService(ILogger<PageInfoService> logger, IOperatingSystemIo operatingSystemIo)
    {
        _logger = logger;
        _operatingSystemIo = operatingSystemIo;
    }

    public async Task<VmsEditorModel> GetPageInfo(string? include, string? exclude, bool showHistory, string path)
    {
        _logger.LogDebug("GetFolder(include={include}, exclude={exclude}, showHistory={showHistory}, path={path})", include, exclude, showHistory, path);

        var disks = await _operatingSystemIo.GetDisks();

        var startingDirectory = path ?? disks[0];
        var breadCrumb = startingDirectory.Split("/").ToList();

        var files = await _operatingSystemIo.GetDirectoryFiles(include, exclude, showHistory, startingDirectory);

        var model = new VmsEditorModel
        {
            Disks = disks,
            Files = files,
            BreadCrumb = breadCrumb
        };
        return model;
    }
}