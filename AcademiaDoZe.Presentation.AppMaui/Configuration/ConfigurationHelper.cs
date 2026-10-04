using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using MySql.Data.MySqlClient;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static RepositoryConfig CreateRepositoryConfig()
    {
        var savedType = Preferences.Default.Get("Banco_Tipo", nameof(AppDatabaseType.Sqlite));
        if (!Enum.TryParse<AppDatabaseType>(savedType, ignoreCase: true, out var databaseType))
            databaseType = AppDatabaseType.Sqlite;

        var sqlitePath = Preferences.Default.Get(
            "Sqlite_Caminho",
            Path.Combine(FileSystem.AppDataDirectory, "db_academia_do_ze.db"));

        return BuildRepositoryConfig(
            databaseType,
            Preferences.Default.Get("Banco_Servidor", string.Empty),
            Preferences.Default.Get("Banco_Porta", string.Empty),
            Preferences.Default.Get("Banco_Nome", "db_academia_do_ze"),
            Preferences.Default.Get("Banco_Usuario", string.Empty),
            Preferences.Default.Get("Banco_Senha", string.Empty),
            sqlitePath);
    }

    public static RepositoryConfig BuildRepositoryConfig(
        AppDatabaseType databaseType,
        string server,
        string port,
        string database,
        string username,
        string password,
        string sqlitePath)
    {
        string connectionString;
        DatabaseType infrastructureType;

        switch (databaseType)
        {
            case AppDatabaseType.Sqlite:
            {
                var path = string.IsNullOrWhiteSpace(sqlitePath)
                    ? Path.Combine(FileSystem.AppDataDirectory, "db_academia_do_ze.db")
                    : sqlitePath.Trim();
                var sqliteBuilder = new SqliteConnectionStringBuilder
                {
                    DataSource = path,
                    Cache = SqliteCacheMode.Shared,
                    DefaultTimeout = 5
                };
                connectionString = sqliteBuilder.ToString();
                infrastructureType = AppDatabaseType.Sqlite.ToInfrastructure();
                break;
            }

            case AppDatabaseType.SqlServer:
            {
                if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Informe servidor, banco de dados e usuário do SQL Server.");
                var sqlBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = string.IsNullOrWhiteSpace(port) ? server.Trim() : $"{server.Trim()},{port.Trim()}",
                    InitialCatalog = database.Trim(),
                    UserID = username.Trim(),
                    Password = password,
                    Encrypt = true,
                    TrustServerCertificate = true,
                    ConnectTimeout = 5
                };
                connectionString = sqlBuilder.ToString();
                infrastructureType = AppDatabaseType.SqlServer.ToInfrastructure();
                break;
            }

            case AppDatabaseType.MySql:
            {
                if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Informe servidor, banco de dados e usuário do MySQL.");
                if (!string.IsNullOrWhiteSpace(port) && !uint.TryParse(port, out _))
                    throw new ArgumentException("A porta do MySQL deve ser numérica.");
                var mysqlBuilder = new MySqlConnectionStringBuilder
                {
                    Server = server.Trim(),
                    Port = string.IsNullOrWhiteSpace(port) ? 3306u : uint.Parse(port),
                    Database = database.Trim(),
                    UserID = username.Trim(),
                    Password = password,
                    ConnectionTimeout = 5
                };
                connectionString = mysqlBuilder.ToString();
                infrastructureType = AppDatabaseType.MySql.ToInfrastructure();
                break;
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(databaseType), "Tipo de banco de dados não suportado.");
        }

        return new RepositoryConfig
        {
            ConnectionString = connectionString,
            DatabaseType = infrastructureType
        };
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        var repositoryConfig = CreateRepositoryConfig();
        services.AddApplicationServices(repositoryConfig);

        services.AddSingleton<AppShell>();
        services.AddTransient<ViewModels.DashboardViewModel>();
        services.AddTransient<ViewModels.LogradouroListViewModel>();
        services.AddTransient<ViewModels.LogradouroFormViewModel>();
        services.AddTransient<Views.DashboardPage>();
        services.AddTransient<Views.LogradouroListPage>();
        services.AddTransient<Views.LogradouroFormPage>();
        services.AddTransient<Views.ConfigPage>();
    }
}
