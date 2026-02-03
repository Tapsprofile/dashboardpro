using DashboardPortal.Application.Abstractions.Analytics;
using DashboardPortal.Application.Abstractions.Data;
using DashboardPortal.Application.Abstractions.Imports;
using DashboardPortal.Application.Services;
using DashboardPortal.Infrastructure.Background;
using DashboardPortal.Infrastructure.Parsing;
using DashboardPortal.Infrastructure.Providers;
using DashboardPortal.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy("AdminOrUser", policy => policy.RequireRole(Roles.Admin, Roles.User));
});

builder.Services.AddSingleton<IisW3cLogParser>();
builder.Services.AddSingleton<IDataProvider, IisLogProvider>();
builder.Services.AddSingleton<IImportQueue, ImportQueue>();
builder.Services.AddSingleton<IDashboardAggregationService, DashboardAggregationService>();
builder.Services.AddHostedService<LogImportWorker>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers().RequireAuthorization(new AuthorizeAttribute("AdminOrUser"));

app.Run();
