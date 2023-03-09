using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace TFSImportAndExport.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiControllerBase : ApiControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ?? = HttpContext.RequestServices.GetRequiredService<ISender>();
}
