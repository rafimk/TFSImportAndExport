namespace TFSImportAndExport.Contracts;

public class WorkItemInfo
{
    public Int32 Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AssignTo { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string IterationPath { get; set; } = string.Empty;
}