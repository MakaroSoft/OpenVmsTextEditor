using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenVmsTextEditor.Domain.Models;

namespace OpenVmsTextEditor.Domain.Interfaces;

public interface IOperatingSystemIo
{
    Task<IList<string>> GetDisks(CancellationToken ct = default);
    Task<IList<File>> GetDirectoryFiles(string? include, string? exclude, bool showHistory, string fullFolderName, CancellationToken ct = default);
    Task<string> GetFile(string fullFileName, CancellationToken ct = default);
    Task<string> SaveFile(string fullFileName, string fileData, CancellationToken ct = default);
}