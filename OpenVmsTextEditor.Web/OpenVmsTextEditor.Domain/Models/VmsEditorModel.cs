using System.Collections.Generic;

namespace OpenVmsTextEditor.Domain.Models;

public class VmsEditorModel
{
    public IList<string> Disks { get; set; }
    public IList<File> Files { get; set; }
    public IList<string> BreadCrumb { get; set; }
}