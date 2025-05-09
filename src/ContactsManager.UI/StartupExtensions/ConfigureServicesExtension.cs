using ContactsManager.Core.Domain.IdentityEntities;
using CRUD.NET.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUD.NET.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddTransient<ResponseHeaderActionFilter>();
        services.AddControllersWithViews(options =>
        {
            var logger = services.BuildServiceProvider()
                .GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

            options.Filters.Add(new ResponseHeaderActionFilter(logger)
            {
                Key = "My-Key-Fom-Global",
                Value = "MyValueGlobal",
                Order = 2
            });

            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });

        services.AddScoped<ICountriesRepository, CountriesRepository>();
        services.AddScoped<IPersonsRepository, PersonsRepository>();

        services.AddScoped<ICountriesGetterService, CountriesGetterService>();
        services.AddScoped<ICountriesAdderService, CountriesAdderService>();
        services.AddScoped<ICountriesUpdaterService, CountriesUpdaterService>();

        services.AddScoped<PersonsGetterService>(); // Register independently
        services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>(); // This will now work

        services.AddScoped<IPersonsAdderService, PersonsAdderService>();
        services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
        services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
        services.AddScoped<IPersonsSorterService, PersonsSorterService>();

        services.AddTransient<PersonsListActionFilter>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (environment.IsEnvironment("Test"))
            {
                options.UseInMemoryDatabase("TestDatabase");
            }
            else
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 5;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build(); // Default policy, enforces authentication for all endpoints (user must be authenticated) for all endpoints or action methods

            options.AddPolicy("NotAuthorized", policy =>
                {
                    policy.RequireAssertion(context =>
                        !context.User.Identity!.IsAuthenticated);
                });
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
        });

        services.AddHttpLogging(options =>
        {
            options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
        });

        return services;
    }
}
