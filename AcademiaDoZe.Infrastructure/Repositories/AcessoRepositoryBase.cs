using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public abstract class AcessoRepositoryBase<TEntity, TPessoa> : BaseRepository, IRepository<TEntity>
    where TEntity : Entity, IAggregateRoot
    where TPessoa : Pessoa
{
    private readonly int _pessoaTipo;
    private readonly Func<int, CancellationToken, Task<TPessoa?>> _obterPessoa;
    private readonly Func<int, TPessoa, DateTime, TEntity> _criar;
    private readonly Func<TEntity, int> _obterPessoaId;

    protected AcessoRepositoryBase(
        Func<(string ConnectionString, DatabaseType DatabaseType)> configurationProvider,
        int pessoaTipo,
        Func<int, CancellationToken, Task<TPessoa?>> obterPessoa,
        Func<int, TPessoa, DateTime, TEntity> criar,
        Func<TEntity, int> obterPessoaId)
        : base(configurationProvider)
    {
        _pessoaTipo = pessoaTipo;
        _obterPessoa = obterPessoa;
        _criar = criar;
        _obterPessoaId = obterPessoaId;
    }

    public async Task<TEntity?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT id_acesso, pessoa_id, data_hora FROM tb_acesso WHERE id_acesso = @Id AND pessoa_tipo = @Tipo";
        await using var command = await CreateCommandAsync(sql, cancellationToken);
        command.AddParameter("@Id", id, DbType.Int32);
        command.AddParameter("@Tipo", _pessoaTipo, DbType.Int32);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? await MapAsync(reader, cancellationToken) : null;
    }

    public async Task<IReadOnlyCollection<TEntity>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT id_acesso, pessoa_id, data_hora FROM tb_acesso WHERE pessoa_tipo = @Tipo ORDER BY data_hora";
        return await ConsultarAsync(sql, command => command.AddParameter("@Tipo", _pessoaTipo, DbType.Int32), cancellationToken);
    }

    public async Task<IReadOnlyCollection<TEntity>> ObterPorPessoaAsync(int pessoaId, DateOnly data, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT id_acesso, pessoa_id, data_hora FROM tb_acesso
            WHERE pessoa_tipo = @Tipo AND pessoa_id = @PessoaId
              AND data_hora >= @Inicio AND data_hora < @Fim ORDER BY data_hora, id_acesso";
        return await ConsultarAsync(sql, command =>
        {
            command.AddParameter("@Tipo", _pessoaTipo, DbType.Int32);
            command.AddParameter("@PessoaId", pessoaId, DbType.Int32);
            command.AddParameter("@Inicio", data.ToDateTime(TimeOnly.MinValue), DbType.DateTime2);
            command.AddParameter("@Fim", data.AddDays(1).ToDateTime(TimeOnly.MinValue), DbType.DateTime2);
        }, cancellationToken);
    }

    public async Task AdicionarAsync(TEntity entidade, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidade);
        const string sql = "INSERT INTO tb_acesso (pessoa_tipo, pessoa_id, data_hora) VALUES (@Tipo, @PessoaId, @DataHora)";
        await using var command = await CreateCommandAsync(FormatInsertQuery(sql), cancellationToken);
        command.AddParameter("@Tipo", _pessoaTipo, DbType.Int32);
        command.AddParameter("@PessoaId", _obterPessoaId(entidade), DbType.Int32);
        command.AddParameter("@DataHora", ObterDataHora(entidade), DbType.DateTime2);
        var id = await command.ExecuteScalarIdAsync("ERRO_REGISTRAR_ACESSO", "Não foi possível registrar o acesso.", cancellationToken);
        typeof(Entity).GetProperty("Id")?.SetValue(entidade, id);
    }

    public async Task AtualizarAsync(TEntity entidade, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidade);
        const string sql = "UPDATE tb_acesso SET pessoa_id = @PessoaId, data_hora = @DataHora WHERE id_acesso = @Id AND pessoa_tipo = @Tipo";
        await using var command = await CreateCommandAsync(sql, cancellationToken);
        command.AddParameter("@Id", entidade.Id, DbType.Int32);
        command.AddParameter("@Tipo", _pessoaTipo, DbType.Int32);
        command.AddParameter("@PessoaId", _obterPessoaId(entidade), DbType.Int32);
        command.AddParameter("@DataHora", ObterDataHora(entidade), DbType.DateTime2);
        if (await command.ExecuteNonQueryAsync(cancellationToken) == 0)
            throw new InvalidOperationException("O registro de acesso não foi encontrado.");
    }

    public async Task RemoverAsync(TEntity entidade, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidade);
        const string sql = "DELETE FROM tb_acesso WHERE id_acesso = @Id AND pessoa_tipo = @Tipo";
        await using var command = await CreateCommandAsync(sql, cancellationToken);
        command.AddParameter("@Id", entidade.Id, DbType.Int32);
        command.AddParameter("@Tipo", _pessoaTipo, DbType.Int32);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task<IReadOnlyCollection<TEntity>> ConsultarAsync(
        string sql, Action<DbCommand> configurar, CancellationToken cancellationToken)
    {
        await using var command = await CreateCommandAsync(sql, cancellationToken);
        configurar(command);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var rows = new List<(int Id, int PessoaId, DateTime DataHora)>();
        while (await reader.ReadAsync(cancellationToken))
            rows.Add((reader.GetInt32Value("id_acesso"), reader.GetInt32Value("pessoa_id"), reader.GetDateTimeValue("data_hora")));
        var result = new List<TEntity>(rows.Count);
        foreach (var row in rows)
        {
            var pessoa = await _obterPessoa(row.PessoaId, cancellationToken)
                ?? throw new InvalidOperationException($"A pessoa {row.PessoaId} do registro de acesso não existe.");
            result.Add(_criar(row.Id, pessoa, DateTime.SpecifyKind(row.DataHora, DateTimeKind.Local)));
        }
        return result.AsReadOnly();
    }

    private async Task<TEntity> MapAsync(DbDataReader reader, CancellationToken cancellationToken)
    {
        var id = reader.GetInt32Value("id_acesso");
        var pessoaId = reader.GetInt32Value("pessoa_id");
        var dataHora = reader.GetDateTimeValue("data_hora");
        var pessoa = await _obterPessoa(pessoaId, cancellationToken)
            ?? throw new InvalidOperationException($"A pessoa {pessoaId} do registro de acesso não existe.");
        return _criar(id, pessoa, DateTime.SpecifyKind(dataHora, DateTimeKind.Local));
    }

    private static DateTime ObterDataHora(TEntity entidade) => entidade switch
    {
        AcessoAluno acesso => acesso.DataHora,
        AcessoColaborador acesso => acesso.DataHora,
        _ => throw new NotSupportedException($"Tipo de acesso não suportado: {typeof(TEntity).Name}.")
    };
}
