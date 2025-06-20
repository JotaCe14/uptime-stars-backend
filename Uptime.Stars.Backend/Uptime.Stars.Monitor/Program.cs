using Hangfire;
using Uptime.Stars.Application;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Infrastructure;
using Uptime.Stars.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices();

builder.Services.AddPersistenceServices(builder.Configuration.GetConnectionString("Uptime"));

builder.Services.AddHangfireServer();

var host = builder.Build();

using IServiceScope serviceScope = host.Services.CreateScope();

using var dbContext = serviceScope.ServiceProvider.GetRequiredService<IDbContext>();

dbContext.Migrate();

host.Run();