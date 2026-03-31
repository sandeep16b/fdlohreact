using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.Data;
using ReactAspNetApp.FDWData;
using ReactAspNetApp.Services;
using NLog;
using NLog.Web;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add FDW DbContext for external database
builder.Services.AddDbContext<FDWDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FDWConnection")));

// Register database initialization service
builder.Services.AddScoped<DatabaseInitializationService>();

// Register centralized logging service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILoggingService, LoggingService>();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:44455", "http://localhost:44455", "https://localhost:44456", "http://localhost:44456")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Use camelCase for JSON property names
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

var app = builder.Build();

// Initialize database on startup
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
    var dbLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        dbLogger.LogInformation("Initializing database...");
        await dbInitializer.InitializeDatabaseAsync();
        
        var stats = await dbInitializer.GetDatabaseStatisticsAsync();
        dbLogger.LogInformation("Database initialized successfully. Statistics: {Stats}", stats.ToString());
    }
    catch (Exception ex)
    {
        dbLogger.LogError(ex, "Failed to initialize database. Application will continue but may not function properly.");
        // Continue running the application even if database initialization fails
        // This allows for manual intervention if needed
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable CORS
app.UseCors("ReactAppPolicy");

// Map API controllers
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}
