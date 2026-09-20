using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Application.DependencyInjection;

public sealed class RepositoryConfig
{
    public required string ConnectionString { get; init; }
    public required DatabaseType DatabaseType { get; init; }
}
