using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using TFSImportAndExport;
using TFSImportAndExport.Application.Interfaces;
using TFSImportAndExport.Options;
using TFSImportAndExport.Persistence;
using TFSImportAndExport.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var tfsOptions = builder.Configuration.GetOptions<TfsOptions>("TfsOptions");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString,
                   builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));



builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services
    .AddSingleton(tfsOptions)
    .AddScoped<IWorkItemService, WorkItemService>()
    .AddMediatR(Assembly.GetExecutingAssembly())
    .AddControllers();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
