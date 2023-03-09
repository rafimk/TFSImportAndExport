namespace TFSImportAndExport.Application.Commands;

public class ImportCommand : IRequest<Unit>
{
    public string? Tags { get; set; } = string.Empty;
}

public class ImportCommandHandler : IRequestHanderl<ImportCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IWorkItemService _workItemService;
    private readonly ILogger<ImportCommandHandler> _logger;

    public ImportCommandHandler(IApplicationDbContext dbContext, IWorkItemService workItemService,
        ILogger<ImportCommandHandler> logger)
    {
        _dbContext = dbContext;
        _workItemService = workItemService;
        _logger = logger;
    }

    public async Task<Unit> Handle(ImportCommand request, CancelationToken cancelationToken)
    {
        _logger.LogInformation("Import job started ..........");

        var epicWorkItem = await _dbContext.WorkItems.
                                        .Where(x => x.Updated == 0 &&
                                                x.Type.ToLower() == "epic" && 
                                                (request.Tags == null || x.Tags.Contains(request.Tags)))
                                        .OrderBy(x => x.WorkItemNo)
                                        .ToListAsync();

        await BatchImport(epicWorkItem);

        var featureWorkItem = await _dbContext.WorkItems.
                                        .Where(x => x.Updated == 0 &&
                                                x.Type.ToLower() == "feature" && 
                                                (request.Tags == null || x.Tags.Contains(request.Tags)))
                                        .OrderBy(x => x.WorkItemNo)
                                        .ToListAsync();

        await BatchImport(featureWorkItem);

        var backLogWorkItem = await _dbContext.WorkItems.
                                        .Where(x => x.Updated == 0 &&
                                                x.Type.ToLower() == "product backlog item" && 
                                                (request.Tags == null || x.Tags.Contains(request.Tags)))
                                        .OrderBy(x => x.WorkItemNo)
                                        .ToListAsync();

        await BatchImport(backLogWorkItem);

        var acWorkItem = await _dbContext.WorkItems.
                                        .Where(x => x.Updated == 0 &&
                                                x.Type.ToLower() == "Acceptance Criteria" && 
                                                (request.Tags == null || x.Tags.Contains(request.Tags)))
                                        .OrderBy(x => x.WorkItemNo)
                                        .ToListAsync();

        await BatchImport(acWorkItem);

         var uiACWorkItem = await _dbContext.WorkItems.
                                        .Where(x => x.Updated == 0 &&
                                                x.Type.ToLower() == "UI Acceptance Criteria" && 
                                                (request.Tags == null || x.Tags.Contains(request.Tags)))
                                        .OrderBy(x => x.WorkItemNo)
                                        .ToListAsync();

        await BatchImport(uiACWorkItem);
       
        _logger.LogInformation("Import job completed ..........");
    }

    private async Task BatchImport(List<workItems> workItems)
    {
        foreach(var workItem in workItems)
        {
            var withParent = workItem.ClientParentId != null;
            var clientId = await _workItemService.CreateWit(workItem, withParent);

            workItem.ClientId = clientId;

            var childWorkItems = await _dbContext.WorkItems.Where(x => x.ParentId == workItem.WorkItemNo).ToListAsync();

            foreach(var childWorkItem in childWorkItems)
            {
                childWorkItem.ClientParentId = clientId;
            }
        }

        await _dbContext.SaveChangesAsync(cancelationToken);

    }
}