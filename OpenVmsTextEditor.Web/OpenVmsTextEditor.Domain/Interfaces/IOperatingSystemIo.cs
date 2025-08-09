using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenVmsTextEditor.Domain.Models;

namespace OpenVmsTextEditor.Domain.Interfaces;

public interface IOperatingSystemIo
{
    Task<IList<string>> GetDisksAsync(CancellationToken ct);
    Task<IList<File>> GetDirectoryFilesAsync(string? include, string? exclude, bool showHistory, string fullFolderName, CancellationToken ct);
    Task<string> GetFileAsync(string fullFileName, CancellationToken ct);
    Task<string> SaveFileAsync(string fullFileName, string fileData, CancellationToken ct);
}