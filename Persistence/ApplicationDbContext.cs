using Microsoft.EntityFrameworkCore;
using TFSImportAndExport.Application.Interfaces;
using TFSImportAndExport.Entities;

namespace TFSImportAndExport.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}