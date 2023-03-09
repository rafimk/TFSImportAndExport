namespace TFSImportAndExport.Application.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<WorkItem> WorkItems { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
}