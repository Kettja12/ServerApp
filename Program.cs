using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using ServerApp.Components;
using ServerApp.Services;
Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
          .WriteTo.File("../logs/Bootstrap.log",
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Verbose)
          .CreateBootstrapLogger();
Log.Information("Init");
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

var connectionString = builder.Configuration.GetConnectionString("Storage") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddTransient<Repository>(provider =>
    new Repository(connectionString)
);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddLocalization();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<Session>();

Log.Debug("Build");
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
Log.Debug("Run");
app.Run();
