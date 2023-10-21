namespace OpenVmsTextEditor.Domain.Models
{
    public class File
    {
        public string Name { get; set; } = string.Empty;
        public bool Dir { get; set; }
        public long Mod { get; set; }
    }
}
