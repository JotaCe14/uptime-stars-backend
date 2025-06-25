using Asp.Versioning;
using FluentValidation;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;
using Uptime.Stars.Api.Extensions;
using Uptime.Stars.Application;
using Uptime.Stars.Infrastructure;
using Uptime.Stars.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices();

builder.Services.AddPersistenceServices(builder.Configuration.GetConnectionString("Uptime"));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHangfireDashboard(options: new DashboardOptions() { Authorization = [] });

var apiVersionSet = app
     .NewApiVersionSet()
     .HasApiVersion(new ApiVersion(1))
     .ReportApiVersions()
     .Build();

var versionedGroup = app
    .MapGroup("api/v{apiVersion:apiVersion}")
    .WithApiVersionSet(apiVersionSet)
    .WithOpenApi();

app.MapEndpoints(versionedGroup);

app.Run();