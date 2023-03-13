using MediatR;
using Microsoft.EntityFrameworkCore;
using TFSImportAndExport.Application.Interfaces;
using TFSImportAndExport.Entities;
using TFSImportAndExport.Services;

namespace TFSImportAndExport.Application.Commands;

public class ImportCommand : IRequest
{
    public string? Tags { get; set; } = string.Empty;
}

public class ImportCommandHandler : IRequestHandler<ImportCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IWorkItemService _workItemService;


    public ImportCommandHandler(IApplicationDbContext dbContext, IWorkItemService workItemService)
    {
        _dbContext = dbContext;
        _workItemService = workItemService;
    }

    public async Task<Unit> Handle(ImportCommand request, CancellationToken cancellationToken)
    {
        var epicWorkItem = await _dbContext.WorkItems
            .Where(x => x.Updated == 0 &&
                   x.Type.ToLower() == "epic" && 
                  (request.Tags == null || x.Tags.Contains(request.Tags)))
            .OrderBy(x => x.WorkItemNo)
            .ToListAsync();

        await BatchImport(epicWorkItem, cancellationToken);

        //var featureWorkItem = await _dbContext.WorkItems
        //    .Where(x => x.Updated == 0 &&
        //            x.Type.ToLower() == "feature" && 
        //            (request.Tags == null || x.Tags.Contains(request.Tags)))
        //    .OrderBy(x => x.WorkItemNo)
        //    .ToListAsync();

        //await BatchImport(featureWorkItem, cancellationToken);

        //var backLogWorkItem = await _dbContext.WorkItems
        //    .Where(x => x.Updated == 0 &&
        //            x.Type.ToLower() == "product backlog item" && 
        //            (request.Tags == null || x.Tags.Contains(request.Tags)))
        //    .OrderBy(x => x.WorkItemNo)
        //    .ToListAsync();

        //await BatchImport(backLogWorkItem, cancellationToken);

        //var acWorkItem = await _dbContext.WorkItems
        //    .Where(x => x.Updated == 0 &&
        //            x.Type.ToLower() == "Acceptance Criteria" && 
        //            (request.Tags == null || x.Tags.Contains(request.Tags)))
        //    .OrderBy(x => x.WorkItemNo)
        //    .ToListAsync();

        //await BatchImport(acWorkItem, cancellationToken);

        // var uiACWorkItem = await _dbContext.WorkItems
        //    .Where(x => x.Updated == 0 &&
        //            x.Type.ToLower() == "UI Acceptance Criteria" && 
        //            (request.Tags == null || x.Tags.Contains(request.Tags)))
        //    .OrderBy(x => x.WorkItemNo)
        //    .ToListAsync();

        //await BatchImport(uiACWorkItem, cancellationToken);
       
        return Unit.Value;
    }

    private async Task BatchImport(List<WorkItem> workItems, CancellationToken cancellationToken)
    {
        foreach(var workItem in workItems)
        {
            var withParent = workItem.ClientParentId != null;
            var clientId = await _workItemService.CreateWit(workItem, withParent);

            workItem.ClientId = clientId;
            workItem.Updated = 1;

            _dbContext.WorkItems.Update(workItem);

            var childWorkItems = await _dbContext.WorkItems.Where(x => x.ParentId == workItem.WorkItemNo).ToListAsync();

            foreach(var childWorkItem in childWorkItems)
            {
                childWorkItem.ClientParentId = clientId;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

    }
}