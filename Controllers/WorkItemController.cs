using Microsoft.AspNetCore.Mvc;
using TFSImportAndExport.Application.Commands;
using TFSImportAndExport.Contracts;
using TFSImportAndExport.Services;
using TFSImportAndExport.Utilities;

namespace TFSImportAndExport.Controllers;

public class WorkItemController : ApiControllerBase
{
    private readonly IWorkItemService _workItemService;
    private readonly ILogger<WorkItemController> _logger;

    public WorkItemController(IWorkItemService workItemService, ILogger<WorkItemController> logger)
    {
        _workItemService = workItemService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ParentWorkItem>>> Get(Int32 parentWorkItemNo)
    {
        var result = await _workItemService.GetWorkItemsWithAllChild(parentWorkItemNo.ToString());

        return Ok(result);
    }

    [HttpGet("GetWorkItem")]
    public async Task<ActionResult<WorkItemInfo>> GetWorkItem(Int32 workItemNo)
    {
        var result = await _workItemService.GetWorkItem(workItemNo.ToString());

        return Ok(result);
    }

    [HttpPost("Test")]
    public ActionResult<string> Test(string words)
    {
        var masked = words.Mask();

        return Ok(masked);
    }

    [HttpPost("Export")]
    public async Task<ActionResult> Export(ExportCommand command)
    {
        var result = await Mediator.Send(command);

        return NoContent();
    }

    [HttpPost("Import")]
    public async Task<ActionResult> Post(ImportCommand command)
    {
        var result = await Mediator.Send(command);

        return NoContent();
    }
}