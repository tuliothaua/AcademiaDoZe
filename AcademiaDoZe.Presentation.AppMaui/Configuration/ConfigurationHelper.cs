using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static RepositoryConfig CreateRepositoryConfig()
    {
        var directory = FileSystem.AppDataDirectory;
        Directory.CreateDirectory(directory);
        var databasePath = Path.Combine(directory, "db_academia_do_ze.db");

        return new RepositoryConfig
        {
            ConnectionString = $"Data Source={databasePath};Cache=Shared;Default Timeout=5;",
            DatabaseType = AppDatabaseType.Sqlite.ToInfrastructure()
        };
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddApplicationServices(CreateRepositoryConfig());

        services.AddSingleton<AppShell>();
        services.AddTransient<ViewModels.DashboardViewModel>();
        services.AddTransient<ViewModels.LogradouroListViewModel>();
        services.AddTransient<ViewModels.LogradouroFormViewModel>();
        services.AddTransient<Views.DashboardPage>();
        services.AddTransient<Views.LogradouroListPage>();
        services.AddTransient<Views.LogradouroFormPage>();
    }
}
