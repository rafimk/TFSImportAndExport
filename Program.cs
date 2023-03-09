var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var tfsOptions = builder.Configuration.GetOption<tfsOptions>("TfsOption");

builder.Services
    .AddSingleton(tfsOptions)
    .AddScoped<IWorkItemService, WorkItemService>()
    .AddMediatR(Assembly.GetExecutingAssembly())
    .AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(tfsOptions =>
    tfsOptions.UseSqlServer(connectionString,
    b => b.MigrationAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

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
