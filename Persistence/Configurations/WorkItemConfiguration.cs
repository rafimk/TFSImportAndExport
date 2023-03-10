using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TFSImportAndExport.Entities;

namespace TFSImportAndExport.Persistence.Configurations;

public class WorkItemConfigurations : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.HasKey(x => x.WorkItemNo);
        builder.Property(x => x.WorkItemNo).ValueGeneratedNever();
    }
}