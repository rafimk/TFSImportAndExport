namespace TFSImportAndExport.Contracts;

public class ParentWorkItem
{
    public Int32 WorkItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string WorkItemType { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public List<LinkWorkItem> Links { get; set; } = new();
}