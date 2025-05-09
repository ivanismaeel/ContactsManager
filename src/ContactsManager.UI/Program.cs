using CRUD.NET.Middleware;
using CRUD.NET.StartupExtensions;
using OfficeOpenXml;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

// Create an instance of EPPlusLicense
var epplusLicense = new EPPlusLicense();

// Read EPPlus license configuration from appsettings.json
var licenseContext = builder.Configuration["EPPlus:ExcelPackage:LicenseContext"];
if (licenseContext == "NonCommercial")
{
    epplusLicense.SetNonCommercialPersonal("YourName"); // Correctly using an instance of EPPlusLicense
}
else if (licenseContext == "Commercial")
{
    // Example for commercial use (ensure you have a valid license key)
    epplusLicense.SetCommercial("YourLicenseKey"); // Correctly using an instance of EPPlusLicense
}

builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error"); // Use a custom error page in production
    app.UseExceptionHandlingMiddleware();
}

app.UseHsts(); // Use HTTP Strict Transport Security (HSTS) in production
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseHttpLogging();

if (builder.Environment.IsEnvironment("Test") == false)
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();

app.UseRouting(); // Add routing middleware to handle incoming requests, identifying action methods and based on the route template
app.UseAuthentication();// Add authentication middleware, reading identity from cookies
app.UseAuthorization(); // Add authorization middleware to check user permissions
app.MapControllers(); // Map controllers to routes, executing the action methods filter pipeline (e.g., action filters, authorization filters, etc.)

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    _ = endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}");
});
app.Run();

public partial class Program { }
