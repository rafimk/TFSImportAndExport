using Microsoft.EntityFrameworkCore;
using TFSImportAndExport.Entities;

namespace TFSImportAndExport.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<WorkItem> WorkItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
}