using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Application.DependencyInjection;

public sealed class RepositoryConfig
{
    public required string ConnectionString { get; set; }
    public required DatabaseType DatabaseType { get; set; }
}

//Aluno: Túlio Thauã Dutra
