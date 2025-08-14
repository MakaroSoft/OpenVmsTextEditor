using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenVmsTextEditor.Domain;
using OpenVmsTextEditor.Domain.Interfaces;
using File = OpenVmsTextEditor.Domain.Models.File;

namespace OpenVmsTextEditor.Infrastructure;

/// <summary>
/// This class exists solely as a test method that bypasses the java API and accesses the files locally.
/// </summary>
public class TestWindowsNoHttpIo : IOperatingSystemIo
{
    private readonly ILogger<TestWindowsNoHttpIo> _logger;

    // ReSharper disable once NotAccessedField.Local
    private readonly VmsEditorSettings _vmsEditorSettings;

    // this signature constructor must not change
    public TestWindowsNoHttpIo(ILoggerFactory loggerFactory, IOptions<VmsEditorSettings> vmsEditorSettings)
    {
        _logger = loggerFactory.CreateLogger<TestWindowsNoHttpIo>();
        _vmsEditorSettings = vmsEditorSettings.Value;
    }

    public async Task<IList<string>> GetDisksAsync(CancellationToken ct)
    {
        return await Task.Run(() => { return DriveInfo.GetDrives().Select(x => StripColonSlash(x.Name)).ToList(); });
    }

    private string StripColonSlash(string diskName)
    {
        var index = diskName.IndexOf(':');
        if (index == -1) return diskName;
        return diskName.Substring(0, index + 1);
    }

    public async Task<IList<File>> GetDirectoryFilesAsync(string? include, string? exclude, bool showHistory, string fullFolderName,
        CancellationToken ct)
    {
        _logger.LogDebug("GetDirectoryFiles(include = {include}, exclude={exclude}, showHistory={showHistory}, fullFolderName = {fullFolderName})", include, exclude, showHistory, fullFolderName);
        fullFolderName = FileFormatter.ToWindowsFolderFormat(fullFolderName);

        var dirs = Directory.GetDirectories(fullFolderName).Select(x => new File
        {
            Name = Path.GetFileName(x),
            Dir = true
        }).ToList();

        string[] allFiles = Directory.GetFiles(fullFolderName);
        if (!string.IsNullOrWhiteSpace(exclude))
        {
            string[] patternsToExclude = exclude.Split(',');

            allFiles = allFiles
                        .Where(file => !patternsToExclude.Any(pattern => MatchesPattern(file, pattern)))
                        .ToArray();
        }

        if (!string.IsNullOrWhiteSpace(include))
        {
            string[] patternsToInclude = include.Split(',');

            allFiles = allFiles
                        .Where(file => patternsToInclude.Any(pattern => MatchesPattern(file, pattern)))
                        .ToArray();
        }

        var files = allFiles.Select(x => new File
        {
            Name = Path.GetFileName(x)
        }).ToList();


        var filesAndFolders = dirs;
        dirs.AddRange(files);

        return await Task.Run(() => filesAndFolders);
    }

    private bool MatchesPattern(string filePath, string pattern)
    {
        string fileName = Path.GetFileName(filePath);
        string regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$";
        return System.Text.RegularExpressions.Regex.IsMatch(fileName, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    public async Task<string> GetFileAsync(string fullFileName, CancellationToken ct)
    {
        _logger.LogDebug("GetFile(fullFileName = {fullFileName})", fullFileName);
        fullFileName = FileFormatter.ToWindowsFolderFormat(fullFileName);
        return await System.IO.File.ReadAllTextAsync(fullFileName);
    }

    public async Task<string> SaveFileAsync(string fullFileName, string fileData, CancellationToken ct )
    {
        _logger.LogDebug("SaveFile(fullFileName = {fullFileName})", fullFileName);
        await Task.CompletedTask;

        var filename = Path.GetFileName(fullFileName);

        return filename;
    }
}

internal class FileFormatter
{
    public static string ToWindowsFolderFormat(string folder)
    {
        var firstSlash = folder.IndexOf("/", StringComparison.Ordinal);
        if (firstSlash == -1)
        {
            // "C:" -> "C:\\"
            if (!folder.EndsWith(":")) return folder;
            folder += @"\";
        }
        else
        {
            // Expect drive:/path format for Windows
            var colon = folder.IndexOf(":", StringComparison.Ordinal);
            if (colon != 1) return folder;
            var stringBuilder = new StringBuilder(folder);
            stringBuilder.Replace(":/", @":\", colon, 2);
            folder = stringBuilder.ToString();
            folder = folder.Replace("/", @"\");
        }
        return folder;
    }
}