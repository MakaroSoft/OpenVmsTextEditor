using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenVmsTextEditor.Domain;
using OpenVmsTextEditor.Domain.Interfaces;
using File = OpenVmsTextEditor.Domain.Models.File;

namespace OpenVmsTextEditor.Infrastructure;

public class WindowsIo : IOperatingSystemIo
{
    private readonly ILogger<WindowsIo> _logger;

    // ReSharper disable once NotAccessedField.Local
    private readonly VmsEditorSettings _vmsEditorSettings;

    // this signature constructor must not change
    public WindowsIo(ILoggerFactory loggerFactory, IOptions<VmsEditorSettings> vmsEditorSettings)
    {
        _logger = loggerFactory.CreateLogger<WindowsIo>();
        _vmsEditorSettings = vmsEditorSettings.Value;
    }

    public async Task<IList<string>> GetDisks()
    {
        return await Task.Run(() => { return DriveInfo.GetDrives().Select(x => StripColonSlash(x.Name)).ToList(); });
    }

    private string StripColonSlash(string diskName)
    {
        var index = diskName.IndexOf(':');
        if (index == -1) return diskName;
        return diskName.Substring(0, index);
    }

    public async Task<IList<File>> GetDirectoryFiles(string filter, string fullFolderName)
    {
        _logger.LogDebug("GetDirectoryFiles(filter = {filter}, fullFolderName = {fullFolderName})", filter, fullFolderName);
        fullFolderName = FileFormatter.ToWindowsFolderFormat(fullFolderName);

        var dirs = Directory.GetDirectories(fullFolderName).Select(x => (File)new File
        {
            Name = Path.GetFileName(x),
            Dir = true
        }).ToList();

        var files = Directory.GetFiles(fullFolderName).Select(x => (File)new File
        {
            Name = Path.GetFileName(x)
        }).ToList();

        var filesAndFolders = dirs;
        dirs.AddRange(files);

        return await Task.Run(() => filesAndFolders);
    }

    public async Task<string> GetFile(string fullFileName)
    {
        _logger.LogDebug("GetFile(fullFileName = {fullFileName})", fullFileName);
        fullFileName = FileFormatter.ToWindowsFolderFormat(fullFileName);
        return await System.IO.File.ReadAllTextAsync(fullFileName);
    }

    public Task<string> SaveFile(string fullFileName, string fileData)
    {
        throw new NotImplementedException();
    }
}

internal class FileFormatter
{
    public static string ToWindowsFolderFormat(string folder)
    {
        var firstSlash = folder.IndexOf("/", StringComparison.Ordinal);
        if (firstSlash == -1)
        {
            folder += @":\";
        }
        else
        {
            if (firstSlash != 1) return folder;
            // we are windows
            var stringBuilder = new StringBuilder(folder);
            stringBuilder.Replace("/", @":\", firstSlash,firstSlash);

            folder = stringBuilder.ToString();

            folder = folder.Replace("/", @"\");
        }
        return folder;
    }
}