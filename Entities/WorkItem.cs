namespace TFSImportAndExport.Entities;

public class WorkItem
{
    public Int32 WorkItemNo { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AssignTo { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string IterationPath { get; set; } = string.Empty;
    public Int32? ParentId { get; set; }
    public Int32? ClientId { get; set; }
    public Int32? ClientParentId { get; set; }
    public Int32 Updated { get; set; }

}