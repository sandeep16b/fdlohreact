using Microsoft.EntityFrameworkCore;
using ReactAspNetApp.Data;
using ReactAspNetApp.FDWData;
using ReactAspNetApp.Services;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.UI;
using System.Security.Claims;

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
        builder.WithOrigins("https://localhost:44455", "http://localhost:44455", "https://localhost:44456", "http://localhost:44456", "http://localhost:3000", "https://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Add Azure AD Authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async ctx =>
            {
                var roleGroups = new Dictionary<string, string>();
                builder.Configuration.Bind("AuthorizationGroups", roleGroups);

                var graphService = await GraphService.CreateOnBehalfOfUserAsync(
                    ctx.SecurityToken.RawData, builder.Configuration);
                var memberGroups = await graphService.CheckMemberGroupsAsync(roleGroups.Keys);
                var claims = memberGroups.Select(g => new Claim(ClaimTypes.Role, roleGroups[g]));

                if (ctx.Principal != null)
                {
                    ctx.Principal.AddIdentity(new ClaimsIdentity(claims));
                }
            }
        };
    });

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Use camelCase for JSON property names
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    })
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AllowAnonymousToPage("/Index");
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
app.UseSession();

// Enable CORS (before routing)
app.UseCors("ReactAppPolicy");

app.UseRouting();

// Add Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map API controllers
app.MapControllers();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");


app.MapFallbackToFile("index.html");

// Login endpoint - requires authentication (user clicks Login button, gets redirected here)
app.MapGet("/account/login", () => Results.Redirect("/")).RequireAuthorization();

// Test endpoint to verify backend is working
app.MapGet("/api/test", () => "Backend on 44456 works!").AllowAnonymous();

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
