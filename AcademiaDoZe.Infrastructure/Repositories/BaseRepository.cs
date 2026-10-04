using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public abstract class BaseRepository : IDisposable, IAsyncDisposable
{
    private readonly Func<(string ConnectionString, DatabaseType DatabaseType)> _configurationProvider;
    private string? _activeConnectionString;
    private DatabaseType? _activeDatabaseType;
    private DbConnection? _connection;
    private bool _disposed;

    protected string _connectionString => _configurationProvider().ConnectionString;
    protected DatabaseType _databaseType => _configurationProvider().DatabaseType;

    protected BaseRepository(string connectionString, DatabaseType databaseType)
        : this(() => (connectionString, databaseType))
    {
    }

    protected BaseRepository(Func<(string ConnectionString, DatabaseType DatabaseType)> configurationProvider)
    {
        _configurationProvider = configurationProvider
            ?? throw new ArgumentNullException(nameof(configurationProvider));
    }

    protected virtual async Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var (connectionString, databaseType) = _configurationProvider();
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InfrastructureException("STRING_CONEXAO_VAZIA", "String de conexão não pode ser vazia.");

        try
        {
            if (_connection is not null &&
                (_activeConnectionString != connectionString || _activeDatabaseType != databaseType))
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            await DbInitializer.InicializarAsync(connectionString, databaseType, cancellationToken);

            if (_connection == null)
            {
                _connection = DbProvider.CreateConnection(connectionString, databaseType);
                await _connection.OpenAsync(cancellationToken);
                _activeConnectionString = connectionString;
                _activeDatabaseType = databaseType;
            }
            else if (_connection.State == ConnectionState.Broken)
            {
                await _connection.CloseAsync();
                await _connection.OpenAsync(cancellationToken);
            }
            else if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync(cancellationToken);
            }

            return _connection;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("FALHA_ABRIR_CONEXAO", "Falha ao abrir conexão com o banco de dados.", ex);
        }
    }

    protected virtual async Task<DbCommand> CreateCommandAsync(string commandText, CancellationToken cancellationToken = default)
    {
        var connection = await GetOpenConnectionAsync(cancellationToken);
        return DbProvider.CreateCommand(commandText, connection);
    }

    protected string FormatInsertQuery(string insertSql) => DbProvider.FormatInsertQuery(insertSql, _databaseType);
    protected string GetCurrentDateFunction() => DbProvider.GetCurrentDateFunction(_databaseType);
    protected string GetDateAddDaysExpression(string dateExpr, string daysParam) => DbProvider.GetDateAddDaysExpression(dateExpr, daysParam, _databaseType);
    protected string GetDateHourExpression(string dateColumn) => DbProvider.GetDateHourExpression(dateColumn, _databaseType);
    protected string GetDateMonthExpression(string dateColumn) => DbProvider.GetDateMonthExpression(dateColumn, _databaseType);
    protected string GetDateDayExpression(string dateColumn) => DbProvider.GetDateDayExpression(dateColumn, _databaseType);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connection?.Dispose();
                _connection = null;
            }
            _disposed = true;
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync().ConfigureAwait(false);
            _connection = null;
        }
    }
}
