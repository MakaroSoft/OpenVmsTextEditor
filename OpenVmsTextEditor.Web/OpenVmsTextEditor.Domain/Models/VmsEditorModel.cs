using System.Collections.Generic;

namespace OpenVmsTextEditor.Domain.Models;

public class VmsEditorModel
{
    public required IList<string> Disks { get; init; }
    public required IList<File> Files { get; init; }
    public required IList<string> BreadCrumb { get; init; }
}