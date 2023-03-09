namespace TFSImportAndExport.Application.Commands;

public class ExportCommand : IRequest<Unit>
{
    public Int32 ParentWorkItemNo { get; set; }
}

public class ExportCommandHandeler : IRequestHandler<ExportCommand, Uinit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IWorkItemService _workItemService;
    private ILogger<ExportCommandHandeler> _logger;

    public ExportCommandHandeler(IApplicationDbContext dbContext, IWorkItemService workItemService,
        Logger<ExportCommandHandeler> logger)
    {
        _dbContext = dbContext;
        _workItemService = workItemService;
        _logger = logger;
    }

    public async Task<Unit> Handle(ExportCommand request, CancelationToken cancelationToken)
    {
        _logger.LogInformation("Export job started .......");
        var workItems = await _workItemService.GetWorkItemsWithAllChild(request.ParentWorkItemNo.ToString());
        _logger.LogInformation($"Retreived : {workItems.Count}");


        foreach (var workItem in workItems)
        {
            var workItemInfo = await _workItemService.GetWorkItem(workItem.WorkItemId.ToString());

            var existingWorkItem = await _dbContext.WorkItems.FirstOrDefaultAsync(x => x.WorkItemNo == workItem.WorkItemId);

            var parentLink = workItem.Links.FirstOrDefault(x => x.LinkTypeName == "Parent");

            if (existingWorkItem is null)
            {
                var workItemToAdd = new workItem
                {
                    WorkItemNo = workItem.WorkItemId,
                    Type = workItem.Type,
                    Title = workItem.Title,
                    Desciption = workItemInfo != null workItemInfo.Desciption : string.Empty,
                    AssingTo = workItem.State,
                    Tags = workItemInfo != null ? workItemInfo.Tags : string.Empty,
                    IterationPath = workItemInfo != null ? workItemInfo.IterationPath : string.Empty,
                    ParentId = parentLink != null parentLink.TargetWorkItemId : null,
                    ClientId = null,
                    ClientParentId = null,
                    Updated = 0
                };

                await _dbContext.WorkItem.AddSync(workItemToAdd);
            }
            else
            {
                existingWorkItem.Type = workItem.Type,
                existingWorkItem.Title = workItem.Title,
                existingWorkItem.Desciption = workItemInfo != null workItemInfo.Desciption : string.Empty,
                existingWorkItem.AssingTo = workItem.State,
                existingWorkItem.Tags = workItemInfo != null ? workItemInfo.Tags : string.Empty,
                existingWorkItem.IterationPath = workItemInfo != null ? workItemInfo.IterationPath : string.Empty,
                existingWorkItem.ParentId = parentLink != null parentLink.TargetWorkItemId : null,
                existingWorkItem.ClientId = null,
                existingWorkItem.ClientParentId = null,
                existingWorkItem.Updated = 0

                _dbContext.WorkItems.Update(existingWorkItem);
            }

            await _dbContext.SaveChangesAsync(cancelationToken);

            _logger.LogInformation("Export job completed ........");

            return Unit.Value;
        }
    }
}