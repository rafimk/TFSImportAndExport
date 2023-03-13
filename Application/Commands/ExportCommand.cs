using MediatR;
using TFSImportAndExport.Application.Interfaces;
using TFSImportAndExport.Services;
using TFSImportAndExport.Entities;
using Microsoft.EntityFrameworkCore;

namespace TFSImportAndExport.Application.Commands;

public class ExportCommand : IRequest
{
    public Int32 ParentWorkItemNo { get; set; }
}

public class ExportCommandHandeler : IRequestHandler<ExportCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IWorkItemService _workItemService;

    public ExportCommandHandeler(IApplicationDbContext dbContext, IWorkItemService workItemService)
    {
        _dbContext = dbContext;
        _workItemService = workItemService;
    }

    public async Task<Unit> Handle(ExportCommand request, CancellationToken cancellationToken)
    {
        var workItems = await _workItemService.GetWorkItemsWithAllChild(request.ParentWorkItemNo.ToString());

        foreach (var workItem in workItems)
        {
            var workItemInfo = await _workItemService.GetWorkItem(workItem.WorkItemId.ToString());

            var existingWorkItem = await _dbContext.WorkItems.FirstOrDefaultAsync(x => x.WorkItemNo == workItem.WorkItemId);

            var parentLink = workItem.Links.FirstOrDefault(x => x.LinkTypeName == "Parent");

            if (existingWorkItem is null)
            {
                var workItemToAdd = new WorkItem
                {
                    WorkItemNo = workItem.WorkItemId,
                    Type = workItem.WorkItemType,
                    Title = workItem.Title,
                    Description = workItemInfo != null ? workItemInfo.Description : string.Empty,
                    State = workItem.State,
                    Tags = workItemInfo != null ? workItemInfo.Tags : string.Empty,
                    IterationPath = workItemInfo != null ? workItemInfo.IterationPath : string.Empty,
                    ParentId = parentLink != null ? parentLink.TargetWorkItemId : null,
                    ClientId = null,
                    ClientParentId = null,
                    Updated = 0
                };

                await _dbContext.WorkItems.AddAsync(workItemToAdd);
            }
            else
            {
                existingWorkItem.Type = workItem.WorkItemType;
                existingWorkItem.Title = workItem.Title;
                existingWorkItem.Description = workItemInfo != null ? workItemInfo.Description : string.Empty;
                existingWorkItem.State = workItem.State;
                existingWorkItem.Tags = workItemInfo != null ? workItemInfo.Tags : string.Empty;
                existingWorkItem.IterationPath = workItemInfo != null ? workItemInfo.IterationPath : string.Empty;
                existingWorkItem.ParentId = parentLink != null ? parentLink.TargetWorkItemId : null;
                existingWorkItem.ClientId = null;
                existingWorkItem.ClientParentId = null;
                existingWorkItem.Updated = 0;

                _dbContext.WorkItems.Update(existingWorkItem);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            
        }
        return Unit.Value;
    }
}