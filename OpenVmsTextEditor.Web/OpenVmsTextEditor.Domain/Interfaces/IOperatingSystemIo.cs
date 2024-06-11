using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenVmsTextEditor.Domain.Models;

namespace OpenVmsTextEditor.Domain.Interfaces;

public interface IOperatingSystemIo
{
    Task<IList<string>> GetDisks();
    Task<IList<File>> GetDirectoryFiles(string? include, string? exclude, bool showHistory, string fullFolderName);
    Task<string> GetFile(string fullFileName);
    Task<string> SaveFile(string fullFileName, string fileData);
}