namespace TFSImportAndExport;

public class TfsOptions
{
    public string SourceToken { get; set; } = string.Empty;
    public string SourceApiVersion { get; set; } = string.Empty;
    public string SourceBaseUrl { get; set; } = string.Empty;
    public string SourceProjectName { get; set; } = string.Empty;
    public string TragetToken { get; set; } = string.Empty;
    public string TargetApiVersion { get; set; } = string.Empty;
    public string TargetBaseUrl { get; set; } = string.Empty;
    public string TargetProjectName { get; set; } = string.Empty;
}