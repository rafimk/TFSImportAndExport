namespace TFSImportAndExport.Services;

public interface IWorkItemService
{
    Task<List<ParentWorkItem>> GetWorkItemsWithAllChild(string epicWorkItemNo);
    Task<WorkItemInfo> GetWorkItem(string workItemNo);
    Task<int> CreateWit(WorkItem workItem, bool withParent);
}