namespace TFSImportAndExport.Contracts;

public class LinkWorkItem
{
    public Int32 SourceWorkItemId { get; set;}
    public Int32 TargetWorkItemId { get; set; }
    public string LinkTypeName { get; set;} = string.Empty;
    public TargetWorkItem TargetWorkItem { get; set;}
}