using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, RepositoryConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        services.AddSingleton(config);
        services.AddTransient<ILogradouroRepository>(sp => new LogradouroRepository(config.ConnectionString, config.DatabaseType));
        services.AddTransient<IAlunoRepository>(sp => new AlunoRepository(config.ConnectionString, config.DatabaseType));
        services.AddTransient<IColaboradorRepository>(sp => new ColaboradorRepository(config.ConnectionString, config.DatabaseType));
        services.AddTransient<IMatriculaRepository>(sp => new MatriculaRepository(config.ConnectionString, config.DatabaseType));
        services.AddTransient<ILogradouroService, LogradouroService>();
        services.AddTransient<IAlunoService, AlunoService>();
        services.AddTransient<IColaboradorService, ColaboradorService>();
        services.AddTransient<IMatriculaService, MatriculaService>();
        return services;
    }
}
