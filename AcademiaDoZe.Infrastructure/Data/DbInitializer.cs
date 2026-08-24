// Nome: Túlio Thauã Dutra
using AcademiaDoZe.Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Reflection;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbInitializer
{
    private static readonly ConcurrentDictionary<string, bool> _bancosInicializados = new();

    public static async Task InicializarAsync(string connectionString, DatabaseType databaseType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        var key = $"{databaseType}: {connectionString}";
        if (_bancosInicializados.ContainsKey(key)) return;

        var scriptSql = ObterScript(databaseType);

        try
        {
            await using var connection = DbProvider.CreateConnection(connectionString, databaseType);
            await connection.OpenAsync(cancellationToken);

            await using var command = DbProvider.CreateCommand(scriptSql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);

            _bancosInicializados.TryAdd(key, true);
        }
        catch (DbException ex)
        {
            // Tratamento especial para SQL Server quando o banco ainda não existe (erro 4060)
            if (databaseType == DatabaseType.SqlServer && ex is SqlException sqlEx && sqlEx.Number == 4060)
            {
                try
                {
                    var builder = new SqlConnectionStringBuilder(connectionString);
                    var targetDb = builder.InitialCatalog;
                    // Conecta no banco master para criar o banco alvo se necessário
                    builder.InitialCatalog = "master";
                    var masterConnStr = builder.ToString();

                    await using var masterConnection = new SqlConnection(masterConnStr);
                    await masterConnection.OpenAsync(cancellationToken);

                    var createDbCmdText = $"IF DB_ID(N'{targetDb}') IS NULL CREATE DATABASE [{targetDb}];";
                    await using (var createCmd = masterConnection.CreateCommand())
                    {
                        createCmd.CommandText = createDbCmdText;
                        await createCmd.ExecuteNonQueryAsync(cancellationToken);
                    }

                    // Após criar, executa o script no banco alvo
                    await using var connection2 = DbProvider.CreateConnection(connectionString, databaseType);
                    await connection2.OpenAsync(cancellationToken);
                    await using var command2 = DbProvider.CreateCommand(scriptSql, connection2);
                    await command2.ExecuteNonQueryAsync(cancellationToken);

                    _bancosInicializados.TryAdd(key, true);
                    return;
                }
                catch (Exception inner)
                {
                    throw new InfrastructureException("ERRO_INICIALIZAR_BANCO", $"Erro ao inicializar banco de dados: {inner.Message}", inner);
                }
            }

            throw new InfrastructureException("ERRO_INICIALIZAR_BANCO", $"Erro ao inicializar banco de dados: {ex.Message}", ex);
        }
    }

    public static string ObterScript(DatabaseType databaseType)
    {
        var nomeScript = DbProvider.GetScriptName(databaseType);
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(r => r.EndsWith(nomeScript, StringComparison.OrdinalIgnoreCase))
            ?? throw new InfrastructureException("SCRIPT_EMBARCADO_NAO_ENCONTRADO", $"Script SQL embarcado '{nomeScript}' não encontrado.");

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InfrastructureException("ERRO_LEITURA_SCRIPT", $"Erro ao carregar o fluxo do script embarcado '{nomeScript}'.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}